using Unity.Netcode;
using UnityEngine;

public class Projectile_Lifetime : NetworkBehaviour
{
    [SerializeField] private float lifetime = 3f;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Invoke(nameof(DespawnProjectile), lifetime);
        }
    }

    private void DespawnProjectile()
    {
        if (NetworkObject != null && NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn();
        }
    }
}