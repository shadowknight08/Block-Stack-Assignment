using UnityEngine;
using PrimeTween;

public class Tower : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Shooting")]
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float aimTolerance = 5f; // Degrees tolerance for aiming

    [Header("Damage Configuration")]
    [SerializeField] private int baseDamage = 10;

    [Header("Feedback")]
    [SerializeField, Range(0f, 1f)] private float cameraShakeStrength = 0.4f;

    private float lastFireTime;
    private Quaternion targetRotation;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    public event System.Action<int, int> OnHealthChanged;

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Update()
    {
        Enemy nearestEnemy = FindNearestEnemy();

        if (nearestEnemy != null)
        {
            // Continuously rotate towards nearest enemy
            RotateTowardsTarget(nearestEnemy.transform);

            // Only shoot when fire rate timer is up AND tower is aimed at target
            if (Time.time >= lastFireTime + fireRate && IsAimed())
            {
                Shoot(nearestEnemy);
                lastFireTime = Time.time;
            }
        }
        else
        {
            // Still increment timer even if no enemies
            if (Time.time >= lastFireTime + fireRate)
            {
                lastFireTime = Time.time;
            }
        }
    }

    private void Shoot(Enemy target)
    {
        // Get the direction the tower is facing (local Y axis = transform.up)
        Vector2 shootDirection = transform.up;

        // Spawn bullet from pool using Bullet component on the prefab so return handlers bind to Bullet
        Bullet bulletPrefabComponent = bulletPrefab.GetComponent<Bullet>();
        if (bulletPrefabComponent == null) bulletPrefabComponent = bulletPrefab.AddComponent<Bullet>();
        Bullet bulletScript = PoolManager.Instance.Spawn(bulletPrefabComponent, shootPoint.position, Quaternion.identity);
        bulletScript.Initialize(shootDirection, this);
    }

    private void RotateTowardsTarget(Transform target)
    {
        // Get direction to target in 2D space
        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;

        // Calculate the angle to rotate around Z-axis so local Y points toward target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f + 180f; // +90 +180 to correct direction

        // Store target rotation
        targetRotation = Quaternion.Euler(0, 0, angle);

        // Smoothly rotate towards target
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private bool IsAimed()
    {
        // Check if tower rotation is close to target rotation
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
        return angleDifference <= aimTolerance;
    }

    private Enemy FindNearestEnemy()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        if (enemies.Length == 0) return null;

        Enemy nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (Enemy enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }

    /// <summary>
    /// Returns the base damage the tower deals to enemies
    /// </summary>
    public int GetBaseDamage()
    {
        return baseDamage;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (Camera.main != null)
        {
            Tween.ShakeCamera(Camera.main, cameraShakeStrength);
        }

        if (currentHealth <= 0)
        {
            GameManager.Instance?.GameOver();
        }
    }
}

