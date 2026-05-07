using Unity.Netcode;
using UnityEngine;

public class UFO_Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 5;

    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            CurrentHealth.Value = maxHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        if (CurrentHealth.Value <= 0)
            return;

        CurrentHealth.Value -= damage;

        if (CurrentHealth.Value <= 0)
        {
            CurrentHealth.Value = 0;
            Die();
        }
    }

    public void ResetHealth()
    {
        if (!IsServer) return;

        CurrentHealth.Value = maxHealth;
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} destroyed!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDestroyed(this);
        }

        // Do NOT despawn player here.
        // The GameManager will freeze the game for everyone.
    }
}