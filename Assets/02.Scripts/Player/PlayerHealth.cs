using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public PlayerStasSO stats;
    public PlayerMove _playerMove;
    public UnityEvent<float> OnHealthChanged;

    // �����̴��� �ݿ��Ϸ��� ������ ���� ����� �Ѵ�!!
    private void Start()
    {
        stats.CurrentHealth = 100;
        OnHealthChanged?.Invoke(stats.CurrentHealth);
    }
    private void Update()
    {
        HealthControl();
        Debug.Log($"{stats.CurrentHealth}");
    }

    private void HealthControl()
    {
        if (_playerMove._isRunning == true || _playerMove._isRolling == true || _playerMove._isWallSliding == true)
        {
            stats.CurrentHealth -= stats.HealthDamage * Time.deltaTime;
        }
        else
        {
            stats.CurrentHealth += stats.HealthGain * Time.deltaTime;
        }
        stats.CurrentHealth = Mathf.Clamp(stats.CurrentHealth, 0f, stats.MaxHealth);
        OnHealthChanged?.Invoke(stats.CurrentHealth);

        if (stats.CurrentHealth <= 0f)
        {
            _playerMove.ForceFallFromWall();
        }

    }

    public void TakeDamage(Damage damage)
    {
        stats.CurrentHealth -= damage.Value;
        stats.CurrentHealth = Mathf.Clamp(stats.CurrentHealth, 0f, stats.MaxHealth);
        OnHealthChanged?.Invoke(stats.CurrentHealth);
        Debug.Log($"{stats.CurrentHealth}");
    }
}
