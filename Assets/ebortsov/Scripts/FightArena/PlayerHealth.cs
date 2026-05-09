using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    public int CurrentHealth => currentHealth.Value;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }

        currentHealth.OnValueChanged += OnHealthChanged;
    }

    public override void OnNetworkDespawn()
    {
        currentHealth.OnValueChanged -= OnHealthChanged;
    }

    public void TakeDamage(int damage)
    {
        if (!IsServer)
            return;

        currentHealth.Value -= damage;

        if (currentHealth.Value <= 0)
        {
            currentHealth.Value = 0;
            Debug.Log("Player destroyed: " + OwnerClientId);

            NetworkObject.Despawn();
        }
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        Debug.Log("Player " + OwnerClientId + " HP: " + newHealth);
    }
}