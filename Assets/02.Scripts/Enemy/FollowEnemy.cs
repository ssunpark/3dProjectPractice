using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FollowEnemy : MonoBehaviour
{
    // 0: ���
    // 1: ����
    // 2: ����
    // 3: ����
    // 4: �ǰ�
    // 5: ���

    // 1. ���¸� ���������� �����Ѵ�.
    public enum EnemyState
    {
        Idle,
        Trace,
        Attack,
        Damaged,
        Die
    }

    // 2. ���� ���¸� �����Ѵ�.
    public EnemyState CurrentState = EnemyState.Idle;

    private GameObject _player;                       // �÷��̾�
    private CharacterController _characterController; // ĳ���� ��Ʈ�ѷ�
    private NavMeshAgent _agent;                      // �׺�Ž� ������Ʈ
    private Vector3 _startPosition;                   // ���� ��ġ

    public float MoveSpeed = 5f;   // �̵� �ӵ�
    public int Health = 100;

    public float FindDistance = 150f;     // �÷��̾� �߰� ����
    public float AttackDistance = 2.5f;   // �÷��̾� ���� ����

    public float AttackCooltime = 2f;     // ���� ��Ÿ��
    private float _attackTimer = 0f;     // �� üũ��

    public float Damagedtime = 0.5f; // ���� �ð�
    public float DeathTime = 1f;


    public int KnockbackPower = 20;
    private float _knockbackTimer = 0f;



    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = MoveSpeed;
        _startPosition = transform.position;
        _characterController = GetComponent<CharacterController>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }


    private void Update()
    {
        // ���� ���� ���¿� ���� ���� �Լ��� ȣ���Ѵ�.
        switch (CurrentState)
        {
            case EnemyState.Idle:
                {
                    Idle();
                    break;
                }

            case EnemyState.Trace:
                {
                    Trace();
                    break;
                }

            case EnemyState.Attack:
                {
                    Attack();
                    break;
                }
            case EnemyState.Damaged:
                {
                    //Damage();
                    break;
                }
        }
    }

    public void TakeDamage(Damage damage)
    {
        if (CurrentState == EnemyState.Die)
        {
            return;
        }
        Health -= damage.Value;
        if (Health <= 0)
        {
            CurrentState = EnemyState.Die;
            StartCoroutine(Die_Coroutine());
            return;
        }

        if (CurrentState != EnemyState.Damaged)
        {
            CurrentState = EnemyState.Damaged;
            StartCoroutine(Damaged_Coroutine());
        }
    }

    // 3. ���� �Լ����� �����Ѵ�.

    private void Idle()
    {
        // �ൿ: ������ �ִ´�.
        if (Vector3.Distance(transform.position, _player.transform.position) < FindDistance)
        {
            Debug.Log("������ȯ: Idle -> Trace");
            CurrentState = EnemyState.Trace;
            return;
        }
    }

    private void Trace()
    {

        // ����: ���� ���� ��ŭ ����� ���� -> Attack
        if (Vector3.Distance(transform.position, _player.transform.position) < AttackDistance)
        {
            Debug.Log("������ȯ: Trace -> Attack");
            CurrentState = EnemyState.Attack;
            return;
        }

        // �ൿ: �÷��̾ �����Ѵ�.
        Vector3 dir = (_player.transform.position - transform.position).normalized;
        //_characterController.Move(dir * MoveSpeed * Time.deltaTime);
        _agent.SetDestination(_player.transform.position);
    }

    private void Attack()
    {
        // ����: ���� ���� ���� �־����� -> Trace
        if (Vector3.Distance(transform.position, _player.transform.position) >= AttackDistance)
        {
            Debug.Log("������ȯ: Attack -> Trace");
            CurrentState = EnemyState.Trace;
            return;
        }

        // �ൿ: �÷��̾ �����Ѵ�.
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= AttackCooltime)
        {
            Debug.Log("�÷��̾� ����!");
            _attackTimer = 0f;
        }
    }

    private IEnumerator Damaged_Coroutine()
    {
        // �˹� ����
        _knockbackTimer = 0f;
        Vector3 knockbackDir = (transform.position - _player.transform.position).normalized;
        while (_knockbackTimer < Damagedtime)
        {
            _knockbackTimer += Time.deltaTime;
            _characterController.Move(knockbackDir * KnockbackPower * Time.deltaTime);
            yield return null;
        }
        Debug.Log("������ȯ: Damaged -> Trace");
        CurrentState = EnemyState.Trace;
    }

    private IEnumerator Die_Coroutine()
    {
        // �ൿ �״´�.
        yield return null;
        gameObject.SetActive(false);
    }

}
