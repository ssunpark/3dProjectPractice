using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public PlayerStasSO stats;
    public PlayerMove _playerMove;
    public UnityEvent<float> OnHealthChanged;

    public float CurrentHealth = 100f; // 얘는 계속 변하니까 따로 안빼줌.
    // 슬라이더에 반영하려면 설정을 따로 해줘야 한다!!
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

        // 회복은 언제나 시도하되, 소비 중에는 막자
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
