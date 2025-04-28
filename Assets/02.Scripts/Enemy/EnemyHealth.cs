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
        _enemy = GetComponent<Enemy>(); // ���� ������Ʈ�� �پ����� �Ŷ� ����
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
            _enemy.OnDeath(); // Enemy ��ũ��Ʈ�� �״� ���� ���� ���� ȣ��
        }
    }
}
