using Unity.Netcode;
using UnityEngine;

public class SyncHealthBar : NetworkBehaviour
{
    [SerializeField] private float _RefreshRate = 2;

    private HealthControler m_HealControler;
    private PlayersTracking m_PlayersTracking;
    private float _nextRefreshTime;

    private void Awake()
    {
        m_HealControler = GetComponent<HealthControler>();
        m_PlayersTracking = FindObjectOfType<PlayersTracking>();
    }

    private void Update()
    {
        if (Time.time >= _nextRefreshTime) Fetch();
    }

    private void Fetch()
    {
        _nextRefreshTime = Time.time + _RefreshRate;

        PlayerData _playerData = FindObjectOfType<GameManager>()?.GetPlayerData(OwnerClientId);
        if (_playerData != null)
        {
            m_HealControler.SetPlayerName(_playerData.name);
        }

        if (IsOwner)
        {
            if (IsServer)
            {
                UpdateHealthClientRpc(m_HealControler.GetHealth());
            }
            else
            {
                UpdateHealthServerRpc(m_HealControler.GetHealth());
            }
        }
        else
        {
            if (IsServer) return;
            UpdateHealthClientRpc(m_HealControler.GetHealth());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Trap _))
        {
            var trap = collision.gameObject.GetComponent<TrapController>();
            int newHeal = TakeDamage(trap.Damage);
            if (IsOwner)
            {
                if (IsServer)
                {
                    UpdateHealthClientRpc(newHeal);
                }
                else
                {
                    UpdateHealthServerRpc(newHeal);
                }
            }
            else
            {
                if (IsServer) return;
                UpdateHealthClientRpc(newHeal);
            }
        }
    }

    private int TakeDamage(int dame)
    {
        return m_HealControler.GetHealth() - dame;
    }

    // make Request Collision ServerRpc and ClientRpc   
    [ServerRpc]
    private void UpdateHealthServerRpc(int newHealth)
    {
        m_HealControler.SetHealth(newHealth);
        if (m_PlayersTracking != null)
        {
            m_PlayersTracking.UpdatePlayerHealth(OwnerClientId, newHealth);
        }
        UpdateHealthClientRpc(newHealth);
    }

    [ClientRpc]
    private void UpdateHealthClientRpc(int newHealth)
    {
        m_HealControler.SetHealth(newHealth);
        if (m_PlayersTracking == null)
            m_PlayersTracking = FindObjectOfType<PlayersTracking>();
        if (m_PlayersTracking != null)
        {
            m_PlayersTracking.UpdatePlayerHealth(OwnerClientId, newHealth);
        }
    }
}
