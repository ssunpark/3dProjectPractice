using UnityEngine;

public class EnemyAttackEvent : MonoBehaviour
{
    public EnemyBase MyEnemy;
    public void AttackEvent()
    {
        Debug.Log("플레이어 공격");
    }
}
