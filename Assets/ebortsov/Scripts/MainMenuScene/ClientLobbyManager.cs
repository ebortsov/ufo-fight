using UnityEngine;

using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class ClientLobbyManager : MonoBehaviour
{
    [SerializeField] private LobbyUI lobbyUI;

    private Lobby currentLobby;
    private bool isStartingGame;

    private const int MaxPlayers = 2;
    private const string ConnectionType = "dtls";

    private const string GameStartedKey = "GameStarted";
    private const string RelayJoinCodeKey = "RelayJoinCode";

    public async void JoinLobby()
    {
        string code = lobbyUI.EnteredGameCode;

        if (string.IsNullOrEmpty(code))
        {
            lobbyUI.SetJoinError("Enter a game code.");
            return;
        }

        lobbyUI.SetJoinError("Joining lobby...");

        try
        {
            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);

            isStartingGame = false;

            lobbyUI.ShowLobbyScreen();
            lobbyUI.ClearJoinError();
            lobbyUI.SetGameCodeText("Game Code:\n" + currentLobby.LobbyCode);
            lobbyUI.SetStartButtonVisible(false);

            UpdateClientUI();
            StartLobbyRefresh();

            Debug.Log("Client joined lobby waiting room. Code: " + currentLobby.LobbyCode);
        }
        catch (LobbyServiceException e)
        {
            lobbyUI.SetJoinError("Invalid code or game already started.");
            Debug.LogError(e);
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

            UpdateClientUI();
            CheckIfGameStarted();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
    }

    private void UpdateClientUI()
    {
        if (currentLobby == null)
            return;

        lobbyUI.SetPlayerCount(currentLobby.Players.Count, MaxPlayers);
        lobbyUI.SetStartButtonVisible(false);
    }

    private async void CheckIfGameStarted()
    {
        if (currentLobby == null || isStartingGame)
            return;

        if (currentLobby.Data == null)
            return;

        if (!currentLobby.Data.ContainsKey(GameStartedKey))
            return;

        bool gameStarted = currentLobby.Data[GameStartedKey].Value == "true";

        if (!gameStarted)
            return;

        if (!currentLobby.Data.ContainsKey(RelayJoinCodeKey))
        {
            lobbyUI.SetGameCodeText("Game started, but Relay code is missing.");
            return;
        }

        isStartingGame = true;

        string relayJoinCode = currentLobby.Data[RelayJoinCodeKey].Value;

        lobbyUI.SetGameCodeText("Game starting...");
        lobbyUI.ClearJoinError();

        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            Debug.Log("Client Relay Region: " + joinAllocation.Region);

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(joinAllocation, ConnectionType));

            bool clientStarted = NetworkManager.Singleton.StartClient();

            if (!clientStarted)
            {
                lobbyUI.SetGameCodeText("Failed to start client.");
                isStartingGame = false;
                return;
            }

            Debug.Log("Client started Netcode through Relay.");
        }
        catch (RelayServiceException e)
        {
            lobbyUI.SetGameCodeText("Failed to join Relay.");
            isStartingGame = false;
            Debug.LogError(e);
        }
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }
}