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
        enemiesKilledText.text = $"Enemies Defeated: {GameStats.EnemiesKilled}";
        timeText.text = $"Time: {GameStats.LevelTime:F1}s";

        nextLevelButton.onClick.AddListener(OnNext);
        mainMenuButton.onClick.AddListener(OnMenu);
    }
    private void OnNext()
    {
        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    private void OnMenu()
    {
        GameStateManager.Instance.SetState(GameState.MainMenu);
    }
}