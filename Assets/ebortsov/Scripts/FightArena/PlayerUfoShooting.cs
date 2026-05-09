using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUfoShooting : NetworkBehaviour
{
    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireCooldown = 0.25f;

    private float nextFireTime;

    public override void OnNetworkSpawn()
    {
        enabled = IsOwner;
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        Mouse mouse = Mouse.current;

        if (mouse == null)
            return;

        if (mouse.leftButton.wasPressedThisFrame && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireCooldown;
            ShootServerRpc();
        }
        else if (mouse.leftButton.wasPressedThisFrame)
        {
            Debug.Log($"Time left before next shot: {nextFireTime - Time.time:F2}s");
        }
    }

    [ServerRpc]
    private void ShootServerRpc(ServerRpcParams serverRpcParams = default)
    {
        GameObject projectileObject = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation * Quaternion.Euler(90f, 0f, 0f)
        );

        Projectile projectile = projectileObject.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.Initialize(serverRpcParams.Receive.SenderClientId);
        }

        NetworkObject networkObject = projectileObject.GetComponent<NetworkObject>();

        if (networkObject == null)
        {
            Debug.LogError("Projectile prefab is missing NetworkObject.");
            Destroy(projectileObject);
            return;
        }

        networkObject.Spawn();
    }
}