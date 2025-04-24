using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // ��ǥ: Enemy Ǯ�� ���� �� ������ ����
    // �ʿ�Ӽ�
    // - ���� ����Ʈ
    // - ������ ����
    // - ���� ����Ʈ�� ���� ���� ����
    // - �� �����Ǵ� ����(�ð�)

    public Vector3 SpawnPoint;
    public GameObject[] Enemies;
    public Vector3 EnemySpawnScope = new Vector3(10, 0, 10);
    public float EnemySpawnTime = 3f;

    private int _poolSize = 50;
    private Queue<GameObject> _enemyPool = new Queue<GameObject>();
    private float _spawnTimer = 0f;

    private void Start()
    {
        SpawnPoint = transform.position;
        InitializePool();

    }

    private void Update()
    {
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= EnemySpawnTime)
        {
            _spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            int r = Random.Range(0, Enemies.Length);
            GameObject enemy = Instantiate(Enemies[r]);
            enemy.SetActive(false);
            _enemyPool.Enqueue(enemy);
        }
    }

    private void SpawnEnemies()
    {
        if (_enemyPool.Count == 0)
        {
            Debug.LogWarning("Enemy Pool �����!");
            return;
        }

        GameObject enemy = _enemyPool.Dequeue();

        Vector3 randomOffset = new Vector3(
            Random.Range(-EnemySpawnScope.x, EnemySpawnScope.x),
            Random.Range(-EnemySpawnScope.y, EnemySpawnScope.y),
            Random.Range(-EnemySpawnScope.z, EnemySpawnScope.z)
        );

        enemy.transform.position = SpawnPoint + randomOffset;
        enemy.transform.rotation = Quaternion.identity;
        enemy.SetActive(true);
    }

    // ���� �װų� ������� �� �ٽ� Ǯ�� ��ȯ�ϴ� �޼���
    public void ReturnToPool(GameObject enemy)
    {
        enemy.SetActive(false);
        _enemyPool.Enqueue(enemy);
    }
}
