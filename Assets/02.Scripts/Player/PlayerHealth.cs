using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public PlayerStasSO stats;
    public Slider PlayerHealthBar;
    public PlayerMove _playerMove;

    // 슬라이더에 반영하려면 설정을 따로 해줘야 한다!!
    private void Start()
    {
        PlayerHealthBar.minValue = 0;
        PlayerHealthBar.maxValue = stats.MaxHealth;
        PlayerHealthBar.value = stats.CurrentHealth;
    }

    private void Update()
    {
        HealthControl();
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
        PlayerHealthBar.value = stats.CurrentHealth;

        if (stats.CurrentHealth <= 0f)
        {
            _playerMove.ForceFallFromWall();
        }

    }
}
