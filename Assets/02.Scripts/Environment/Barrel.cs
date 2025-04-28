using System.Collections;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    // ��ǥ: �÷��̾��� �Ѿ��̳� ��ź�� ������ �����Ѵ�.
    // �ʿ� �Ӽ�
    // - ü��
    // - �÷��̾�� ������ �� ��������
    // - ��� �ݰ���� �������� �� ������ ������(�Ÿ���) Vector3
    // - ���ư� ����
    // - ���� ����Ʈ
    // ���ư� �� �ʿ��� MoveSpeed

    public float BarrelHealth = 7f;
    public int BarrelDamage = 10;
    public float BarrelSpeed = 10f;
    Vector3 BarrelDamageScope;
    public GameObject BarrelVfxPrefab;
    private bool _isExploded = false;

    private void Update()
    {
    }

    //�÷��̾��� �Ѿ��̳� ��ź�� �����Ѵ�.
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
        // ��򰡷� ���󰣴�.
        Collider[] targets = Physics.OverlapSphere(transform.position, 5f);
        foreach (var target in targets)
        {
            if (target.CompareTag("Player"))
            {
                //Debug.Log("�÷��̾� �߰�!");
                PlayerHealth player = target.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    Damage damage = new Damage();
                    damage.Value = BarrelDamage;
                    damage.From = this.gameObject;
                    player.TakeDamage(damage);
                    //Debug.Log($"�巳�� ����! �÷��̾�� ����� ��!!: {damage}");
                }
            }
            if (target.CompareTag("Enemy"))
            {
                //Debug.Log("�� �߰�!");
                Enemy enemy = target.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Damage damage = new Damage();
                    damage.Value = BarrelDamage;
                    damage.From = this.gameObject;
                    enemy.TakeDamage(damage);
                    //Debug.Log($"�巳�� ����! ������ ����� ��!!: {damage}");
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
