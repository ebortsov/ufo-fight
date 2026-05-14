using UnityEngine;

public class LocalProjectileVisual : MonoBehaviour
{
    [SerializeField] private float speed = 90f;
    [SerializeField] private float lifetime = 5f;

    [Header("Expansion")]
    [SerializeField] private float radiusGrowthPerSecond = 2f;

    private float lifeTimer;
    private Vector3 originalScale;
    private Transform ownerRoot;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void Initialize(Transform owner)
    {
        ownerRoot = owner;
    }

    private void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;

        lifeTimer += Time.deltaTime;

        UpdateExpansion();

        if (lifeTimer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ownerRoot != null && other.transform.root == ownerRoot)
            return;

        if (other.GetComponentInParent<Projectile>() != null)
            return;

        Destroy(gameObject);
    }

    private void UpdateExpansion()
    {
        float radiusMultiplier = 1f + lifeTimer * radiusGrowthPerSecond;

        transform.localScale = new Vector3(
            originalScale.x * radiusMultiplier,
            originalScale.y,
            originalScale.z * radiusMultiplier
        );
    }
}