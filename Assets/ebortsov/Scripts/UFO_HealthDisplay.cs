using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UFO_HealthDisplay : MonoBehaviour
{
    [SerializeField] private UFO_Health health;
    [SerializeField] private TextMeshProUGUI healthText;

    private Canvas canvas;
    private NetworkObject parentNetworkObject;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        parentNetworkObject = GetComponentInParent<NetworkObject>();
    }

    private void Update()
    {
        if (parentNetworkObject != null && parentNetworkObject.IsOwner)
        {
            canvas.enabled = false;
            return;
        }

        canvas.enabled = true;

        if (health != null && healthText != null)
        {
            healthText.text = $"HP: {health.CurrentHealth.Value}";
        }
    }
}