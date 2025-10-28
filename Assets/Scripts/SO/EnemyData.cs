using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Tower Defense/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Visual")]
    public EnemyType enemyType;
    public GameObject enemyPrefab;
    public Color enemyColor;
    
    [Header("Stats")]
    [Range(1, 10)] public int speed = 1;
    [Range(1, 10)] public int attack = 1;
    [Range(1, 10)] public int shield = 1;
    [Range(1, 50)] public int health = 10;
    
    [Header("Damage to Tower")]
    [Range(1, 10)] public int damageDealt = 1;
}

