using Unity.Netcode;
using UnityEngine;

public class SyncHealthBar : NetworkBehaviour
{
    private HealthControler m_HealControler;
    private PlayersTracking m_PlayersTracking;

    private void Awake()
    {
        m_HealControler = GetComponent<HealthControler>();
        m_PlayersTracking = FindObjectOfType<PlayersTracking>();
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
