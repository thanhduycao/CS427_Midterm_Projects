using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
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
    private NetworkVariable<int> _numberOfPlayers = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> _numberOfPlayersFinished = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> _numberOfPlayersAlive = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> _gameStarted = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> _gamePaused = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> _gameEnded = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> _gameFinished = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> _gameLooser = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public Action OnGameDestroy;
    public Action OnLeaveGame;

    public delegate void OnRemovePlayerHandle(ulong clientId);
    public event OnRemovePlayerHandle OnRemovePlayerEvent;

    public delegate void OnDeSpawnPlayerHandle(ulong clientId, Vector3 position);
    public event OnDeSpawnPlayerHandle OnDeSpawnPlayerEvent;

    private bool _isQuitting = false;
    private Dictionary<ulong, PlayerController> _playersPrefab = new();

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
            if (OnePlayerDead)
            {
                _gameLooser.Value = true;
            }
        }
    }
    public bool GameStarted { get => _gameStarted.Value; set => _gameStarted.Value = value; }
    public bool GamePaused { get => _gamePaused.Value; set => _gamePaused.Value = value; }
    public bool GameEnded { get => _gameEnded.Value; set => _gameEnded.Value = value; }
    public bool OnePlayerDead { get => NumberOfPlayersAlive != NumberOfPlayers; }
    public bool AllPlayersFinished { get => NumberOfPlayersFinished == NumberOfPlayers && NumberOfPlayers != 0; }
    public bool AllPlayersDead { get => NumberOfPlayersAlive == 0; }
    public bool GameFinished { get => AllPlayersFinished; }
    public bool GameLooser { get => OnePlayerDead; set => _gameLooser.Value = value; }
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

        if (_gameFinishedUI != null)
            _gameFinishedUI?.SetActive(newValue);
        else
        {
            GlobalVariable.Instance.Round += 1;
            NetworkManager.Singleton.SceneManager.LoadScene(Constants.Rounds[GlobalVariable.Instance.Round], LoadSceneMode.Single);
        }
    }

    private void OnGameLooser(bool oldValue, bool newValue)
    {
        if (newValue)
            Debug.Log("===== GAME LOOSER =====");

        if (_gameLooserUI != null)
            _gameLooserUI?.SetActive(newValue);

        DeSpawnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeSpawnServerRpc()
    {
        foreach (ulong playerId in _playersPrefab.Keys)
        {
            DeSpanClientRpc(playerId);
        }

        // clean up game state
        NumberOfPlayersFinished = 0;
        NumberOfPlayersAlive = NumberOfPlayers;
        GameEnded = false;
        GameStarted = true;
        GamePaused = false;
        GameLooser = false;
    }

    [ClientRpc]
    private void DeSpanClientRpc(ulong clientId)
    {
        Vector3 spawnPoint = new Vector3(UnityEngine.Random.Range(_spawnStartX, _spawnEndX), UnityEngine.Random.Range(_spawnStartY, _spawnEndY), 0f);
        OnDeSpawnPlayerEvent?.Invoke(clientId, spawnPoint);
    }

    private void OnGameEnded(bool oldValue, bool newValue)
    {
        if (newValue)
            Debug.Log("===== GAME ENDED =====");

        if (_gameEndedUI != null)
            _gameEndedUI?.SetActive(newValue);
    }

    public void OnGameQuit()
    {
        Debug.Log("===== GAME QUIT =====");
        _isQuitting = true;
        OnLeaveGame?.Invoke();
        if (!IsServer && IsOwner) LeaveLobby();
        Destroy(gameObject);
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

    public void OnRemovePlayer(ulong clientId)
    {
        if (_isQuitting && IsServer) return;
        Debug.Log($"Player {clientId} is leaving the game");
        OnRemovePlayerServerRpc(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnRemovePlayerServerRpc(ulong clientId)
    {
        Debug.Log($"Server: Player {clientId} is leaving the game");
        if (m_PlayerState.ContainsKey(clientId))
        {
            m_PlayerState.Remove(clientId);
        }

        if (MatchmakingService._playersInLobby.ContainsKey(clientId))
        {
            MatchmakingService._playersInLobby.Remove(clientId);
        }

        _playersPrefab[clientId].NetworkObject.Despawn(true);
        _playersPrefab.Remove(clientId);

        NumberOfPlayers = m_PlayerState.Count;
        OnPlayerStateChangedServerRpc();
        OnRemovePlayerClientRpc(clientId);
        _PlayersTracking?.RemovePlayer(clientId);
        OnRemovePlayerEvent?.Invoke(clientId);
    }

    [ClientRpc]
    private void OnRemovePlayerClientRpc(ulong clientId)
    {
        if (IsServer) return;
        Debug.Log($"Client: Player {clientId} is leaving the game");
        if (m_PlayerState.ContainsKey(clientId))
        {
            m_PlayerState.Remove(clientId);
        }
        // NumberOfPlayers = m_PlayerState.Count;
        _PlayersTracking?.RemovePlayer(clientId);
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
        var spawnPoint = new Vector3(UnityEngine.Random.Range(_spawnStartX, _spawnEndX), UnityEngine.Random.Range(_spawnStartY, _spawnEndY), 0f);
        var spawn = Instantiate(GlobalVariable.Instance.PlayerPrefab, spawnPoint, Quaternion.identity);
        spawn.NetworkObject.SpawnWithOwnership(playerId);
        _playersPrefab.Add(playerId, spawn);
    }

    public override void OnDestroy()
    {
        if ((_isQuitting && (IsServer || IsClient)) || (!GameFinished && IsServer))
        {
            LeaveLobby();
        }
        OnGameDestroy?.Invoke();
        base.OnDestroy();
    }

    public void LeaveLobby()
    {
        //if (!IsOwner) return;
        LeaveLobbyClientRpc();
    }

    public async void OnLeaveLobby()
    {
        await MatchmakingService.LeaveLobby();
        if (NetworkManager.Singleton != null) NetworkManager.Singleton.Shutdown();
        if (!IsServer) MatchmakingService.ResetStatics();
    }

    [ClientRpc]
    private void LeaveLobbyClientRpc()
    {
        GlobalVariable.Instance.OnReload = true;
        string sceneName = GlobalVariable.Instance.GameMode == 1 ? Constants.LobbyScene : Constants.MainMenu;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        OnLeaveLobby();
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