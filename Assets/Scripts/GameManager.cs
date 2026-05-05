using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    private NetworkVariable<bool> gameOver = new NetworkVariable<bool>(false);

    private void Awake()
    {
        Instance = this;
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
        Debug.Log($"Game Over! Player {loserClientId} lost.");

        DisableControls();
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