using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Settings")]
    public CanvasGroup pauseMenuCanvasGroup;
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;

    [Header("Input Settings")]
    public InputActionReference pauseAction;

    private bool isPaused = false;

    private void Awake()
    {
        if (pauseMenuCanvasGroup == null)
        {
            pauseMenuCanvasGroup = GetComponent<CanvasGroup>();

            if (pauseMenuCanvasGroup == null)
            {
                Debug.LogError("❌ CanvasGroup not found on PauseMenu! Add one to the GameObject.");
                return;
            }
        }

        // Ensure Pause Menu starts hidden
        HidePauseMenuInstantly();

        // Assign Button Logic
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);

        // Enable Gamepad Pause Action
        if (pauseAction != null)
        {
            pauseAction.action.performed += _ => TogglePause();
            pauseAction.action.Enable();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (pauseMenuCanvasGroup == null) return;

        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            ShowPauseMenu();
        }
    }

    public void ShowPauseMenu()
    {
        if (pauseMenuCanvasGroup == null) return;

        isPaused = true;
        Time.timeScale = 0f; 

        StartCoroutine(DelayCursorUnlock()); 
        StartCoroutine(FadeInPauseMenu());

        // Force focus on UI element
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }


    private IEnumerator DelayCursorUnlock()
    {
        yield return new WaitForSecondsRealtime(0.1f); 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator FadeInPauseMenu()
    {
        if (pauseMenuCanvasGroup == null) yield break;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            pauseMenuCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        pauseMenuCanvasGroup.interactable = true;
        pauseMenuCanvasGroup.blocksRaycasts = true;  // Ensures UI registers clicks
    }

    public void ResumeGame()
    {
        if (pauseMenuCanvasGroup == null) return;

        isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        HidePauseMenuInstantly();
    }

    private void HidePauseMenuInstantly()
    {
        if (pauseMenuCanvasGroup != null)
        {
            pauseMenuCanvasGroup.alpha = 0f;
            pauseMenuCanvasGroup.interactable = false;
            pauseMenuCanvasGroup.blocksRaycasts = false; // Prevents accidental clicks when hidden
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); 
    }
}
