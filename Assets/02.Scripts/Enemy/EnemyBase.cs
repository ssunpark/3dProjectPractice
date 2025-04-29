using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class EnemyBase : MonoBehaviour, IDamagable
{
    [Header("공통 설정")]
    public EnemyHealth enemyHealth;
    public GameObject bloodScreen;
    protected GameObject player;
    protected CharacterController characterController;
    protected NavMeshAgent agent;
    protected Vector3 startPosition;

    [Header("공통 이동 설정")]
    public float moveSpeed = 3.3f;
    public float attackDistance = 2.5f;
    public float findDistance = 5f;
    public float attackCooltime = 2f;
    public int knockbackPower = 20;

    [Header("공통 전투 설정")]
    public float damagedTime = 0.5f;

    protected float attackTimer = 0f;
    protected float knockbackTimer = 0f;
    protected bool isDead = false;

    protected virtual void Awake()
    {
        characterController = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        startPosition = transform.position;
        agent.speed = moveSpeed;

        if (enemyHealth == null)
            enemyHealth = GetComponent<EnemyHealth>();
    }

    protected virtual void Update()
    {
        if (isDead) return;

        attackTimer += Time.deltaTime;
        OnUpdate();
    }

    protected abstract void OnUpdate();

    public virtual void TakeDamage(Damage damage)
    {
        if (isDead) return;

        enemyHealth.ApplyDamage(damage.Value);

        if (enemyHealth.CurrentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamagedCoroutine());
        }
    }

    protected IEnumerator DamagedCoroutine()
    {
        knockbackTimer = 0f;
        Vector3 knockbackDir = (transform.position - player.transform.position).normalized;
        while (knockbackTimer < damagedTime)
        {
            knockbackTimer += Time.deltaTime;
            characterController.Move(knockbackDir * knockbackPower * Time.deltaTime);
            yield return null;
        }
    }

    protected virtual void Attack()
    {
        if (attackTimer >= attackCooltime)
        {
            Debug.Log("플레이어 공격!");
            StartCoroutine(AttackEffectCoroutine());
            attackTimer = 0f;
        }
    }

    protected IEnumerator AttackEffectCoroutine()
    {
        if (bloodScreen == null) yield break;

        Image image = bloodScreen.GetComponent<Image>();
        Color color = image.color;
        color.a = 1f;
        image.color = color;
        bloodScreen.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        float fadeDuration = 0.5f;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float newAlpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            image.color = new Color(color.r, color.g, color.b, newAlpha);
            yield return null;
        }

        bloodScreen.SetActive(false);
        image.color = new Color(color.r, color.g, color.b, 0f);
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        StartCoroutine(DieCoroutine());
    }

    protected IEnumerator DieCoroutine()
    {
        yield return null;
        gameObject.SetActive(false);
    }
}
