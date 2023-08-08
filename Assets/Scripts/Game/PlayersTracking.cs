using System.Collections.Generic;
using UnityEngine;

public class PlayersTracking : MonoBehaviour
{
    [SerializeField] private HealthPlayerPanel m_HealthPlayerPanelPrefab;
    [SerializeField] private Transform m_HealthPlayerPanelParent;

    private readonly List<HealthPlayerPanel> _HealthPlayerPanels = new();

    public delegate void OnUpdatedPlayerState(ulong playerId, PlayerState playerState);
    public static event OnUpdatedPlayerState OnPlayerStateUpdated;

    private void OnEnable()
    {
        foreach (Transform child in m_HealthPlayerPanelParent) Destroy(child.gameObject);
        _HealthPlayerPanels.Clear();
    }

    public void NetworkPlayersUpdated(Dictionary<ulong, PlayerState> list)
    {
        var allActivePlayerIds = list.Keys;
        foreach (var playerId in allActivePlayerIds)
        {
            var playerState = list[playerId];
            var playerPanel = _HealthPlayerPanels.Find(panel => panel.GetPlayerId() == playerId);

            if (playerPanel == null)
            {
                playerPanel = Instantiate(m_HealthPlayerPanelPrefab, m_HealthPlayerPanelParent);
                _HealthPlayerPanels.Add(playerPanel);
            }

            playerPanel.SetPlayerId(playerId);
            playerPanel.Set(playerState);
            OnPlayerStateUpdated += playerPanel.SetPlayerState;
        }
    }

    public void RemovePlayer(ulong playerId)
    {
        var playerPanel = _HealthPlayerPanels.Find(panel => panel.GetPlayerId() == playerId);
        if (playerPanel != null)
        {
            // remove callback for update health
            OnPlayerStateUpdated -= playerPanel.SetPlayerState;

            _HealthPlayerPanels.Remove(playerPanel);
            Destroy(playerPanel.gameObject);
        }
    }

    public void UpdatePlayerState(ulong playerId, PlayerState playerState)
    {
        OnPlayerStateUpdated?.Invoke(playerId, playerState);
    }
}
