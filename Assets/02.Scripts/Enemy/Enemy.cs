using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
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
        Patrol,
        Trace,
        Return,
        Attack,
        Damaged,
        Die
    }

    // 2. ���� ���¸� �����Ѵ�.
    public EnemyState CurrentState = EnemyState.Idle;

    private GameObject _player;                       // �÷��̾�
    public GameObject _bloodScreen;
    private CharacterController _characterController; // ĳ���� ��Ʈ�ѷ�
    private NavMeshAgent _agent;                      // �׺�Ž� ������Ʈ
    private Vector3 _startPosition;                   // ���� ��ġ
    public Transform[] EnemyPatrolPoint; // ������ �� ���ƴٴ� ����Ʈ��
    public Vector3 CurrentPatrolTarget;
    public EnemyHealth _health;

    public float MoveSpeed = 3.3f;   // �̵� �ӵ�
    //public int Health = 100;

    public float FindDistance = 5f;     // �÷��̾� �߰� ����
    public float ReturnDistance = 5f;     // �� ���� ����
    public float AttackDistance = 2.5f;   // �÷��̾� ���� ����

    public float AttackCooltime = 2f;     // ���� ��Ÿ��
    private float _attackTimer = 0f;     // �� üũ��

    public float Damagedtime = 0.5f; // ���� �ð�
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
        // ���� ���� ���¿� ���� ���� �Լ��� ȣ���Ѵ�.
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

    // 3. ���� �Լ����� �����Ѵ�.

    private void Idle()
    {
        // �ൿ: ������ �ִ´�.

        // ����: �������� ������ ������ Patrol ����
        if (_idleTimer < IdleTime)
        {
            _idleTimer += Time.deltaTime;
        }
        else if (_idleTimer >= IdleTime)
        {
            Debug.Log("������ȯ: Idle -> Patrol");
            CurrentState = EnemyState.Patrol;
            _idleTimer = 0f;
            return;
        }

        // ����: �÷��̾�� ����� ���� -> Trace
        if (Vector3.Distance(transform.position, _player.transform.position) < FindDistance)
        {
            Debug.Log("������ȯ: Idle -> Trace");
            CurrentState = EnemyState.Trace;
            _idleTimer = 0f;
            return;
        }
    }

    private void Patrol()
    {
        //Debug.Log("Patrol�ƴ�!!!!!");
        // Idle ���°� ���� ���ӵǸ� ������ ��ġ 3���� �Դٸ� ���ٸ�
        // �ʿ� �Ӽ�
        // Idle ���°� �󸶳� ���ӵǾ����� Ȯ���� �ð� ����
        // ���ƴٴ� ��ġ 3��

        // �������� ����Ʈ �� �� ���� idle
        if (_hasPatrolTarget == false)
        {
            int i = Random.Range(0, EnemyPatrolPoint.Length);
            CurrentPatrolTarget = EnemyPatrolPoint[i].position;
            _hasPatrolTarget = true;
        }

        if (Vector3.Distance(transform.position, CurrentPatrolTarget) <= _characterController.minMoveDistance + transform.position.y)
        {
            Debug.Log("������ȯ: Patrol -> Idle");
            CurrentState = EnemyState.Idle;
            _hasPatrolTarget = false;
            return;
        }

        // ���ƴٴϴٰ� �����̿��� ����
        if (Vector3.Distance(transform.position, _player.transform.position) < FindDistance)
        {
            Debug.Log("������ȯ: Patrol -> Trace");
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
        // ����: �÷��̾�� �־����� -> Return
        if (Vector3.Distance(transform.position, _player.transform.position) > ReturnDistance)
        {
            Debug.Log("������ȯ: Trace -> Return");
            CurrentState = EnemyState.Return;
            return;
        }

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

    private void Return()
    {
        // ����: ���� ��ġ�� ����� ���� -> Idle
        if (Vector3.Distance(transform.position, _startPosition) <= _characterController.minMoveDistance)
        {
            Debug.Log("������ȯ: Return -> Idle");
            transform.position = _startPosition;
            CurrentState = EnemyState.Idle;
            return;
        }

        // ����: �÷��̾�� ����� ���� -> Trace
        if (Vector3.Distance(transform.position, _player.transform.position) < FindDistance)
        {
            Debug.Log("������ȯ: Return -> Trace");
            CurrentState = EnemyState.Trace;
        }


        // �ൿ: ���� ��ġ�� �ǵ��ư���.
        Vector3 dir = (_startPosition - transform.position).normalized;
        //_characterController.Move(dir * MoveSpeed * Time.deltaTime);
        _agent.SetDestination(_startPosition);
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
            StartCoroutine(Attack_Coroutine());
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

    private IEnumerator Attack_Coroutine()
    {
        // �̹��� ���� �ʱ�ȭ
        Image image = _bloodScreen.GetComponent<Image>();
        Color color = image.color;
        color.a = 1f;
        image.color = color;

        // �ϴ� �ٷ� ������
        _bloodScreen.SetActive(true);

        // ��� ����
        yield return new WaitForSeconds(0.2f);

        // ���̵�ƿ�
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

        // Ȯ���� ���� ���ĵ� 0���� ����
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
