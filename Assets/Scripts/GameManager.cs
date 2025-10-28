using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private bool isGameOver = false;
    
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
    
    private void Start()
    {
        EnemyManager.Instance?.StartSpawning();
    }
    
    public void GameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        EnemyManager.Instance?.StopSpawning();
        
        // Trigger UI to show Game Over screen
        UIManager.Instance?.ShowGameOver();
    }
    
    public bool IsGameOver()
    {
        return isGameOver;
    }
}

