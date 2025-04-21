using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    // ��ǥ: �÷��̾ ShiftŰ�� ������ ���� ü���� ������ ���ҵȴ�
    // �ʿ� �Ӽ�
    // - �⺻ ü��
    // - Max ü��
    // - Shift Ű ���� ���� ���ҵ� ü�·�
    public float CurrentHealth = 100f;
    public float MaxHealth = 200f;
    public float HealthDamage = 5f;
    public float HealthGain = 2f;
    public Slider PlayerHealthBar;

    public PlayerMove _playerMove;

    // �����̴��� �ݿ��Ϸ��� ������ ���� ����� �Ѵ�!!
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
