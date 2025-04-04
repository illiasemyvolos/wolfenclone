using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);

        // Force-hide on load
        UIManager.Instance.ShowPauseMenu(false);
    }

    private void Update()
    {
        if (GameStateManager.Instance.CurrentState == GameState.Gameplay &&
            Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
        else if (GameStateManager.Instance.CurrentState == GameState.Paused &&
                 Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        GameStateManager.Instance.SetState(GameState.Paused);
        UIManager.Instance.ShowPauseMenu(true);
        UIManager.Instance.ShowHUD(false);
    }

    private void ResumeGame()
    {
        GameStateManager.Instance.SetState(GameState.Gameplay);
        UIManager.Instance.ShowPauseMenu(false);
        UIManager.Instance.ShowHUD(true);
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        UIManager.Instance.ShowPauseMenu(false);
        GameStateManager.Instance.SetState(GameState.Loading);
        InitializationManager.Instance.RestartScene();
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        UIManager.Instance.ShowPauseMenu(false);
        GameStateManager.Instance.SetState(GameState.MainMenu);
    }
}