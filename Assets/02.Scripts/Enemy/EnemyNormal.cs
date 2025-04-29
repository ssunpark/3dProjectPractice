using UnityEngine;

public class EnemyNormal : EnemyBase
{
    [Header("순찰 설정")]
    public Transform[] patrolPoints;
    private Vector3 currentPatrolTarget;
    private bool hasPatrolTarget = false;

    [Header("Idle 설정")]
    public float idleTime = 5f;
    private float idleTimer = 0f;

    protected override void OnUpdate()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < findDistance)
        {
            TracePlayer();
            return;
        }

        Patrol();
    }

    private void Patrol()
    {
        if (!hasPatrolTarget)
        {
            int i = Random.Range(0, patrolPoints.Length);
            currentPatrolTarget = patrolPoints[i].position;
            hasPatrolTarget = true;
        }

        agent.SetDestination(currentPatrolTarget);

        if (Vector3.Distance(transform.position, currentPatrolTarget) < 1f)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > idleTime)
            {
                hasPatrolTarget = false;
                idleTimer = 0f;
            }
        }
    }

    private void TracePlayer()
    {
        agent.SetDestination(player.transform.position);

        if (Vector3.Distance(transform.position, player.transform.position) < attackDistance)
        {
            Attack();
        }
    }
}
