using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 60f;
    [SerializeField] private float lifetime = 2f;

    [Header("Damage")]
    [SerializeField] private int damage = 25;

    [Header("Expansion")]
    [SerializeField] private float radiusGrowthPerSecond = 0.6f;

    private readonly NetworkVariable<double> spawnServerTime = new NetworkVariable<double>();

    private float lifeTimer;
    private ulong shooterClientId;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void Initialize(ulong shooterId)
    {
        shooterClientId = shooterId;

        if (NetworkManager.Singleton != null)
        {
            spawnServerTime.Value = NetworkManager.Singleton.ServerTime.Time;
        }
    }

    public override void OnNetworkSpawn()
    {
        originalScale = transform.localScale;

        double elapsedTime = 0;

        if (NetworkManager.Singleton != null)
        {
            elapsedTime = NetworkManager.Singleton.ServerTime.Time - spawnServerTime.Value;
        }

        float elapsed = Mathf.Max(0f, (float)elapsedTime);

        // If this projectile arrived late on a client, move it forward immediately.
        transform.position += transform.up * speed * elapsed;

        lifeTimer = elapsed;
        UpdateExpansion();

        if (NetworkManager.Singleton != null &&
            OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }
        }
    }
    private void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;

        lifeTimer += Time.deltaTime;

        UpdateExpansion();

        if (!IsServer)
            return;

        if (lifeTimer >= lifetime)
        {
            if (NetworkObject.IsSpawned)
            {
                NetworkObject.Despawn();
            }
        }
    }

    private void UpdateExpansion()
    {
        float radiusMultiplier = 1f + lifeTimer * radiusGrowthPerSecond;

        // Cylinder length stays the same on local Y.
        // Radius grows on local X and Z.
        transform.localScale = new Vector3(
            originalScale.x * radiusMultiplier,
            originalScale.y,
            originalScale.z * radiusMultiplier
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
            return;

        if (other.GetComponentInParent<LocalProjectileVisual>() != null)
            return;

        PlayerHealth targetHealth = other.GetComponentInParent<PlayerHealth>();

        if (targetHealth != null)
        {
            if (targetHealth.OwnerClientId == shooterClientId)
                return;

            targetHealth.TakeDamage(damage);
            AnalyticsTracker.LogEvent("player_hit");

            if (NetworkObject.IsSpawned)
            {
                NetworkObject.Despawn();
            }

            return;
        }

        if (NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn();
        }
    }
}