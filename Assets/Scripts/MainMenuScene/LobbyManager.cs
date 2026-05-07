using TMPro;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;

public class LobbyManager : MonoBehaviour
{
    [Header("Create Lobby UI")]
    [SerializeField] private TMP_Text gameCodeText;

    [Header("Join Lobby UI")]
    [SerializeField] private TMP_InputField gameCodeInputField;
    [SerializeField] private TMP_Text joinErrorText;

    private Lobby currentLobby;

    private const string LobbyName = "UFO Fight Lobby";
    private const int MaxPlayers = 2;

    public async void CreateLobby()
    {
        gameCodeText.text = "Creating lobby...";

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            gameCodeText.text = "Not signed in yet. Try again.";
            return;
        }

        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = true
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync(
                LobbyName,
                MaxPlayers,
                options
            );

            gameCodeText.text = "Game Code:\n" + currentLobby.LobbyCode;

            Debug.Log("Created lobby. Code: " + currentLobby.LobbyCode);

            StartLobbyHeartbeat();
        }
        catch (LobbyServiceException e)
        {
            gameCodeText.text = "Failed to create lobby.";
            Debug.LogError(e);
        }
    }

    public async void JoinLobby()
    {
        string code = gameCodeInputField.text.Trim().ToUpper();

        if (string.IsNullOrEmpty(code))
        {
            joinErrorText.text = "Enter a game code.";
            return;
        }

        joinErrorText.text = "Joining lobby...";

        try
        {
            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);

            joinErrorText.text = "Joined lobby!";
            Debug.Log("Joined lobby. Code: " + currentLobby.LobbyCode);
        }
        catch (LobbyServiceException e)
        {
            joinErrorText.text = "Invalid game code.";
            Debug.LogError(e);
        }
    }

    private void StartLobbyHeartbeat()
    {
        InvokeRepeating(nameof(SendHeartbeat), 15f, 15f);
    }

    private async void SendHeartbeat()
    {
        if (currentLobby == null)
            return;

        try
        {
            await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            Debug.Log("Lobby heartbeat sent.");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
    }
}