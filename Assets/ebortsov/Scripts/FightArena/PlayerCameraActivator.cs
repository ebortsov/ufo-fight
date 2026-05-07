using Unity.Netcode;
using UnityEngine;

public class PlayerCameraActivator : NetworkBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioListener audioListener;

    public override void OnNetworkSpawn()
    {
        bool shouldEnable = IsOwner;

        if (playerCamera != null)
        {
            playerCamera.enabled = shouldEnable;
        }

        if (audioListener != null)
        {
            audioListener.enabled = shouldEnable;
        }

        if (IsOwner)
        {
            Camera sceneCamera = Camera.main;

            if (sceneCamera != null && sceneCamera != playerCamera)
            {
                sceneCamera.gameObject.SetActive(false);
            }
        }
    }
}