using TMPro;
using Unity.Netcode;
using UnityEngine;

public class NetworkPingUI : MonoBehaviour
{
    [SerializeField] private TMP_Text pingText;
    [SerializeField] private float updateInterval = 0.5f;

    private float timer;

    private void Update()
    {
        timer += Time.unscaledDeltaTime;

        if (timer < updateInterval)
            return;

        timer = 0f;
        UpdatePingText();
    }

    private void UpdatePingText()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
        {
            pingText.text = "Ping: -- ms";
            return;
        }

        ulong targetClientId;

        if (NetworkManager.Singleton.IsHost)
        {
            // In a 2-player game, show host's RTT to the connected client.
            if (NetworkManager.Singleton.ConnectedClientsIds.Count < 2)
            {
                pingText.text = "Ping: -- ms";
                return;
            }

            targetClientId = NetworkManager.Singleton.ConnectedClientsIds[1];
        }
        else
        {
            // For client, server/host is always client id 0.
            targetClientId = NetworkManager.ServerClientId;
        }

        ulong rtt = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(targetClientId);

        pingText.text = "Ping: " + rtt + " ms";
    }
}