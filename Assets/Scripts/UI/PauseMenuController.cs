using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem; // <-- add this
using System.Collections;

public class PauseMenuController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Gamepad Support")]
    [SerializeField] private GameObject firstSelectedButton;

    private InputAction pauseAction;

    private void OnEnable()
    {
        Debug.Log("☑️ PauseMenuController enabled. Hooking up buttons.");

        resumeButton.onClick.RemoveAllListeners();
        resumeButton.onClick.AddListener(OnResumeClicked);

        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(OnRestartClicked);

        mainMenuButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);

        // 🕹️ Setup Pause input action
        var playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput != null)
        {
            pauseAction = playerInput.actions["Pause"];
            if (pauseAction != null)
            {
                pauseAction.performed += OnPausePerformed;
                pauseAction.Enable();
            }
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
            pauseAction.performed -= OnPausePerformed;
    }

    private void Start()
    {
        UIManager.Instance.ShowPauseMenu(false);
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        if (GameStateManager.Instance.CurrentState == GameState.Gameplay)
        {
            PauseGame();
        }
        else if (GameStateManager.Instance.CurrentState == GameState.Paused)
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
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    private void ResumeGame()
    {
        StartCoroutine(ResumeAfterFrame());
    }

    private IEnumerator ResumeAfterFrame()
    {
        yield return null;

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