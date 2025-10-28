using System;
using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 5f;

    private Vector2 direction;
    private Tower tower;
    private float despawnAt;
    private Action<IPoolable> returnHandler;

    public void Initialize(Vector2 shootDirection, Tower towerScript)
    {
        direction = shootDirection;
        tower = towerScript;
        despawnAt = Time.time + lifetime;

        // Rotate bullet sprite to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        if (Time.time >= despawnAt) returnHandler?.Invoke(this);

        MoveStraight();
        CheckHitEnemies();
    }

    private void MoveStraight()
    {
        // Move in straight direction
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void CheckHitEnemies()
    {
        // Check for collisions with enemies using overlap
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.2f);

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                HitEnemy(enemy);
                break;
            }
        }
    }

    private void HitEnemy(Enemy enemy)
    {
        if (enemy != null && tower != null)
        {
            int baseDamage = tower.GetBaseDamage();
            enemy.TakeDamage(baseDamage);
        }
        returnHandler?.Invoke(this);
    }

    // IPoolable implementation
    public void OnSpawned()
    {
        gameObject.SetActive(true);
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

