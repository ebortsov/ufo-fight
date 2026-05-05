using TMPro;
using UnityEngine;

public class LocalHealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI localHealthText;

    private UFO_Health localHealth;

    private void Update()
    {
        if (localHealth == null)
        {
            FindLocalPlayerHealth();
            return;
        }

        localHealthText.text = $"HP: {localHealth.CurrentHealth.Value}";
    }

    private void FindLocalPlayerHealth()
    {
        UFO_Health[] healthComponents = FindObjectsByType<UFO_Health>(FindObjectsSortMode.None);

        foreach (UFO_Health health in healthComponents)
        {
            if (health.IsOwner)
            {
                localHealth = health;
                break;
            }
        }
    }
}