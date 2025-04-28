using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStasSO", menuName = "Scriptable Objects/PlayerStasSO")]
public class PlayerStasSO : ScriptableObject
{
    // 목표: 플레이어가 Shift키를 누르는 동안 체력이 서서히 감소된다
    // 필요 속성
    // - 기본 체력
    // - Max 체력
    // - Shift 키 누를 동안 감소될 체력량
    public float MaxHealth = 200f;
    public float HealthDamage = 5f;
    public float HealthGain = 3f;
}
