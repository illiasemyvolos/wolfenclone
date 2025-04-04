using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private void OnEnable()
    {
        Debug.Log("☑️ PauseMenuController enabled. Hooking up buttons.");

        resumeButton.onClick.RemoveAllListeners();
        resumeButton.onClick.AddListener(OnResumeClicked);

        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(OnRestartClicked);

        mainMenuButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    private void Start()
    {
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
        Debug.Log("⏸ PauseMenuController: Game Paused");

        GameStateManager.Instance.SetState(GameState.Paused);
        UIManager.Instance.ShowPauseMenu(true);
        UIManager.Instance.ShowHUD(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        EventSystem.current.SetSelectedGameObject(null);
    }

    private void ResumeGame()
    {
        StartCoroutine(ResumeAfterFrame());
    }

    private IEnumerator ResumeAfterFrame()
    {
        yield return null; // Let Unity process the UI click before gameplay input resumes

        Debug.Log("▶️ PauseMenuController: Resume button clicked");

        GameStateManager.Instance.SetState(GameState.Gameplay);
        UIManager.Instance.ShowPauseMenu(false);
        UIManager.Instance.ShowHUD(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnResumeClicked()
    {
        Debug.Log("🟢 Resume button pressed");
        ResumeGame();
    }

    private void OnRestartClicked()
    {
        Debug.Log("🔁 Restart button clicked");

        Time.timeScale = 1f;
        UIManager.Instance.ShowPauseMenu(false);
        GameStateManager.Instance.SetState(GameState.Loading);
        InitializationManager.Instance.RestartScene();
    }

    private void OnMainMenuClicked()
    {
        Debug.Log("🏠 Main Menu button clicked");

        Time.timeScale = 1f;
        UIManager.Instance.ShowPauseMenu(false);
        GameStateManager.Instance.SetState(GameState.MainMenu);
    }
}