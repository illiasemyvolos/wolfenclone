using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DeathScreenController : MonoBehaviour
{
    [Header("UI Settings")]
    public CanvasGroup deathScreenCanvasGroup;
    public Button restartButton;
    public Button mainMenuButton;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (deathScreenCanvasGroup == null)
        {
            deathScreenCanvasGroup = GetComponent<CanvasGroup>();

            if (deathScreenCanvasGroup == null)
            {
                Debug.LogError("❌ CanvasGroup not found on DeathScreen! Add one to the GameObject.");
                return;
            }
        }

        // Ensure DeathScreen starts hidden
        deathScreenCanvasGroup.alpha = 0f;
        gameObject.SetActive(false);

        // Link buttons if not assigned
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    public void ShowDeathScreen()
    {
        gameObject.SetActive(true); // Activate before coroutine starts
        Time.timeScale = 0f;        // Pause the game
        Cursor.lockState = CursorLockMode.None; // Unlock cursor
        Cursor.visible = true;      // Show cursor

        StartCoroutine(FadeInDeathScreen());
    }

    private IEnumerator FadeInDeathScreen()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;  // Important for animation while paused
            deathScreenCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        deathScreenCanvasGroup.interactable = true;
        deathScreenCanvasGroup.blocksRaycasts = true;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Resume game speed before restarting
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor back to game mode
        Cursor.visible = false; // Hide cursor for gameplay
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Resume game speed before returning
        Cursor.lockState = CursorLockMode.None; // Unlock cursor for menu
        Cursor.visible = true; // Show cursor for menu
        SceneManager.LoadScene("MainMenu");  // Ensure the scene name matches your Main Menu scene
    }
}
