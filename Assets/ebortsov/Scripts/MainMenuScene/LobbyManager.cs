using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

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
    private string relayJoinCode;

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
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayers - 1);
            relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = true,
                Data = new Dictionary<string, DataObject>
                {
                    {
                        "RelayJoinCode",
                        new DataObject(
                            DataObject.VisibilityOptions.Member,
                            relayJoinCode
                        )
                    }
                }
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

        try
        {
            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);

            isHost = false;
            startGameButton.gameObject.SetActive(false);

            gameCodeText.text = "Game Code:\n" + currentLobby.LobbyCode;
            joinErrorText.text = "";

            mainMenuUI.ShowCreateLobby();

            if (currentLobby.Data != null && currentLobby.Data.ContainsKey("RelayJoinCode"))
            {
                relayJoinCode = currentLobby.Data["RelayJoinCode"].Value;
                Debug.Log("Received Relay Join Code: " + relayJoinCode);
            }
            else
            {
                joinErrorText.text = "Lobby has no Relay code.";
                return;
            }
        }
        catch (LobbyServiceException e)
        {
            joinErrorText.text = "Invalid game code or game already started.";
            Debug.LogError(e);
        }
        catch (Exception e)
        {
            joinErrorText.text = "Something went wrong.";
            Debug.LogError(e);
        }
    }

    public async void StartGame()
    {
        if (!isHost || currentLobby == null)
            return;

        startGameButton.interactable = false;
        gameCodeText.text = "Game Code:\n" + currentLobby.LobbyCode + "\n\nStarting game...";

        try
        {
            currentLobby = await LobbyService.Instance.UpdateLobbyAsync(
                currentLobby.Id,
                new UpdateLobbyOptions
                {
                    IsLocked = true
                }
            );

            Debug.Log("Lobby locked. New players cannot join now.");
        }
        catch (LobbyServiceException e)
        {
            gameCodeText.text = "Failed to start game.";
            startGameButton.interactable = true;
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
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
    }
}