using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IPoolable
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private float movementSpeed;
    [SerializeField] private int maxHealth = 10; // Unified health so shield controls hits to kill
    private int currentHealth;

    private Tower targetTower;
    private bool isDead = false;

    public EnemyData EnemyData => enemyData;

    private void Start()
    {
        targetTower = FindObjectOfType<Tower>();

        if (enemyData != null)
        {
            // Convert speed stat (1-10) to actual movement speed
            movementSpeed = enemyData.speed * 0.5f;

            // Set visual properties
            SetVisualProperties();
            maxHealth = Mathf.Max(1, enemyData.health);
        }

        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDead || targetTower == null) return;

        MoveTowardsTower();
        CheckReachedTower();
    }

    private void MoveTowardsTower()
    {
        Vector3 direction = (targetTower.transform.position - transform.position).normalized;
        transform.position += direction * movementSpeed * Time.deltaTime;

        // Optional: rotate towards tower
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
    }

    private void CheckReachedTower()
    {
        float distanceToTower = Vector3.Distance(transform.position, targetTower.transform.position);

        if (distanceToTower < 0.5f)
        {
            DealDamageToTower();
            Die();
        }
    }

    private void DealDamageToTower()
    {
        if (targetTower != null && enemyData != null)
        {
            // Use damageDealt directly (already set in ScriptableObject)
            targetTower.TakeDamage(enemyData.damageDealt);
        }
    }

    private Action<IPoolable> returnHandler;

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        EnemyManager.Instance?.RemoveEnemy(this);
        returnHandler?.Invoke(this);
    }

    public void TakeDamage(int baseDamage)
    {
        // Calculate actual damage based on this enemy's shield stat
        int actualDamage = CalculateDamageReceived(baseDamage, enemyData.shield);

        // Apply damage to health
        currentHealth -= Mathf.Max(1, actualDamage);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private int CalculateDamageReceived(int baseDamage, int shield)
    {
        // Higher shield = less damage taken
        // Shield (1-10) → Damage multiplier (1.0 - 0.0)
        float shieldMultiplier = 1f - ((shield - 1) / 9f);
        int actualDamage = Mathf.Max(1, Mathf.RoundToInt(baseDamage * shieldMultiplier));
        return actualDamage;
    }

    private void SetVisualProperties()
    {
        // Set color based on enemy data
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = enemyData.enemyColor;
        }
    }

    public void SetEnemyData(EnemyData data)
    {
        enemyData = data;
        if (enemyData != null)
        {
            movementSpeed = enemyData.speed * 0.5f;
            SetVisualProperties();
        }
    }

    // IPoolable implementation
    public void OnSpawned()
    {
        isDead = false;
        gameObject.SetActive(true);
        // reset health on spawn
        maxHealth = enemyData != null ? Mathf.Max(1, enemyData.health) : maxHealth;
        currentHealth = maxHealth;
    }

    public void OnDespawned()
    {
        gameObject.SetActive(false);
    }

    public void SetReturnHandler(Action<IPoolable> handler)
    {
        returnHandler = handler;
    }

    public Action<IPoolable> GetReturnHandler()
    {
        return returnHandler;
    }
}

