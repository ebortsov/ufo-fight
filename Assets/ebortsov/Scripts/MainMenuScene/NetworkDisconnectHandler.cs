using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkDisconnectHandler : MonoBehaviour
{
    private bool isReturningToMenu;

    private void Start()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogWarning("NetworkDisconnectHandler: No NetworkManager found.");
            return;
        }

        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void HandleClientDisconnect(ulong disconnectedClientId)
    {
        if (isReturningToMenu)
            return;

        NetworkManager networkManager = NetworkManager.Singleton;

        if (networkManager == null)
            return;

        // If we are host, ignore normal client disconnects.
        if (networkManager.IsHost)
            return;

        // If we are a client and the server/host disconnects, return to menu.
        if (networkManager.IsClient)
        {
            Debug.Log("Host disconnected. Returning to main menu.");
            StartCoroutine(ReturnToMainMenu());
        }
    }

    private IEnumerator ReturnToMainMenu()
    {
        isReturningToMenu = true;

        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }

        yield return new WaitForSeconds(0.3f);

        SceneManager.LoadScene("MainMenuScene");
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
        }
    }
}