using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerController _playerPrefab;

    [Header("Spawn Points")]
    [SerializeField] private float _spawnStartY = 0f;
    [SerializeField] private float _spawnEndY = 0f;
    [SerializeField] private float _spawnStartX = 0f;
    [SerializeField] private float _spawnEndX = 1f;

    [Header("Services")]
    [SerializeField] private PlayersTracking _PlayersTracking;

    private readonly Dictionary<ulong, PlayerData> m_PlayerData = new();

    private void Start()
    {
        if (IsServer) GetPlayerDataServerRpc();

        //Debug.Log($"ClientID: {NetworkManager.Singleton.LocalClientId}");
        //Debug.Log($"IsServer: {IsServer}");
        //Debug.Log($"IsClient: {IsClient}");
        //Debug.Log($"IsHost: {IsHost}");
        //Debug.Log($"IsOwner: {IsOwner}");

        if (!IsServer) RequestUpdatePlayerDataServerRpc();
        if (_PlayersTracking != null) _PlayersTracking.NetworkPlayersUpdated(m_PlayerData);
    }

    public override void OnNetworkSpawn()
    {
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        base.OnNetworkSpawn();
    }

    public Dictionary<ulong, PlayerData> GetPlayersData()
    {
        return m_PlayerData;
    }

    public PlayerData GetPlayerData(ulong clientId)
    {
        if (m_PlayerData.ContainsKey(clientId))
        {
            return m_PlayerData[clientId];
        }
        return null;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong playerId)
    {
        // make random spawn point
        var spawnPoint = new Vector3(Random.Range(_spawnStartX, _spawnEndX), Random.Range(_spawnStartY, _spawnEndY), 0f);
        var spawn = Instantiate(_playerPrefab, spawnPoint, Quaternion.identity);
        spawn.NetworkObject.SpawnWithOwnership(playerId);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        LeaveLobby();
    }

    public async void LeaveLobby()
    {
        await MatchmakingService.LeaveLobby();
        if (NetworkManager.Singleton != null) NetworkManager.Singleton.Shutdown();
    }

    [ServerRpc(RequireOwnership = false)]
    private void GetPlayerDataServerRpc()
    {
        foreach (var data in MatchmakingService._playersInLobby)
        {
            if (m_PlayerData.ContainsKey(data.Key))
            {
                m_PlayerData[data.Key] = data.Value;
            }
            else
            {
                m_PlayerData.Add(data.Key, data.Value);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestUpdatePlayerDataServerRpc()
    {
        foreach (KeyValuePair<ulong, PlayerData> data in m_PlayerData)
        {
            SendPlayerDataClientRpc(data.Key, data.Value);
        }
    }

    [ClientRpc]
    private void SendPlayerDataClientRpc(ulong clientId, PlayerData playerData)
    {
        if (IsServer) return;
        if (m_PlayerData.ContainsKey(clientId))
        {
            m_PlayerData[clientId] = playerData;
        }
        else
        {
            m_PlayerData.Add(clientId, playerData);
        }
        _PlayersTracking.NetworkPlayersUpdated(m_PlayerData);
    }
}