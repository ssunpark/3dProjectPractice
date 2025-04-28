using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int MaxHealth = 100;
    public int CurrentHealth { get; private set; }

    public Slider HealthSlider;
    private Enemy _enemy;

    private void Start()
    {
        _enemy = GetComponent<Enemy>(); // 같은 오브젝트에 붙어있을 거라 가정
        CurrentHealth = MaxHealth;

        if (HealthSlider != null)
        {
            HealthSlider.maxValue = MaxHealth;
            HealthSlider.value = MaxHealth;
        }
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        if (HealthSlider != null)
        {
            HealthSlider.value = CurrentHealth;
        }

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_enemy != null)
        {
            _enemy.OnDeath(); // Enemy 스크립트에 죽는 로직 따로 만들어서 호출
        }
    }
}
