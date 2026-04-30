using UnityEngine;
using UnityEngine.InputSystem;

public class UFO_Shooting : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float projectileSpeed = 25f;
    [SerializeField] private float shootCooldown = 1.5f;

    private float nextShootTime = 0f;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextShootTime)
        {
            Shoot();
            nextShootTime = Time.time + shootCooldown;
        }
    }

    private void Shoot()
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            shootPoint.position,
            shootPoint.rotation
        );

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        projectileRb.linearVelocity = shootPoint.forward * projectileSpeed;

        Physics.IgnoreCollision(
            projectile.GetComponent<Collider>(),
            GetComponent<Collider>()
        );
    }
}