using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject createLobbyPanel;
    [SerializeField] private GameObject joinLobbyPanel;

    private void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        createLobbyPanel.SetActive(false);
        joinLobbyPanel.SetActive(false);
    }

    public void ShowCreateLobby()
    {
        mainMenuPanel.SetActive(false);
        createLobbyPanel.SetActive(true);
        joinLobbyPanel.SetActive(false);
    }

    public void ShowJoinLobby()
    {
        mainMenuPanel.SetActive(false);
        createLobbyPanel.SetActive(false);
        joinLobbyPanel.SetActive(true);
    }
}