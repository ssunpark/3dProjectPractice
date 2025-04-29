using UnityEngine;

public class EnemyFollow : EnemyBase
{
    protected override void OnUpdate()
    {
        agent.SetDestination(player.transform.position);

        if (Vector3.Distance(transform.position, player.transform.position) < attackDistance)
        {
            Attack();
        }
    }
}