using System.Collections;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    // 목표: 플레이어의 총알이나 폭탄을 맞으면 폭발한다.
    // 필요 속성
    // - 체력
    // - 플레이어와 적에게 줄 데미지량
    // - 어느 반경까지 데미지를 줄 것인지 범위값(거리값) Vector3
    // - 날아갈 방향
    // - 폭발 이펙트
    // 날아갈 때 필요한 MoveSpeed

    public float BarrelHealth = 7f;
    public int BarrelDamage = 10;
    public float BarrelSpeed = 10f;
    Vector3 BarrelDamageScope;
    public GameObject BarrelVfxPrefab;
    private bool _isExploded = false;

    private void Update()
    {
    }

    //플레이어의 총알이나 폭탄을 감지한다.
    public void TakeDamage(Damage damage)
    {
        BarrelHealth -= damage.Value;

        if (BarrelHealth <= 0 && _isExploded == false)
        {
            _isExploded = true;
            GameObject barrelVFX = Instantiate(BarrelVfxPrefab, transform.position, Quaternion.identity, transform);
            StartCoroutine(Explosion_Coroutine());
        }
    }

    private IEnumerator Explosion_Coroutine()
    {
        // 어딘가로 날라간다.
        Collider[] targets = Physics.OverlapSphere(transform.position, 5f);
        foreach (var target in targets)
        {
            if (target.CompareTag("Player"))
            {
                //Debug.Log("플레이어 발견!");
                PlayerHealth player = target.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    Damage damage = new Damage();
                    damage.Value = BarrelDamage;
                    damage.From = this.gameObject;
                    player.TakeDamage(damage);
                    //Debug.Log($"드럼통 폭발! 플레이어에게 대미지 줌!!: {damage}");
                }
            }
            if (target.CompareTag("Enemy"))
            {
                //Debug.Log("적 발견!");
                Enemy enemy = target.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Damage damage = new Damage();
                    damage.Value = BarrelDamage;
                    damage.From = this.gameObject;
                    enemy.TakeDamage(damage);
                    //Debug.Log($"드럼통 폭발! 적에게 대미지 줌!!: {damage}");
                }
            }
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 randomDirection = (Random.onUnitSphere + Vector3.up).normalized;
            rb.AddForce(randomDirection * BarrelSpeed, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }
}
