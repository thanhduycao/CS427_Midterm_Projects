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

    [Header("Game Assets")]
    [SerializeField] private GameObject _gameFinishedUI = null;
    [SerializeField] private GameObject _gameLooserUI = null;
    [SerializeField] private GameObject _gameEndedUI = null;
    [SerializeField] private GameObject _gamePausedUI = null;

    private readonly Dictionary<ulong, PlayerState> m_PlayerState = new();

    // Game State
    private NetworkVariable<int> _numberOfPlayers = new NetworkVariable<int>(0);
    private NetworkVariable<int> _numberOfPlayersFinished = new NetworkVariable<int>(0);
    private NetworkVariable<int> _numberOfPlayersAlive = new NetworkVariable<int>(0);
    private NetworkVariable<bool> _gameStarted = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> _gamePaused = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> _gameEnded = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> _gameFinished = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> _gameLooser = new NetworkVariable<bool>(false);

    public int NumberOfPlayers { get => _numberOfPlayers.Value; set => _numberOfPlayers.Value = value; }
    public int NumberOfPlayersFinished
    {
        get => _numberOfPlayersFinished.Value;
        set
        {
            _numberOfPlayersFinished.Value = value;
            if (AllPlayersFinished)
            {
                _gameFinished.Value = true;
                _gameEnded.Value = true;
            }
        }
    }
    public int NumberOfPlayersAlive
    {
        get => _numberOfPlayersAlive.Value;
        set
        {
            _numberOfPlayersAlive.Value = value;
            if (AllPlayersDead)
            {
                _gameLooser.Value = true;
                _gameEnded.Value = true;
            }
        }
    }
    public bool GameStarted { get => _gameStarted.Value; set => _gameStarted.Value = value; }
    public bool GamePaused { get => _gamePaused.Value; set => _gamePaused.Value = value; }
    public bool GameEnded { get => _gameEnded.Value; set => _gameEnded.Value = value; }
    public bool AllPlayersFinished { get => NumberOfPlayersFinished == NumberOfPlayers; }
    public bool AllPlayersDead { get => NumberOfPlayersAlive == 0; }
    public bool GameFinished { get => AllPlayersFinished || AllPlayersDead; }
    // ~Game State


    private void Awake()
    {
        if (_spawnArea != null)
        {
            _spawnStartY = _spawnArea.position.y - _spawnArea.localScale.y / 2;
            _spawnEndY = _spawnArea.position.y + _spawnArea.localScale.y / 2;
            _spawnStartX = _spawnArea.position.x - _spawnArea.localScale.x / 2;
            _spawnEndX = _spawnArea.position.x + _spawnArea.localScale.x / 2;
        }

        GameStarted = true;
    }

    private void Start()
    {
        if (IsServer) GetPlayerDataServerRpc();
        else RequestUpdatePlayerDataServerRpc();
        if (_PlayersTracking != null) _PlayersTracking.NetworkPlayersUpdated(m_PlayerState);

        // set OnValueChanged callbacks
        _gameFinished.OnValueChanged += OnGameFinished;
        _gameLooser.OnValueChanged += OnGameLooser;
        _gameEnded.OnValueChanged += OnGameEnded;
    }

    public override void OnNetworkSpawn()
    {
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        base.OnNetworkSpawn();
    }

    private void OnGameFinished(bool oldValue, bool newValue)
    {
        if (newValue)
            Debug.Log("===== GAME FINISHED =====");
        _gameFinishedUI?.SetActive(newValue);
    }

    private void OnGameLooser(bool oldValue, bool newValue)
    {
        if (newValue)
            Debug.Log("===== GAME LOOSER =====");
        _gameLooserUI?.SetActive(newValue);
    }

    private void OnGameEnded(bool oldValue, bool newValue)
    {
        if (newValue)
            Debug.Log("===== GAME ENDED =====");
        // _gameEndedUI?.SetActive(newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnPlayerStateChangedServerRpc()
    {
        // reset game state
        NumberOfPlayersFinished = 0;
        NumberOfPlayersAlive = NumberOfPlayers;

        foreach (KeyValuePair<ulong, PlayerState> data in m_PlayerState)
        {
            Debug.Log($"Id {data.Key}; Name {data.Value.Name} - Health {data.Value.Health}");
            if (!data.Value.IsAlive)
            {
                Debug.Log($"-   Player {data.Value.Name} is dead");
                NumberOfPlayersAlive -= 1;
            }
            if (data.Value.IsFinished)
            {
                Debug.Log($"-   Player {data.Value.Name} is finished");
                NumberOfPlayersFinished += 1;
            }
        }
        Debug.Log("====================================");
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

    public void UpdatePlayerState(PlayerState playerState)
    {
        UpdatePlayerStateServerRpc(playerState);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerStateServerRpc(PlayerState playerState)
    {
        if (playerState == null) return;
        if (m_PlayerState.ContainsKey(playerState.Id))
        {
            m_PlayerState[playerState.Id] = playerState;
        }
        else
        {
            m_PlayerState.Add(playerState.Id, playerState);
        }

        UpdatePlayerStateClientRpc(playerState);
    }

    [ClientRpc]
    private void UpdatePlayerStateClientRpc(PlayerState playerState)
    {
        if (playerState == null) return;
        if (m_PlayerState.ContainsKey(playerState.Id))
        {
            m_PlayerState[playerState.Id] = playerState;
        }
        else
        {
            m_PlayerState.Add(playerState.Id, playerState);
        }
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
        //if (m_PlayerState.ContainsKey(NetworkManager.Singleton.LocalClientId))
        //{
        //    m_PlayerState.Remove(NetworkManager.Singleton.LocalClientId);
        //}
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
        foreach (KeyValuePair<ulong, PlayerData> data in MatchmakingService._playersInLobby)
        {
            if (m_PlayerState.ContainsKey(data.Key))
            {
                m_PlayerState[data.Key] = new PlayerState(data.Value);
                m_PlayerState[data.Key].Id = data.Key; // just in case
            }
            else
            {
                m_PlayerState.Add(data.Key, new PlayerState(data.Value));
                m_PlayerState[data.Key].Id = data.Key; // just in case
            }
        }

        NumberOfPlayers = m_PlayerState.Count;
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
        _PlayersTracking?.NetworkPlayersUpdated(m_PlayerState);
    }
}