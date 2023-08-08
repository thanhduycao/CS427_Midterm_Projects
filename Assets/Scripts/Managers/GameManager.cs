using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerController _playerPrefab;

    [Header("Spawn Area")]
    [SerializeField] Transform _spawnArea;
    [SerializeField] private float _spawnStartY = 0f;
    [SerializeField] private float _spawnEndY = 0f;
    [SerializeField] private float _spawnStartX = 0f;
    [SerializeField] private float _spawnEndX = 1f;

    [Header("Services")]
    [SerializeField] private PlayersTracking _PlayersTracking;

    private readonly Dictionary<ulong, PlayerState> m_PlayerState = new();

    private void Awake()
    {
        if (_spawnArea != null)
        {
            _spawnStartY = _spawnArea.position.y - _spawnArea.localScale.y / 2;
            _spawnEndY = _spawnArea.position.y + _spawnArea.localScale.y / 2;
            _spawnStartX = _spawnArea.position.x - _spawnArea.localScale.x / 2;
            _spawnEndX = _spawnArea.position.x + _spawnArea.localScale.x / 2;
        }
    }

    private void Start()
    {
        if (IsServer) GetPlayerDataServerRpc();
        else RequestUpdatePlayerDataServerRpc();
        if (_PlayersTracking != null) _PlayersTracking.NetworkPlayersUpdated(m_PlayerState);
    }

    public override void OnNetworkSpawn()
    {
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        base.OnNetworkSpawn();
    }

    public Dictionary<ulong, PlayerState> GetPlayersState()
    {
        return m_PlayerState;
    }

    public PlayerState GetPlayerState(ulong clientId)
    {
        if (m_PlayerState.ContainsKey(clientId))
        {
            return m_PlayerState[clientId];
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
            if (m_PlayerState.ContainsKey(data.Key))
            {
                m_PlayerState[data.Key] = new PlayerState(data.Value);
            }
            else
            {
                m_PlayerState.Add(data.Key, new PlayerState(data.Value));
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestUpdatePlayerDataServerRpc()
    {
        foreach (KeyValuePair<ulong, PlayerState> data in m_PlayerState)
        {
            SendPlayerDataClientRpc(data.Key, data.Value);
        }
    }

    [ClientRpc]
    private void SendPlayerDataClientRpc(ulong clientId, PlayerState playerState)
    {
        if (IsServer) return;
        if (m_PlayerState.ContainsKey(clientId))
        {
            m_PlayerState[clientId] = playerState;
        }
        else
        {
            m_PlayerState.Add(clientId, playerState);
        }
        _PlayersTracking.NetworkPlayersUpdated(m_PlayerState);
    }
}