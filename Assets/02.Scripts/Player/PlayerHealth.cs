using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public PlayerStasSO stats;
    public PlayerMove _playerMove;
    public UnityEvent<float> OnHealthChanged;

    public float CurrentHealth = 100f; // ��� ��� ���ϴϱ� ���� �Ȼ���.
    // �����̴��� �ݿ��Ϸ��� ������ ���� ����� �Ѵ�!!
    private void Start()
    {
        OnHealthChanged?.Invoke(CurrentHealth);
    }
    private void Update()
    {
        HealthControl();
    }

    private void HealthControl()
    {
        bool isConsuming = _playerMove._isRunning || _playerMove._isRolling || _playerMove._isWallSliding;

        if (isConsuming)
        {
            CurrentHealth -= stats.HealthDamage * Time.deltaTime;
        }

        // ȸ���� ������ �õ��ϵ�, �Һ� �߿��� ����
        if (!isConsuming)
        {
            CurrentHealth += stats.HealthGain * Time.deltaTime;
        }

        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, stats.MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0f)
        {
            _playerMove.ForceFallFromWall();
        }

    }

    public void TakeDamage(Damage damage)
    {
        CurrentHealth -= damage.Value;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, stats.MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
        Debug.Log($"{CurrentHealth}");
    }
}
