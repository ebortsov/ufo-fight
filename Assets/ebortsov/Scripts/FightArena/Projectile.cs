using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private float speed = 60f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int damage = 25;

    private float lifeTimer;
    private ulong shooterClientId;

    public void Initialize(ulong shooterId)
    {
        shooterClientId = shooterId;
    }

    private void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;

        if (!IsServer)
            return;

        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifetime)
        {
            NetworkObject.Despawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
            return;

        PlayerHealth targetHealth = other.GetComponentInParent<PlayerHealth>();

        if (targetHealth != null)
        {
            if (targetHealth.OwnerClientId == shooterClientId)
                return;

            targetHealth.TakeDamage(damage);
            NetworkObject.Despawn();
            return;
        }

        NetworkObject.Despawn();
    }
}