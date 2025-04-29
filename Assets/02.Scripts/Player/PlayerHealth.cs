using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    public PlayerStasSO stats;
    public PlayerMove _playerMove;
    public UnityEvent<float> OnHealthChanged;

    public float CurrentHealth { get; private set; }
    private bool _isDead = false;

    private void Start()
    {
        CurrentHealth = stats.MaxHealth;
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
            ApplyHealth(-stats.HealthDamage * Time.deltaTime);
        else
            ApplyHealth(stats.HealthGain * Time.deltaTime);

        if (CurrentHealth <= 0f && !_isDead)
        {
            _isDead = true;
            _playerMove.ForceFallFromWall();
        }
    }

    public void TakeDamage(Damage damage)
    {
        ApplyHealth(-damage.Value);
        Debug.Log($"[ÇÇ°Ý] CurrentHealth: {CurrentHealth}");
    }

    private void ApplyHealth(float amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, stats.MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
    }
}
