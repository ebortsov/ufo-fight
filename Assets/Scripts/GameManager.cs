using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool gameOver = false;

    private void Awake()
    {
        Instance = this;
        Debug.Log("GameManager initialized");
    }

    public void PlayerDestroyed(UFO_Health destroyedUfo)
    {
        if (gameOver)
            return;

        gameOver = true;

        Debug.Log($"{destroyedUfo.gameObject.name} was destroyed. Game over!");

        DisableAllPlayerControls();
    }

    private void DisableAllPlayerControls()
    {
        UFO_Movement[] movementScripts = FindObjectsByType<UFO_Movement>(FindObjectsSortMode.None);
        UFO_Shooting[] shootingScripts = FindObjectsByType<UFO_Shooting>(FindObjectsSortMode.None);

        foreach (UFO_Movement movement in movementScripts)
        {
            movement.enabled = false;
        }

        foreach (UFO_Shooting shooting in shootingScripts)
        {
            shooting.enabled = false;
        }
    }
}