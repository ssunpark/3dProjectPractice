using UnityEngine;

public class Bomb : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // 이펙트 풀링
        GameObject vfx = BombPoolManager.Instance.GetVFX();
        if (vfx != null)
        {
            vfx.transform.position = transform.position;
            vfx.transform.rotation = Quaternion.identity;
        }

        // 데미지 처리
        if (TryGetComponent<SphereCollider>(out var sphere))
        {
            Collider[] targets = Physics.OverlapSphere(transform.position, sphere.radius * transform.localScale.x);
            foreach (var target in targets)
            {
                if (target.TryGetComponent<IDamagable>(out var damagable))
                {
                    damagable.TakeDamage(new Damage
                    {
                        Value = 10,
                        From = this.gameObject
                    });
                }
            }
        }

        // 물리 초기화
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 다시 풀로 복귀
        BombPoolManager.Instance.ReturnBomb(this.gameObject);
    }
}
