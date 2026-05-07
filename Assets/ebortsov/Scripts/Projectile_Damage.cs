using Unity.Netcode;
using UnityEngine;

public class Projectile_Damage : NetworkBehaviour
{
    [SerializeField] private int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        UFO_Health health = other.GetComponent<UFO_Health>();

        if (health != null)
        {
            health.TakeDamage(damage);
        }

        NetworkObject networkObject = GetComponent<NetworkObject>();

        if (networkObject != null && networkObject.IsSpawned)
        {
            networkObject.Despawn();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}