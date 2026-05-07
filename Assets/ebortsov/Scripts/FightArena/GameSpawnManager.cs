using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameSpawnManager : MonoBehaviour
{
    [Header("Player Prefab")]
    [SerializeField] private GameObject playerUfoPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        StartCoroutine(SpawnPlayers());
    }

    private IEnumerator SpawnPlayers()
    {
        yield return new WaitForSeconds(0.5f);

        List<ulong> clientIds = NetworkManager.Singleton.ConnectedClientsIds
            .OrderBy(id => id)
            .ToList();

        Transform[] spawnPoints =
        {
            spawnPoint1,
            spawnPoint2
        };

        for (int i = 0; i < clientIds.Count && i < spawnPoints.Length; i++)
        {
            ulong clientId = clientIds[i];

            if (NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject != null)
                continue;

            GameObject playerObject = Instantiate(
                playerUfoPrefab,
                spawnPoints[i].position,
                spawnPoints[i].rotation
            );

            NetworkObject networkObject = playerObject.GetComponent<NetworkObject>();

            if (networkObject == null)
            {
                Debug.LogError("Player_UFO prefab is missing NetworkObject.");
                Destroy(playerObject);
                continue;
            }

            networkObject.SpawnAsPlayerObject(clientId, true);

            Debug.Log("Spawned UFO for client: " + clientId);
        }
    }
}