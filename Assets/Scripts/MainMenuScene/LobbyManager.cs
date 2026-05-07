using TMPro;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [Header("Create Lobby UI")]
    [SerializeField] private TMP_Text gameCodeText;

    [Header("Join Lobby UI")]
    [SerializeField] private TMP_InputField gameCodeInputField;
    [SerializeField] private TMP_Text joinErrorText;

    public void CreateLobby()
    {
        gameCodeText.text = "Creating lobby...";
        Debug.Log("Create Lobby button clicked");
    }

    public void JoinLobby()
    {
        string code = gameCodeInputField.text.Trim();

        if (string.IsNullOrEmpty(code))
        {
            joinErrorText.text = "Enter a game code.";
            return;
        }

        joinErrorText.text = "";
        Debug.Log("Join Lobby button clicked with code: " + code);
    }
}