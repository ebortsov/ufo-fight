using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class HostLobbyManager : MonoBehaviour
{
    [SerializeField] private LobbyUI lobbyUI;

    private Lobby currentLobby;
    private bool isStartingGame;

    private const string LobbyName = "UFO Fight Lobby";
    private const int MaxPlayers = 2;
    private const string ConnectionType = "dtls";

    private const string GameStartedKey = "GameStarted";
    private const string RelayJoinCodeKey = "RelayJoinCode";

    public async void CreateLobby()
    {
        lobbyUI.ShowLobbyScreen();
        lobbyUI.ClearJoinError();
        lobbyUI.SetGameCodeText("Creating lobby...");
        lobbyUI.SetPlayerCount(1, MaxPlayers);
        lobbyUI.SetStartButtonVisible(false);

        isStartingGame = false;

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            lobbyUI.SetGameCodeText("Not signed in yet. Try again.");
            return;
        }

        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = true,
                Data = new Dictionary<string, DataObject>
                {
                    {
                        GameStartedKey,
                        new DataObject(
                            DataObject.VisibilityOptions.Member,
                            "false"
                        )
                    }
                }
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync(
                LobbyName,
                MaxPlayers,
                options
            );

            lobbyUI.SetGameCodeText("Game Code:\n" + currentLobby.LobbyCode);
            lobbyUI.SetStartButtonVisible(true);
            lobbyUI.SetStartButtonInteractable(false);

            UpdateHostUI();

            StartLobbyHeartbeat();
            StartLobbyRefresh();

            Debug.Log("Host created lobby. Code: " + currentLobby.LobbyCode);
        }
        catch (LobbyServiceException e)
        {
            lobbyUI.SetGameCodeText("Failed to create lobby.");
            Debug.LogError(e);
        }
    }

    public async void StartGame()
    {
        if (currentLobby == null || isStartingGame)
            return;

        if (currentLobby.Players.Count < MaxPlayers)
        {
            lobbyUI.SetGameCodeText("Game Code:\n" + currentLobby.LobbyCode + "\n\nNeed 2 players to start.");
            return;
        }

        isStartingGame = true;
        lobbyUI.SetStartButtonInteractable(false);
        lobbyUI.SetGameCodeText("Game Code:\n" + currentLobby.LobbyCode + "\n\nStarting game...");

        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayers - 1);
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, ConnectionType));

            bool hostStarted = NetworkManager.Singleton.StartHost();

            if (!hostStarted)
            {
                lobbyUI.SetGameCodeText("Failed to start host.");
                isStartingGame = false;
                lobbyUI.SetStartButtonInteractable(true);
                return;
            }

            currentLobby = await LobbyService.Instance.UpdateLobbyAsync(
                currentLobby.Id,
                new UpdateLobbyOptions
                {
                    IsLocked = true,
                    Data = new Dictionary<string, DataObject>
                    {
                        {
                            RelayJoinCodeKey,
                            new DataObject(
                                DataObject.VisibilityOptions.Member,
                                relayJoinCode
                            )
                        },
                        {
                            GameStartedKey,
                            new DataObject(
                                DataObject.VisibilityOptions.Member,
                                "true"
                            )
                        }
                    }
                }
            );

            Debug.Log("Host started Netcode and shared Relay code.");
            StartCoroutine(LoadGameSceneWhenClientConnected());
        }
        catch (LobbyServiceException e)
        {
            lobbyUI.SetGameCodeText("Failed to update lobby.");
            isStartingGame = false;
            lobbyUI.SetStartButtonInteractable(true);
            Debug.LogError(e);
        }
        catch (RelayServiceException e)
        {
            lobbyUI.SetGameCodeText("Failed to create Relay.");
            isStartingGame = false;
            lobbyUI.SetStartButtonInteractable(true);
            Debug.LogError(e);
        }
    }

    private void StartLobbyHeartbeat()
    {
        CancelInvoke(nameof(SendHeartbeat));
        InvokeRepeating(nameof(SendHeartbeat), 15f, 15f);
    }

    private async void SendHeartbeat()
    {
        if (currentLobby == null)
            return;

        try
        {
            await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
    }

    private void StartLobbyRefresh()
    {
        CancelInvoke(nameof(RefreshLobby));
        InvokeRepeating(nameof(RefreshLobby), 2f, 2f);
    }

    private async void RefreshLobby()
    {
        if (currentLobby == null || isStartingGame)
            return;

        try
        {
            currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
            UpdateHostUI();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
    }

    private void UpdateHostUI()
    {
        if (currentLobby == null)
            return;

        int playerCount = currentLobby.Players.Count;

        lobbyUI.SetPlayerCount(playerCount, MaxPlayers);
        lobbyUI.SetStartButtonVisible(true);
        lobbyUI.SetStartButtonInteractable(playerCount >= MaxPlayers && !isStartingGame);
    }

    private IEnumerator LoadGameSceneWhenClientConnected()
    {
        lobbyUI.SetGameCodeText("Waiting for client connection...");

        float timeout = 10f;
        float timer = 0f;

        while (NetworkManager.Singleton.ConnectedClientsIds.Count < MaxPlayers && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count < MaxPlayers)
        {
            lobbyUI.SetGameCodeText("Client did not connect in time.");
            Debug.LogWarning("Timed out waiting for client Netcode connection.");
            yield break;
        }

        Debug.Log("Both players connected. Loading GameScene.");

        NetworkManager.Singleton.SceneManager.LoadScene(
            "CityFightScene",
            LoadSceneMode.Single
        );
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }
}