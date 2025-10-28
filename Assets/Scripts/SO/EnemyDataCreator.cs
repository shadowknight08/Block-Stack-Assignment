using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyDataCreator : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Create Enemy Data Assets")]
    public static void CreateEnemyDataAssets()
    {
        CreateEnemyData(EnemyType.Square, 3, 4, 2, 5, new Color(0.2f, 0.2f, 0.2f));
        CreateEnemyData(EnemyType.Star, 10, 2, 1, 2, new Color(0.1f, 0.1f, 0.5f));
        CreateEnemyData(EnemyType.Triangle, 1, 1, 10, 1, new Color(0.9f, 0.8f, 0.7f));
    }
    
    static void CreateEnemyData(EnemyType type, int speed, int attack, int shield, int damage, Color color)
    {
        EnemyData data = ScriptableObject.CreateInstance<EnemyData>();
        data.enemyType = type;
        data.speed = speed;
        data.attack = attack;
        data.shield = shield;
        data.damageDealt = damage;
        data.enemyColor = color;
        
        string path = $"Assets/SO_Enemy{type}.asset";
        AssetDatabase.CreateAsset(data, path);
        AssetDatabase.SaveAssets();
    }
#endif
}

