using System.Collections;
using UnityEngine;

public class Barrel : MonoBehaviour, IDamagable
{
    public float BarrelHealth = 7f;
    public int BarrelDamage = 10;
    public float ExplosionRadius = 5f;
    public float BarrelSpeed = 10f;

    public GameObject BarrelVfxPrefab;

    private bool _isExploded = false;

    public void TakeDamage(Damage damage)
    {
        if (_isExploded) return;

        BarrelHealth -= damage.Value;

        if (BarrelHealth <= 0)
        {
            _isExploded = true;
            Instantiate(BarrelVfxPrefab, transform.position, Quaternion.identity);
            StartCoroutine(Explosion_Coroutine());
        }
    }

    private IEnumerator Explosion_Coroutine()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, ExplosionRadius);

        foreach (var target in targets)
        {
            if (target.TryGetComponent(out IDamagable damagable))
            {
                Damage damage = new Damage
                {
                    Value = BarrelDamage,
                    From = this.gameObject
                };
                damagable.TakeDamage(damage);
            }
        }

        if (TryGetComponent(out Rigidbody rb))
        {
            Vector3 randomDirection = (Random.onUnitSphere + Vector3.up).normalized;
            rb.AddForce(randomDirection * BarrelSpeed, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }
}
