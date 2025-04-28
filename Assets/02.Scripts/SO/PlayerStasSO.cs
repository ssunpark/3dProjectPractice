using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStasSO", menuName = "Scriptable Objects/PlayerStasSO")]
public class PlayerStasSO : ScriptableObject
{
    // ��ǥ: �÷��̾ ShiftŰ�� ������ ���� ü���� ������ ���ҵȴ�
    // �ʿ� �Ӽ�
    // - �⺻ ü��
    // - Max ü��
    // - Shift Ű ���� ���� ���ҵ� ü�·�
    public float MaxHealth = 200f;
    public float HealthDamage = 5f;
    public float HealthGain = 3f;
}
