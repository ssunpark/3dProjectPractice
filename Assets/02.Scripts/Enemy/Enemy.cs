using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
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
        Patrol,
        Trace,
        Return,
        Attack,
        Damaged,
        Die
    }

    // 2. 현재 상태를 지정한다.
    public EnemyState CurrentState = EnemyState.Idle;

    private GameObject _player;                       // 플레이어
    public GameObject _bloodScreen;
    private CharacterController _characterController; // 캐릭터 컨트롤러
    private NavMeshAgent _agent;                      // 네브매시 에이전트
    private Vector3 _startPosition;                   // 시작 위치
    public Transform[] EnemyPatrolPoint; // 순찰할 때 돌아다닐 포인트들
    public Vector3 CurrentPatrolTarget;
    public EnemyHealth _health;

    public float MoveSpeed = 3.3f;   // 이동 속도
    //public int Health = 100;

    public float FindDistance = 5f;     // 플레이어 발견 범위
    public float ReturnDistance = 5f;     // 적 복귀 범위
    public float AttackDistance = 2.5f;   // 플레이어 공격 범위

    public float AttackCooltime = 2f;     // 공격 쿨타임
    private float _attackTimer = 0f;     // ㄴ 체크기

    public float Damagedtime = 0.5f; // 경직 시간
    public float DeathTime = 1f;

    public float IdleTime = 5f;
    private float _idleTimer = 0f;

    private bool _hasPatrolTarget = false;

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

    private void Awake()
    {
        _health = GetComponent<EnemyHealth>();
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

            case EnemyState.Patrol:
                {
                    Patrol();
                    break;
                }

            case EnemyState.Trace:
                {
                    Trace();
                    break;
                }

            case EnemyState.Return:
                {
                    Return();
                    break;
                }

            case EnemyState.Attack:
                {
                    Attack();
                    break;
                }
                //case EnemyState.Damaged:
                //    {
                //        Damage();
                //        break;
                //    }
        }
    }

    public void TakeDamage(Damage damage)
    {
        if (CurrentState == EnemyState.Die)
        {
            return;
        }
        if (_health != null)
        {
            _health.TakeDamage(damage.Value);
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

        // 전이: 오랫동안 가만히 있으면 Patrol 시작
        if (_idleTimer < IdleTime)
        {
            _idleTimer += Time.deltaTime;
        }
        else if (_idleTimer >= IdleTime)
        {
            Debug.Log("상태전환: Idle -> Patrol");
            CurrentState = EnemyState.Patrol;
            _idleTimer = 0f;
            return;
        }

        // 전이: 플레이어와 가까워 지면 -> Trace
        if (Vector3.Distance(transform.position, _player.transform.position) < FindDistance)
        {
            Debug.Log("상태전환: Idle -> Trace");
            CurrentState = EnemyState.Trace;
            _idleTimer = 0f;
            return;
        }
    }

    private void Patrol()
    {
        //Debug.Log("Patrol됐다!!!!!");
        // Idle 상태가 오래 지속되면 지정된 위치 3곳을 왔다리 갔다리
        // 필요 속성
        // Idle 상태가 얼마나 지속되엇는지 확인할 시간 변수
        // 돌아다닐 위치 3곳

        // 랜덤으로 포인트 한 곳 가고 idle
        if (_hasPatrolTarget == false)
        {
            int i = Random.Range(0, EnemyPatrolPoint.Length);
            CurrentPatrolTarget = EnemyPatrolPoint[i].position;
            _hasPatrolTarget = true;
        }

        if (Vector3.Distance(transform.position, CurrentPatrolTarget) <= _characterController.minMoveDistance + transform.position.y)
        {
            Debug.Log("상태전환: Patrol -> Idle");
            CurrentState = EnemyState.Idle;
            _hasPatrolTarget = false;
            return;
        }

        // 돌아다니다가 가까이오면 추적
        if (Vector3.Distance(transform.position, _player.transform.position) < FindDistance)
        {
            Debug.Log("상태전환: Patrol -> Trace");
            CurrentState = EnemyState.Trace;
            _hasPatrolTarget = false;
            return;
        }
        Vector3 dir = (CurrentPatrolTarget - transform.position).normalized;
        //_characterController.Move(dir * MoveSpeed * Time.deltaTime);
        _agent.SetDestination(CurrentPatrolTarget);
    }

    private void Trace()
    {
        // 전이: 플레이어와 멀어지면 -> Return
        if (Vector3.Distance(transform.position, _player.transform.position) > ReturnDistance)
        {
            Debug.Log("상태전환: Trace -> Return");
            CurrentState = EnemyState.Return;
            return;
        }

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

    private void Return()
    {
        // 전이: 시작 위치와 가까워 지면 -> Idle
        if (Vector3.Distance(transform.position, _startPosition) <= _characterController.minMoveDistance)
        {
            Debug.Log("상태전환: Return -> Idle");
            transform.position = _startPosition;
            CurrentState = EnemyState.Idle;
            return;
        }

        // 전이: 플레이어와 가까워 지면 -> Trace
        if (Vector3.Distance(transform.position, _player.transform.position) < FindDistance)
        {
            Debug.Log("상태전환: Return -> Trace");
            CurrentState = EnemyState.Trace;
        }


        // 행동: 시작 위치로 되돌아간다.
        Vector3 dir = (_startPosition - transform.position).normalized;
        //_characterController.Move(dir * MoveSpeed * Time.deltaTime);
        _agent.SetDestination(_startPosition);
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
            StartCoroutine(Attack_Coroutine());
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

    private IEnumerator Attack_Coroutine()
    {
        // 이미지 알파 초기화
        Image image = _bloodScreen.GetComponent<Image>();
        Color color = image.color;
        color.a = 1f;
        image.color = color;

        // 일단 바로 보여줌
        _bloodScreen.SetActive(true);

        // 잠깐 유지
        yield return new WaitForSeconds(0.2f);

        // 페이드아웃
        float fadeDuration = 0.5f;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            float newAlpha = Mathf.Lerp(1f, 0f, t);
            image.color = new Color(color.r, color.g, color.b, newAlpha);
            yield return null;
        }

        // 확실히 끄고 알파도 0으로 고정
        _bloodScreen.SetActive(false);
        image.color = new Color(color.r, color.g, color.b, 0f);
    }

    public void OnDeath()
    {
        if (CurrentState == EnemyState.Die) return;
        CurrentState = EnemyState.Die;
        StartCoroutine(Die_Coroutine());
    }
}
