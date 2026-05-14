using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUfoShooting : NetworkBehaviour
{
    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject localProjectileVisualPrefab;
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

            Vector3 firePosition = firePoint.position;
            Quaternion fireRotation = firePoint.rotation * Quaternion.Euler(90f, 0f, 0f);

            SpawnLocalProjectileVisual(firePosition, fireRotation);

            ShootServerRpc(firePosition, fireRotation);
        }
    }

    private void SpawnLocalProjectileVisual(Vector3 position, Quaternion rotation)
    {
        if (localProjectileVisualPrefab == null)
            return;

        GameObject localProjectile = Instantiate(
            localProjectileVisualPrefab,
            position,
            rotation
        );

        LocalProjectileVisual visual = localProjectile.GetComponent<LocalProjectileVisual>();

        if (visual != null)
        {
            visual.Initialize(transform.root);
        }
    }

    [ServerRpc]
    private void ShootServerRpc(Vector3 firePosition, Quaternion fireRotation, ServerRpcParams serverRpcParams = default)
    {
        GameObject projectileObject = Instantiate(
            projectilePrefab,
            firePosition,
            fireRotation
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

        networkObject.SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
        
        AnalyticsTracker.LogEvent("shot_fired");
    }
}