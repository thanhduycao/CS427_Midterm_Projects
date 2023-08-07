using System.Collections.Generic;
using UnityEngine;

public class PlayersTracking : MonoBehaviour
{
    [SerializeField] private HealthPlayerPanel m_HealthPlayerPanelPrefab;
    [SerializeField] private Transform m_HealthPlayerPanelParent;

    private readonly List<HealthPlayerPanel> _HealthPlayerPanels = new();

    // action for update health
    public delegate void UpdateHealth(ulong playerId, int health);
    public static event UpdateHealth OnUpdateHealth;

    private void OnEnable()
    {
        foreach (Transform child in m_HealthPlayerPanelParent) Destroy(child.gameObject);
        _HealthPlayerPanels.Clear();
    }

    public void NetworkPlayersUpdated(Dictionary<ulong, PlayerData> list)
    {
        var allActivePlayerIds = list.Keys;
        foreach (var playerId in allActivePlayerIds)
        {
            var playerData = list[playerId];
            var playerPanel = _HealthPlayerPanels.Find(panel => panel.GetPlayerId() == playerId);

            if (playerPanel == null)
            {
                playerPanel = Instantiate(m_HealthPlayerPanelPrefab, m_HealthPlayerPanelParent);
                _HealthPlayerPanels.Add(playerPanel);
            }

            playerPanel.SetPlayerId(playerId);
            playerPanel.SetPlayerName(playerData.name);
            playerPanel.SetPlayerHealth(100);

            // add callback for update health
            OnUpdateHealth += playerPanel.SetPlayerHealth;
        }
    }

    public void UpdatePlayerHealth(ulong playerId, int health)
    {
        OnUpdateHealth?.Invoke(playerId, health);
    }
}
