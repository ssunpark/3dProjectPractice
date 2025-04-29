using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int MaxHealth = 100;
    public int CurrentHealth { get; private set; }
    public Slider HealthSlider;

    private void Awake()
    {
        CurrentHealth = MaxHealth;

        if (HealthSlider != null)
        {
            HealthSlider.maxValue = MaxHealth;
            HealthSlider.value = MaxHealth;
        }
    }

    public void ApplyDamage(int amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        if (HealthSlider != null)
        {
            HealthSlider.value = CurrentHealth;
        }
    }
}
