using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TMP_Text healthText;

    private void Start()
    {
        SetHealth(100);
    }

    public void SetHealth(int health)
    {
        healthBarSlider.value = health;
        healthText.text = "HP: " + health;
    }
}