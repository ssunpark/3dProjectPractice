using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Slider healthSlider;
    public PlayerHealth playerHealth;

    private void Start()
    {
        healthSlider.minValue = 0;
        healthSlider.maxValue = playerHealth.stats.MaxHealth;
        playerHealth.OnHealthChanged.AddListener(UpdateHealthUI);
    }

    private void UpdateHealthUI(float currentHealth)
    {
        healthSlider.value = currentHealth;
    }
}
