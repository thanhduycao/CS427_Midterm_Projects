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

    public override void OnNetworkSpawn()
    {
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        base.OnNetworkSpawn();
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
}