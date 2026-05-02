using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private UFO_Health playerHealth;
    [SerializeField] private UFO_Health enemyHealth;

    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private TextMeshProUGUI enemyHealthText;

    private void Update()
    {
        playerHealthText.text = $"Player HP: {playerHealth.CurrentHealth}";
        enemyHealthText.text = $"Enemy HP: {enemyHealth.CurrentHealth}";
    }
}