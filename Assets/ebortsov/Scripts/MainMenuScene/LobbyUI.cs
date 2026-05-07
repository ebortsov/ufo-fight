using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [Header("Screen Navigation")]
    [SerializeField] private MainMenuUI mainMenuUI;

    [Header("Lobby UI")]
    [SerializeField] private TMP_Text gameCodeText;
    [SerializeField] private TMP_Text playerCountText;
    [SerializeField] private TMP_InputField gameCodeInputField;
    [SerializeField] private TMP_Text joinErrorText;
    [SerializeField] private Button startGameButton;

    public string EnteredGameCode => gameCodeInputField.text.Trim().ToUpper();

    public void ShowLobbyScreen()
    {
        mainMenuUI.ShowCreateLobby();
    }

    public void ShowMainMenu()
    {
        mainMenuUI.ShowMainMenu();
    }

    public void SetGameCodeText(string message)
    {
        gameCodeText.text = message;
    }

    public void SetPlayerCount(int currentPlayers, int maxPlayers)
    {
        playerCountText.text = "Players: " + currentPlayers + " / " + maxPlayers;
    }

    public void SetJoinError(string message)
    {
        joinErrorText.text = message;
    }

    public void ClearJoinError()
    {
        joinErrorText.text = "";
    }

    public void SetStartButtonVisible(bool visible)
    {
        startGameButton.gameObject.SetActive(visible);
    }

    public void SetStartButtonInteractable(bool interactable)
    {
        startGameButton.interactable = interactable;
    }
}