using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class UFO_Shooting : NetworkBehaviour
{
    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float projectileSpeed = 25f;
    [SerializeField] private float shootCooldown = 1.5f;

    private float nextShootTime = 0f;

    private void Update()
    {
        if (!IsOwner) return;

        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextShootTime)
        {
            ShootServerRpc(shootPoint.position, shootPoint.rotation);
            nextShootTime = Time.time + shootCooldown;
        }
    }

    [ServerRpc]
    private void ShootServerRpc(Vector3 position, Quaternion rotation)
    {
        GameObject projectile = Instantiate(projectilePrefab, position, rotation);

        NetworkObject networkObject = projectile.GetComponent<NetworkObject>();
        networkObject.Spawn();

        Vector3 velocity = rotation * Vector3.forward * projectileSpeed;

        projectile.GetComponent<Projectile_Movement>().Initialize(velocity);

        Physics.IgnoreCollision(
            projectile.GetComponent<Collider>(),
            GetComponent<Collider>()
        );
    }
}