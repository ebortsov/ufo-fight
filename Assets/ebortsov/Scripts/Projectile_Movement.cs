using Unity.Netcode;
using UnityEngine;

public class Projectile_Movement : NetworkBehaviour
{
    private Vector3 velocity;

    public void Initialize(Vector3 startVelocity)
    {
        velocity = startVelocity;
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;

        transform.position += velocity * Time.fixedDeltaTime;
    }
}