using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SerializeField] private TextMeshProUGUI gameOverText;

    private NetworkVariable<bool> gameOver = new NetworkVariable<bool>(false);

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (gameOver.Value && Keyboard.current.rKey.wasPressedThisFrame)
        {
            RequestRestartServerRpc();
        }
    }

    public void PlayerDestroyed(UFO_Health destroyedUfo)
    {
        if (!IsServer) return;

        if (gameOver.Value)
            return;

        gameOver.Value = true;

        EndGameClientRpc(destroyedUfo.OwnerClientId);
    }

    [ClientRpc]
    private void EndGameClientRpc(ulong loserClientId)
    {
        bool isLocalLoser = NetworkManager.Singleton.LocalClientId == loserClientId;

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = isLocalLoser ? "You Lost\nPress R to restart" : "You Won\nPress R to restart";
        }

        DisableControls();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestRestartServerRpc()
    {
        gameOver.Value = false;

        UFO_Health[] players = FindObjectsByType<UFO_Health>(FindObjectsSortMode.None);

        foreach (UFO_Health player in players)
        {
            player.ResetHealth();
        }

        RestartGameClientRpc();
    }

    [ClientRpc]
    private void RestartGameClientRpc()
    {
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }

        UFO_Movement[] movements = FindObjectsByType<UFO_Movement>(FindObjectsSortMode.None);
        UFO_Shooting[] shootings = FindObjectsByType<UFO_Shooting>(FindObjectsSortMode.None);

        foreach (var m in movements)
            m.enabled = true;

        foreach (var s in shootings)
            s.enabled = true;
    }
    
    private void DisableControls()
    {
        UFO_Movement[] movements = FindObjectsByType<UFO_Movement>(FindObjectsSortMode.None);
        UFO_Shooting[] shootings = FindObjectsByType<UFO_Shooting>(FindObjectsSortMode.None);

        foreach (var m in movements)
            m.enabled = false;

        foreach (var s in shootings)
            s.enabled = false;
    }
}