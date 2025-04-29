using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject VfxPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        // 이펙트 생성
        if (VfxPrefab != null)
            Instantiate(VfxPrefab, transform.position, Quaternion.identity);

        // 필요한 경우 폭발 범위 내 대미지 처리
        if (TryGetComponent<SphereCollider>(out var sphere))
        {
            Collider[] targets = Physics.OverlapSphere(transform.position, sphere.radius * transform.localScale.x);
            foreach (var target in targets)
            {
                if (target.TryGetComponent<IDamagable>(out var damagable))
                {
                    Damage damage = new Damage
                    {
                        Value = 10,
                        From = this.gameObject
                    };
                    damagable.TakeDamage(damage);
                }
            }
        }

        // Rigidbody 초기화
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 풀링 방식: SetActive(false)
        gameObject.SetActive(false);
    }
}
