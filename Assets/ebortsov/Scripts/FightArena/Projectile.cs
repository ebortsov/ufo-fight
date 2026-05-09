using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private float speed = 45f;
    [SerializeField] private float lifetime = 2f;

    private float lifeTimer;

    private void Update()
    {
        if (!IsServer)
            return;

        transform.position += transform.up * speed * Time.deltaTime;

        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifetime)
        {
            NetworkObject.Despawn();
        }
    }
}