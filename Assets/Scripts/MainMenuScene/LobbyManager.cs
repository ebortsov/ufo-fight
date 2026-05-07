using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class LobbyManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private MainMenuUI mainMenuUI;
    [SerializeField] private TMP_Text gameCodeText;
    [SerializeField] private TMP_InputField gameCodeInputField;
    [SerializeField] private TMP_Text joinErrorText;
    [SerializeField] private Button startGameButton;

    private Lobby currentLobby;
    private bool isHost;

    private const string LobbyName = "UFO Fight Lobby";
    private const int MaxPlayers = 2;

    public async void CreateLobby()
    {
        gameCodeText.text = "Creating lobby...";
        isHost = true;
        startGameButton.gameObject.SetActive(false);

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

            startGameButton.gameObject.SetActive(true);

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

            isHost = false;
            startGameButton.gameObject.SetActive(false);

            gameCodeText.text = "Game Code:\n" + currentLobby.LobbyCode;
            joinErrorText.text = "";

            mainMenuUI.ShowCreateLobby();

            Debug.Log("Joined lobby. Code: " + currentLobby.LobbyCode);
        }
        catch (LobbyServiceException e)
        {
            joinErrorText.text = "Invalid game code.";
            Debug.LogError(e);
        }
    }

    public void StartGame()
    {
        if (!isHost)
            return;

        Debug.Log("Start Game clicked by host.");
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
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
    }
}