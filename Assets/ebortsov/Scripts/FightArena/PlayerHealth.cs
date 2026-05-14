using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float returnToMenuDelay = 4f;
    [SerializeField] private GameObject explosionVfxPrefab;

    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    private bool isDead;

    public int CurrentHealth => currentHealth.Value;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }

        currentHealth.OnValueChanged += OnHealthChanged;

        if (IsOwner)
        {
            HealthUI healthUI = FindFirstObjectByType<HealthUI>();

            if (healthUI != null)
            {
                healthUI.SetHealth(currentHealth.Value);
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        currentHealth.OnValueChanged -= OnHealthChanged;
    }

    public void TakeDamage(int damage)
    {
        if (!IsServer || isDead)
            return;

        currentHealth.Value -= damage;

        if (currentHealth.Value <= 0)
        {
            currentHealth.Value = 0;
            isDead = true;
            
            Debug.Log("Player defeated: " + OwnerClientId);

            AnalyticsTracker.LogEvent("player_defeated");

            HandlePlayerDefeatedClientRpc(OwnerClientId);
        }
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        Debug.Log("Player " + OwnerClientId + " HP: " + newHealth);

        if (!IsOwner)
            return;

        HealthUI healthUI = FindFirstObjectByType<HealthUI>();

        if (healthUI != null)
        {
            healthUI.SetHealth(newHealth);
        }
    }

    [ClientRpc]
    private void HandlePlayerDefeatedClientRpc(ulong loserClientId)
    {
        bool localPlayerLost = NetworkManager.Singleton.LocalClientId == loserClientId;

        if (OwnerClientId == loserClientId)
        {
            SpawnExplosionEffect();
            DisableDefeatedUfo();
        }

        GameOverUI gameOverUI = FindFirstObjectByType<GameOverUI>();

        if (gameOverUI == null)
        {
            Debug.LogWarning("GameOverUI not found in scene.");
            return;
        }

        if (localPlayerLost)
        {
            gameOverUI.ShowLose();
        }
        else
        {
            gameOverUI.ShowWin();
        }

        StartCoroutine(ReturnToMainMenuAfterDelay());
    }

    private void SpawnExplosionEffect()
    {
        if (explosionVfxPrefab == null)
            return;

        GameObject explosion = Instantiate(
            explosionVfxPrefab,
            transform.position,
            Quaternion.identity
        );

        Destroy(explosion, 2f);
    }

    private void DisableDefeatedUfo()
    {
        PlayerUfoMovement movement = GetComponent<PlayerUfoMovement>();
        if (movement != null)
            movement.enabled = false;

        PlayerUfoShooting shooting = GetComponent<PlayerUfoShooting>();
        if (shooting != null)
            shooting.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }
    }

    private IEnumerator ReturnToMainMenuAfterDelay()
    {
        yield return new WaitForSeconds(returnToMenuDelay);

        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }

        SceneManager.LoadScene("MainMenuScene");
    }
}