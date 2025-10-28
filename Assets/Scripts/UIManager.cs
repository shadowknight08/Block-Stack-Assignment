using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("Health Bar")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI healthText;
    
    [Header("Kill Them All Button")]
    [SerializeField] private Button killThemAllButton;
    
    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    
    private int killThemAllUses = 3;
    private Tower tower;
    
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
        tower = FindObjectOfType<Tower>();
        
        if (tower != null)
        {
            tower.OnHealthChanged += UpdateHealthBar;
            UpdateHealthBar(tower.CurrentHealth, tower.MaxHealth);
        }
        
        if (killThemAllButton != null)
        {
            killThemAllButton.onClick.AddListener(OnKillThemAllClicked);
        }
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
    
    private void OnDestroy()
    {
        if (tower != null)
        {
            tower.OnHealthChanged -= UpdateHealthBar;
        }
    }
    
    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
        
        if (healthText != null)
        {
            // Display health out of 50 for the UI
            float displayHealth = (currentHealth / (float)maxHealth) * 50f;
            healthText.text = $"Life: {Mathf.CeilToInt(displayHealth)}/50";
        }
    }
    
    private void OnKillThemAllClicked()
    {
        if (killThemAllUses > 0 && !GameManager.Instance.IsGameOver())
        {
            EnemyManager.Instance?.RemoveAllEnemies(5f);
            killThemAllUses--;
            
            if (killThemAllUses <= 0 && killThemAllButton != null)
            {
                killThemAllButton.interactable = false;
                killThemAllButton.GetComponentInChildren<TextMeshProUGUI>().text = "No Uses Left";
            }
            else if (killThemAllButton != null)
            {
                TextMeshProUGUI buttonText = killThemAllButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = $"Kill Them All ({killThemAllUses} uses left)";
                }
            }
        }
    }
    
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (gameOverText != null)
        {
            gameOverText.text = "Game Over";
        }
    }
}

