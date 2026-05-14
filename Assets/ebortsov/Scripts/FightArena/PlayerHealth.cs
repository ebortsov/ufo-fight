using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float returnToMenuDelay = 4f;

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

            ShowGameOverClientRpc(OwnerClientId);
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
    private void ShowGameOverClientRpc(ulong loserClientId)
    {
        GameOverUI gameOverUI = FindFirstObjectByType<GameOverUI>();

        if (gameOverUI == null)
        {
            Debug.LogWarning("GameOverUI not found in scene.");
            return;
        }

        if (NetworkManager.Singleton.LocalClientId == loserClientId)
        {
            gameOverUI.ShowLose();
        }
        else
        {
            gameOverUI.ShowWin();
        }

        StartCoroutine(ReturnToMainMenuAfterDelay());
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