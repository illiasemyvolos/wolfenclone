using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        retryButton.onClick.AddListener(OnRetry);
        mainMenuButton.onClick.AddListener(OnMainMenu);

        // Listen for GameState changes
        GameStateManager.OnGameStateChanged += OnGameStateChanged;
        gameObject.SetActive(false); // Hide by default
    }

    private void OnDestroy()
    {
        GameStateManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newState)
    {
        gameObject.SetActive(newState == GameState.GameOver);
    }

    private void OnRetry()
    {
        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    private IEnumerator ReloadCurrentLevel()
    {
        GameStateManager.Instance.SetState(GameState.Loading);

        string currentLevel = "Level_01"; // You can make this dynamic later
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        yield return SceneManager.LoadSceneAsync(currentLevel, LoadSceneMode.Additive);

        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    private void OnMainMenu()
    {
        GameStateManager.Instance.SetState(GameState.MainMenu);
    }
}