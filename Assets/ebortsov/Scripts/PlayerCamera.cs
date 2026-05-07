using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    private Camera cam;
    private AudioListener audioListener;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        audioListener = GetComponent<AudioListener>();

        cam.enabled = false;

        if (audioListener != null)
        {
            audioListener.enabled = false;
        }
    }

    public override void OnNetworkSpawn()
    {
        bool isLocalPlayer = IsOwner;

        cam.enabled = isLocalPlayer;

        if (audioListener != null)
        {
            audioListener.enabled = isLocalPlayer;
        }
    }
}