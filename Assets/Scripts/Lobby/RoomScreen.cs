using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///     NetworkBehaviours cannot easily be parented, so the network logic will take place
///     on the network scene object "NetworkLobby"
/// </summary>
public class RoomScreen : MonoBehaviour {
    [SerializeField] private LobbyPlayerPanel _playerPanelPrefab;
    [SerializeField] private LobbyOrchestrator _lobbyOrchestrator;
    [SerializeField] private Transform _playerPanelParent;
    [SerializeField] private TMP_Text _waitingText;
    [SerializeField] private GameObject _startButton, _readyButton;
    [SerializeField] private float _roomRefreshRate = 2;

    [Header("List buttons for enable")]
    [SerializeField] private List<GameObject> _listButtons;
    
    [Header("List buttons for disable")]
    [SerializeField] private List<GameObject> _listButtonsDisable;

    private readonly List<LobbyPlayerPanel> _playerPanels = new();
    private bool _allReady;
    private bool _ready;
    private float _nextRefreshTime;
    private string _sceneName;

    public static event Action StartPressed;

    private void Awake()
    {
        foreach (var button in _listButtons)
        {
            button.SetActive(true);
        }
        
        foreach (var button in _listButtonsDisable)
        {
            button.SetActive(false);
        }
    }

    private void Update()
    {
        if (Time.time >= _nextRefreshTime) FetchRoom();
    }

    private void FetchRoom()
    {
        _nextRefreshTime = Time.time + _roomRefreshRate;
        _lobbyOrchestrator.UpdatePlayerData();
    }

    private void OnEnable() {
        foreach (Transform child in _playerPanelParent) Destroy(child.gameObject);
        _playerPanels.Clear();

        LobbyOrchestrator.LobbyPlayersUpdated += NetworkLobbyPlayersUpdated;
        MatchmakingService.CurrentLobbyRefreshed += OnCurrentLobbyRefreshed;
        _startButton.SetActive(false);
        _readyButton.SetActive(false);

        _ready = false;
    }

    private void OnDisable() {
        LobbyOrchestrator.LobbyPlayersUpdated -= NetworkLobbyPlayersUpdated;
        MatchmakingService.CurrentLobbyRefreshed -= OnCurrentLobbyRefreshed;
    }

    public static event Action LobbyLeft;

    public void OnLeaveLobby() {
        LobbyLeft?.Invoke();
    }

    private void NetworkLobbyPlayersUpdated(Dictionary<ulong, PlayerData> players) {
        var allActivePlayerIds = players.Keys;

        // Remove all inactive panels
        var toDestroy = _playerPanels.Where(p => !allActivePlayerIds.Contains(p.PlayerId)).ToList();
        foreach (var panel in toDestroy) {
            _playerPanels.Remove(panel);
            Destroy(panel.gameObject);
        }

        foreach (var player in players) {
            var currentPanel = _playerPanels.FirstOrDefault(p => p.PlayerId == player.Key);
            if (currentPanel != null) {
                currentPanel.Set(player.Value);
            }
            else {
                var panel = Instantiate(_playerPanelPrefab, _playerPanelParent);
                panel.Init(player.Key, player.Value);
                _playerPanels.Add(panel);
            }
        }

        _startButton.SetActive(NetworkManager.Singleton.IsHost && players.All(p => p.Value.ready));
        _readyButton.SetActive(!_ready);
    }

    private void OnCurrentLobbyRefreshed(Lobby lobby) {
        _waitingText.text = $"Waiting on players... {lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    public void OnReadyClicked() {
        _readyButton.SetActive(false);
        _ready = true;
    }

    public void OnStartClicked() {
        StartPressed?.Invoke();
    }

    public void OnBackClicked()
    {
        foreach (var button in _listButtons)
        {
            button.SetActive(false);
        }

        foreach (var button in _listButtonsDisable)
        {
            button.SetActive(true);
        }
    }
}