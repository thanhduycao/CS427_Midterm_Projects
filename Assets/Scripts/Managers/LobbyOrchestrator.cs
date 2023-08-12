using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable CS4014

public class LobbyOrchestrator : NetworkBehaviour
{
    [SerializeField] private MainLobbyScreen _mainLobbyScreen;
    [SerializeField] private CreateLobbyScreen _createScreen;
    [SerializeField] private RoomScreen _roomScreen;
    [SerializeField] private PlayerSetting _playerSetting;

    private LobbyData _lobby;

    private void Start()
    {
        _mainLobbyScreen.gameObject.SetActive(false);
        _createScreen.gameObject.SetActive(false);
        _roomScreen.gameObject.SetActive(false);
        _playerSetting.gameObject.SetActive(true);

        CreateLobbyScreen.LobbyCreated += CreateLobby;
        LobbyRoomPanel.LobbySelected += OnLobbySelected;
        RoomScreen.LobbyLeft += OnLobbyLeft;
        RoomScreen.StartPressed += OnGameStart;
        RoomScreen.BackPressed += OnBackClick;

        NetworkObject.DestroyWithScene = true;

        CurrenPlayerData.Instance.Id = NetworkManager.Singleton.LocalClientId;
    }

    #region Main Lobby

    private async void OnLobbySelected(Lobby lobby)
    {
        // using (new Load("Joining Lobby...")) {
        try
        {
            await MatchmakingService.JoinLobbyWithAllocation(lobby.Id);

            _mainLobbyScreen.gameObject.SetActive(false);
            _roomScreen.gameObject.SetActive(true);

            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            CanvasUtilities.Instance.ShowError("Failed joining lobby");
        }
        // }
    }

    #endregion

    #region Create

    private async void CreateLobby(LobbyData data)
    {
        // using (new Load("Creating Lobby...")) {
        try
        {
            await MatchmakingService.CreateLobbyWithAllocation(data);

            _createScreen.gameObject.SetActive(false);
            _roomScreen.gameObject.SetActive(true);

            // store the lobby data
            _lobby = data;

            // Starting the host immediately will keep the relay server alive
            NetworkManager.Singleton.StartHost();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            CanvasUtilities.Instance.ShowError("Failed creating lobby");
        }
        //}
    }

    #endregion

    #region Room

    public static event Action<Dictionary<ulong, PlayerData>> LobbyPlayersUpdated;
    private float _nextLobbyUpdate;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            MatchmakingService._playersInLobby.Add(NetworkManager.Singleton.LocalClientId, new PlayerData(NetworkManager.Singleton.LocalClientId));
            UpdateInterface();
        }

        // Client uses this in case host destroys the lobby
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
    }

    private void OnClientConnectedCallback(ulong playerId)
    {
        if (!IsServer) return;

        // Add locally
        if (!MatchmakingService._playersInLobby.ContainsKey(playerId)) MatchmakingService._playersInLobby.Add(playerId, new PlayerData(NetworkManager.Singleton.LocalClientId));

        PropagateToClients();
        UpdateInterface();
    }

    private void PropagateToClients()
    {
        foreach (var player in MatchmakingService._playersInLobby) UpdatePlayerClientRpc(player.Key, player.Value);
    }

    [ClientRpc]
    private void UpdatePlayerClientRpc(ulong clientId, PlayerData _playerValue)
    {
        if (IsServer) return;

        if (!MatchmakingService._playersInLobby.ContainsKey(clientId)) MatchmakingService._playersInLobby.Add(clientId, new PlayerData(NetworkManager.Singleton.LocalClientId));
        else MatchmakingService._playersInLobby[clientId] = _playerValue;
        UpdateInterface();
    }

    private void OnClientDisconnectCallback(ulong playerId)
    {
        if (IsServer)
        {
            // Handle locally
            if (MatchmakingService._playersInLobby.ContainsKey(playerId)) MatchmakingService._playersInLobby.Remove(playerId);

            // Propagate all clients
            RemovePlayerClientRpc(playerId);
            UpdateInterface();
        }
        else
        {
            // This happens when the host disconnects the lobby
            _roomScreen.gameObject.SetActive(false);
            _mainLobbyScreen.gameObject.SetActive(true);
            OnLobbyLeft();
        }
    }

    [ClientRpc]
    private void RemovePlayerClientRpc(ulong clientId)
    {
        if (IsServer) return;

        if (MatchmakingService._playersInLobby.ContainsKey(clientId)) MatchmakingService._playersInLobby.Remove(clientId);
        UpdateInterface();
    }

    public void OnReadyClicked()
    {
        CurrenPlayerData.Instance.Ready = true;
    }

    public void UpdatePlayer(PlayerData playerData)
    {
        UpdatePlayerServerRpc(NetworkManager.Singleton.LocalClientId, playerData);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerServerRpc(ulong playerId, PlayerData playerData)
    {
        if (MatchmakingService._playersInLobby.ContainsKey(playerId)) MatchmakingService._playersInLobby[playerId] = playerData;
        else MatchmakingService._playersInLobby.Add(playerId, playerData);

        PropagateToClients();
        UpdateInterface();
    }

    private void UpdateInterface()
    {
        LobbyPlayersUpdated?.Invoke(MatchmakingService._playersInLobby);
    }

    private async void OnLobbyLeft()
    {
        if (IsServer) OnLobbyLeftClientRpc();
        // using (new Load("Leaving Lobby...")) {
        MatchmakingService._playersInLobby.Clear();
        NetworkManager.Singleton.Shutdown();
        await MatchmakingService.LeaveLobby();
        MatchmakingService.ResetStatics();
        // }
    }

    public void OnQuitClick()
    { 
        Application.Quit();
    }

    public void OnBackClick()
    {
        OnLobbyLeft();
    }

    [ClientRpc]
    private void OnLobbyLeftClientRpc()
    {
        if (IsServer) return;
        Debug.Log("=== Left Lobby ===");
        _createScreen.OnBackButton();
    }

    private void OnApplicationQuit()
    {
        OnLobbyLeft();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        CreateLobbyScreen.LobbyCreated -= CreateLobby;
        LobbyRoomPanel.LobbySelected -= OnLobbySelected;
        RoomScreen.LobbyLeft -= OnLobbyLeft;
        RoomScreen.StartPressed -= OnGameStart;

        // We only care about this during lobby
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        }
    }

    private async void OnGameStart()
    {
        // using (new Load("Starting the game...")) {
        // await MatchmakingService.LockLobby();
        NetworkManager.Singleton.SceneManager.LoadScene(Constants.Rounds[_lobby.Round], LoadSceneMode.Single);
        GlobalVariable.Instance.Round = _lobby.Round;
        // }
    }

    #endregion
}
