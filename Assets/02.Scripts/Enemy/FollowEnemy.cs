using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FollowEnemy : MonoBehaviour
{
    // 0: 대기
    // 1: 추적
    // 2: 복귀
    // 3: 공격
    // 4: 피격
    // 5: 사망

    // 1. 상태를 열거형으로 정의한다.
    public enum EnemyState
    {
        Idle,
        Trace,
        Attack,
        Damaged,
        Die
    }

    // 2. 현재 상태를 지정한다.
    public EnemyState CurrentState = EnemyState.Idle;

    private GameObject _player;                       // 플레이어
    private CharacterController _characterController; // 캐릭터 컨트롤러
    private NavMeshAgent _agent;                      // 네브매시 에이전트
    private Vector3 _startPosition;                   // 시작 위치

    public float MoveSpeed = 5f;   // 이동 속도
    public int Health = 100;

    public float FindDistance = 150f;     // 플레이어 발견 범위
    public float AttackDistance = 2.5f;   // 플레이어 공격 범위

    public float AttackCooltime = 2f;     // 공격 쿨타임
    private float _attackTimer = 0f;     // ㄴ 체크기

    public float Damagedtime = 0.5f; // 경직 시간
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
        // 나의 현재 상태에 따라 상태 함수를 호출한다.
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

    // 3. 상태 함수들을 구현한다.

    private void Idle()
    {
        // 행동: 가만히 있는다.
        if (Vector3.Distance(transform.position, _player.transform.position) < FindDistance)
        {
            Debug.Log("상태전환: Idle -> Trace");
            CurrentState = EnemyState.Trace;
            return;
        }
    }

    private void Trace()
    {

        // 전이: 공격 범위 만큼 가까워 지면 -> Attack
        if (Vector3.Distance(transform.position, _player.transform.position) < AttackDistance)
        {
            Debug.Log("상태전환: Trace -> Attack");
            CurrentState = EnemyState.Attack;
            return;
        }

        // 행동: 플레이어를 추적한다.
        Vector3 dir = (_player.transform.position - transform.position).normalized;
        //_characterController.Move(dir * MoveSpeed * Time.deltaTime);
        _agent.SetDestination(_player.transform.position);
    }

    private void Attack()
    {
        // 전이: 공격 범위 보다 멀어지면 -> Trace
        if (Vector3.Distance(transform.position, _player.transform.position) >= AttackDistance)
        {
            Debug.Log("상태전환: Attack -> Trace");
            CurrentState = EnemyState.Trace;
            return;
        }

        // 행동: 플레이어를 공격한다.
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= AttackCooltime)
        {
            Debug.Log("플레이어 공격!");
            _attackTimer = 0f;
        }
    }

    private IEnumerator Damaged_Coroutine()
    {
        // 넉백 구현
        _knockbackTimer = 0f;
        Vector3 knockbackDir = (transform.position - _player.transform.position).normalized;
        while (_knockbackTimer < Damagedtime)
        {
            _knockbackTimer += Time.deltaTime;
            _characterController.Move(knockbackDir * KnockbackPower * Time.deltaTime);
            yield return null;
        }
        Debug.Log("상태전환: Damaged -> Trace");
        CurrentState = EnemyState.Trace;
    }

    private IEnumerator Die_Coroutine()
    {
        // 행동 죽는다.
        yield return null;
        gameObject.SetActive(false);
    }

}
