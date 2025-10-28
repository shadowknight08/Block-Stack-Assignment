using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private float spawnRadius = 15f;

    [Header("Enemy Data")]
    [SerializeField] private EnemyData[] enemyTypes;

    private List<Enemy> activeEnemies = new List<Enemy>();
    private bool isSpawning = false;

    public bool AllEnemiesRemoved { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartSpawning()
    {
        isSpawning = true;
        StartCoroutine(SpawnEnemies());
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    private IEnumerator SpawnEnemies()
    {
        while (isSpawning)
        {
            SpawnRandomEnemy();
            yield return new WaitForSeconds(spawnRate);
        }
    }

    private void SpawnRandomEnemy()
    {
        if (enemyTypes == null || enemyTypes.Length == 0) return;

        int randomIndex = Random.Range(0, enemyTypes.Length);
        EnemyData randomEnemyData = enemyTypes[randomIndex];

        // Use prefab from the EnemyData ScriptableObject
        if (randomEnemyData.enemyPrefab == null) return;

        Vector3 spawnPosition = GetRandomSpawnPosition();
        // Spawn via pool using the Enemy component so handler injection targets Enemy (caller)
        Enemy enemyPrefabComponent = randomEnemyData.enemyPrefab.GetComponent<Enemy>();
        if (enemyPrefabComponent == null) enemyPrefabComponent = randomEnemyData.enemyPrefab.AddComponent<Enemy>();
        Enemy enemyScript = PoolManager.Instance.Spawn(enemyPrefabComponent, spawnPosition, Quaternion.identity);

        if (enemyScript != null)
        {
            enemyScript.SetEnemyData(randomEnemyData);
            activeEnemies.Add(enemyScript);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        return new Vector3(randomCircle.x, randomCircle.y, 0);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    public void RemoveAllEnemies(float duration)
    {
        AllEnemiesRemoved = true;
        StartCoroutine(RemoveAllEnemiesCoroutine(duration));
    }

    private IEnumerator RemoveAllEnemiesCoroutine(float duration)
    {
        List<Enemy> enemiesToDestroy = new List<Enemy>(activeEnemies);

        foreach (Enemy enemy in enemiesToDestroy)
        {
            if (enemy != null)
            {
                IPoolable poolable = enemy.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    PoolManager.Instance.Despawn(poolable);
                }
                else
                {
                    Destroy(enemy.gameObject);
                }
            }
        }

        activeEnemies.Clear();

        yield return new WaitForSeconds(duration);

        AllEnemiesRemoved = false;
    }

    public int GetActiveEnemyCount()
    {
        return activeEnemies.Count;
    }
}

