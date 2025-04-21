using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    // 목표: 플레이어가 Shift키를 누르는 동안 체력이 서서히 감소된다
    // 필요 속성
    // - 기본 체력
    // - Max 체력
    // - Shift 키 누를 동안 감소될 체력량
    public float CurrentHealth = 100f;
    public float MaxHealth = 200f;
    public float HealthDamage = 5f;
    public float HealthGain = 2f;
    public Slider PlayerHealthBar;

    public PlayerMove _playerMove;

    // 슬라이더에 반영하려면 설정을 따로 해줘야 한다!!
    private void Start()
    {
        PlayerHealthBar.minValue = 0;
        PlayerHealthBar.maxValue = MaxHealth;
        PlayerHealthBar.value = CurrentHealth;
    }

    private void Update()
    {
        if (_playerMove._isRunning == true || _playerMove._isRolling == true)
        {
            CurrentHealth -= HealthDamage * Time.deltaTime;
        }
        else
        {
            CurrentHealth += HealthGain * Time.deltaTime;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);
        }
        PlayerHealthBar.value = CurrentHealth;

    }
}
