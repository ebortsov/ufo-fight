using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        cam.enabled = IsOwner;
    }
}