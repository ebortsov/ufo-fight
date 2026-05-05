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

        CurrentHealth.Value -= damage;

        Debug.Log($"{gameObject.name} HP: {CurrentHealth.Value}");

        if (CurrentHealth.Value <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} destroyed!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDestroyed(this);
        }

        NetworkObject.Despawn();
    }
}