using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class LevelFinishController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI enemiesKilledText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Gamepad Support")]
    [SerializeField] private GameObject firstSelectedButton;

    private void Start()
    {
        enemiesKilledText.text = $"Enemies Defeated: {GameStats.EnemiesKilled}";
        timeText.text = $"Time: {GameStats.LevelTime:F1}s";

        nextLevelButton.onClick.AddListener(OnNext);
        mainMenuButton.onClick.AddListener(OnMenu);

        GameStateManager.OnGameStateChanged += OnGameStateChanged;
        gameObject.SetActive(false);
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
            // Update stats
            enemiesKilledText.text = $"Enemies Defeated: {GameStats.EnemiesKilled}";
            timeText.text = $"Time: {GameStats.LevelTime:F1}s";

            // 🎮 Select default button
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }

    private void OnNext()
    {
        Debug.Log("▶️ LevelFinish: Next Level clicked");

        UIManager.Instance.ShowHUD(true);
        UIManager.Instance.ShowPauseMenu(false);

        InitializationManager.Instance.RestartScene();
    }

    private void OnMenu()
    {
        Debug.Log("🏠 LevelFinish: Main Menu clicked");

        GameStateManager.Instance.SetState(GameState.MainMenu);
    }
}