using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelFinishController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI enemiesKilledText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        // Initialize UI content
        enemiesKilledText.text = $"Enemies Defeated: {GameStats.EnemiesKilled}";
        timeText.text = $"Time: {GameStats.LevelTime:F1}s";

        nextLevelButton.onClick.AddListener(OnNext);
        mainMenuButton.onClick.AddListener(OnMenu);

        GameStateManager.OnGameStateChanged += OnGameStateChanged;
        gameObject.SetActive(false); // Hide by default
    }

    private void OnDestroy()
    {
        GameStateManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newState)
    {
        gameObject.SetActive(newState == GameState.Victory);

        if (newState == GameState.Victory)
        {
            // Refresh stats on state change
            enemiesKilledText.text = $"Enemies Defeated: {GameStats.EnemiesKilled}";
            timeText.text = $"Time: {GameStats.LevelTime:F1}s";
        }
    }

    private void OnNext()
    {
        // 🔁 Restart the scene cleanly using InitializationManager
        UIManager.Instance.ShowHUD(true);
        UIManager.Instance.ShowPauseMenu(false);

        InitializationManager.Instance.RestartScene();
    }

    private void OnMenu()
    {
        GameStateManager.Instance.SetState(GameState.MainMenu);
    }
}