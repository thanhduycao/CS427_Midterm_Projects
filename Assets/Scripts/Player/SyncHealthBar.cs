using Unity.Netcode;
using UnityEngine;

public class SyncHealthBar : NetworkBehaviour
{
    [SerializeField] private float _RefreshRate = 2;

    private HealthControler m_HealControler;
    private PlayersTracking m_PlayersTracking;
    private PlayerState _playerState;
    private float _nextRefreshTime;

    private void Awake()
    {
        m_HealControler = GetComponent<HealthControler>();
        m_PlayersTracking = FindObjectOfType<PlayersTracking>();
    }

    private void Update()
    {
        if (Time.time >= _nextRefreshTime) PropagateToClients();

        if (_playerState == null)
        {
            _playerState = FindObjectOfType<GameManager>()?.GetPlayerState(OwnerClientId);
            if (_playerState != null)
            {
                // _playerState.OnValueChange += OnPlayerStateChange;
                m_HealControler.SetPlayerName(_playerState.Name);
                m_HealControler.SetColor(_playerState.Color);
            }
        }
    }

    public override void OnDestroy()
    {
        if (m_PlayersTracking != null)
        {
            m_PlayersTracking.RemovePlayer(OwnerClientId);
        }
        base.OnDestroy();
    }

    private void PropagateToClients()
    {
        _nextRefreshTime = Time.time + _RefreshRate;
        if (IsOwner)
        {
            if (IsServer) UpdatePlayerStateClientRpc(_playerState);
            else UpdatePlayerStateServerRpc(_playerState);
        }
        else
        {
            if (IsServer) return;
            UpdatePlayerStateClientRpc(_playerState);
        }
    }

    public void OnPlayerStateChange()
    {
        PropagateToClients();
        if (IsOwner)
            FindObjectOfType<GameManager>()?.OnPlayerStateChangedServerRpc();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Trap _))
        {
            var trap = collision.gameObject.GetComponent<TrapController>();
            int newHeal = TakeDamage(trap.Damage);
            if (_playerState != null)
                if (IsOwner || !IsServer)
                {
                    _playerState.OnValueChange += OnPlayerStateChange;
                    _playerState.Health = newHeal;
                }

            MovementNoMana playerMovement = gameObject.GetComponent<MovementNoMana>();
            playerMovement.KBCounter = playerMovement.KBTotalTime;
            if (playerMovement.GetFacingProperty() == true)
            {
                playerMovement.KnockFromRight = true;
            }
            if (playerMovement.GetFacingProperty() == false)
            {
                playerMovement.KnockFromRight = false;
            }
        }
        else if (collision.TryGetComponent(out FinishFlag _))
        {
            if (_playerState != null)
                if (IsOwner || !IsServer)
                {
                    _playerState.OnValueChange += OnPlayerStateChange;
                    _playerState.IsFinished = true;
                }
        }
    }

    private int TakeDamage(int dame)
    {
        return m_HealControler.GetHealth() - dame;
    }

    [ServerRpc]
    private void UpdatePlayerStateServerRpc(PlayerState playerState)
    {
        _playerState = playerState;
        UpdateInterface();
        FindObjectOfType<GameManager>()?.UpdatePlayerState(_playerState);
        UpdatePlayerStateClientRpc(playerState);
    }

    [ClientRpc]
    private void UpdatePlayerStateClientRpc(PlayerState playerState)
    {
        _playerState = playerState;
        FindObjectOfType<GameManager>()?.UpdatePlayerState(_playerState);
        UpdateInterface();
    }

    private void UpdateInterface()
    {
        if (_playerState == null) return;
        m_HealControler.SetHealth(_playerState.Health);
        if (m_PlayersTracking == null && IsClient) // recheck if null
            m_PlayersTracking = FindObjectOfType<PlayersTracking>();
        if (m_PlayersTracking != null)
        {
            m_PlayersTracking.UpdatePlayerState(OwnerClientId, _playerState);
        }
    }
}
