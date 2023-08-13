using Unity.Netcode;
using UnityEngine;

public class SyncHealthBar : NetworkBehaviour
{
    [SerializeField] private float _RefreshRate = 2;

    private Animator m_Animator;
    private HealthControler m_HealControler;
    private PlayersTracking m_PlayersTracking;
    private PlayerState m_playerState;
    private float _nextRefreshTime;

    private bool _setCallback = false;

    private void Awake()
    {
        m_HealControler = GetComponent<HealthControler>();
        m_PlayersTracking = FindObjectOfType<PlayersTracking>();
        m_Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Time.time >= _nextRefreshTime) PropagateToClients();

        if (m_playerState == null)
        {
            m_playerState = FindObjectOfType<GameManager>()?.GetPlayerState(OwnerClientId);
            if (m_playerState != null)
            {
                if (!_setCallback && IsOwner)
                {
                    m_playerState.OnValueChange += OnPlayerStateChange;
                    FindObjectOfType<GameManager>().OnGameDestroy += OnGameDestroy;
                    FindObjectOfType<GameManager>().OnLeaveGame += OnLeaveGame;
                    FindObjectOfType<GameManager>().OnRemovePlayerEvent += OnRemovePlayer;
                    _setCallback = true;
                }
                m_HealControler.SetPlayerName(m_playerState.Name);
                m_HealControler.SetColor(m_playerState.Color);
                m_Animator.runtimeAnimatorController = GlobalVariable.Instance.GetAvatar(m_playerState.Avatar).AvatarAnimator;
            }
        }
    }

    public void OnGameDestroy()
    {
        Destroy(gameObject);
    }

    public void OnLeaveGame()
    {
        OnDestroyPlayer();
    }

    public void OnRemovePlayer(ulong clientId)
    {
        if (m_playerState != null && clientId == m_playerState.Id)
        {
            OnDestroyPlayer();
        }
    }

    public override void OnDestroy()
    {
        OnDestroyPlayer();
        base.OnDestroy();
    }

    public void OnDestroyPlayer()
    {
        if (m_playerState != null)
        {
            m_playerState.OnValueChange -= OnPlayerStateChange;
        }
        if (m_PlayersTracking != null)
        {
            m_PlayersTracking.RemovePlayer(OwnerClientId);
        }
        //FindObjectOfType<GameManager>().OnRemovePlayerEvent -= OnRemovePlayer;
        FindObjectOfType<GameManager>()?.OnRemovePlayer(OwnerClientId);
    }

    private void PropagateToClients()
    {
        _nextRefreshTime = Time.time + _RefreshRate;
        if (IsOwner)
        {
            if (IsServer) UpdatePlayerStateClientRpc(m_playerState);
            else UpdatePlayerStateServerRpc(m_playerState);
        }
        else
        {
            if (IsServer) return;
            UpdatePlayerStateClientRpc(m_playerState);
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
            if (m_playerState != null)
                if (IsOwner || !IsServer)
                {
                    m_playerState.OnValueChange += OnPlayerStateChange;
                    m_playerState.Health = newHeal;
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
            if (m_playerState != null)
                if (IsOwner || !IsServer)
                {
                    m_playerState.OnValueChange += OnPlayerStateChange;
                    m_playerState.IsFinished = true;
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
        m_playerState = playerState;
        UpdateInterface();
        FindObjectOfType<GameManager>()?.UpdatePlayerState(m_playerState);
        UpdatePlayerStateClientRpc(playerState);
    }

    [ClientRpc]
    private void UpdatePlayerStateClientRpc(PlayerState playerState)
    {
        m_playerState = playerState;
        FindObjectOfType<GameManager>()?.UpdatePlayerState(m_playerState);
        UpdateInterface();
    }

    private void UpdateInterface()
    {
        if (m_playerState == null) return;
        m_HealControler.SetHealth(m_playerState.Health);
        if (m_PlayersTracking == null && IsClient) // recheck if null
            m_PlayersTracking = FindObjectOfType<PlayersTracking>();
        if (m_PlayersTracking != null)
        {
            m_PlayersTracking.UpdatePlayerState(OwnerClientId, m_playerState);
        }
    }
}
