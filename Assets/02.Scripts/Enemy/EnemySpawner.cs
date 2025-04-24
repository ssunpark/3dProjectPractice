using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // 목표: Enemy 풀링 적용 및 스포너 구현
    // 필요속성
    // - 스폰 포인트
    // - 스폰될 적들
    // - 스폰 포인트의 일정 랜덤 범위
    // - 적 생성되는 간격(시간)

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
            Debug.LogWarning("Enemy Pool 비었음!");
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

    // 적이 죽거나 사라졌을 때 다시 풀에 반환하는 메서드
    public void ReturnToPool(GameObject enemy)
    {
        enemy.SetActive(false);
        _enemyPool.Enqueue(enemy);
    }
}
