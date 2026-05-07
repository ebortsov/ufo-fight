using Unity.Netcode;
using UnityEngine;

public class NetworkStartUI : MonoBehaviour
{
    private void OnGUI()
    {
        if (NetworkManager.Singleton == null)
        {
            GUI.Label(new Rect(10, 10, 300, 30), "NetworkManager.Singleton is NULL");
            return;
        }

        GUILayout.BeginArea(new Rect(10, 10, 200, 150));

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Start Host"))
            {
                NetworkManager.Singleton.StartHost();
            }

            if (GUILayout.Button("Start Client"))
            {
                NetworkManager.Singleton.StartClient();
            }

            if (GUILayout.Button("Start Server"))
            {
                NetworkManager.Singleton.StartServer();
            }
        }

        GUILayout.EndArea();
    }
}