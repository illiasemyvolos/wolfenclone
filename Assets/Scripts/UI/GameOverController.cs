using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameOverController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;

    [Header("UI Panel")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Gamepad Support")]
    [SerializeField] private GameObject firstSelectedButton;

    private void Start()
    {
        Debug.Log("🟢 GameOverController Start");

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(OnRetry);
            Debug.Log("✅ RetryButton listener attached.");
        }
        else
        {
            Debug.LogWarning("❌ RetryButton reference missing.");
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenu);
            Debug.Log("✅ MainMenuButton listener attached.");
        }
        else
        {
            Debug.LogWarning("❌ MainMenuButton reference missing.");
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            Debug.Log("🔻 GameOverPanel deactivated at Start.");
        }
        else
        {
            Debug.LogWarning("❌ GameOverPanel is not assigned!");
        }

        GameStateManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameStateManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newState)
    {
        Debug.Log($"📣 GameState changed to: {newState}");

        bool isGameOver = newState == GameState.GameOver;

        if (gameOverPanel == null)
        {
            Debug.LogWarning("⚠️ GameOverPanel is NULL during OnGameStateChanged!");
            return;
        }

        gameOverPanel.SetActive(isGameOver);
        Debug.Log($"🔁 GameOverPanel set active = {isGameOver}");

        if (isGameOver)
        {
            Debug.Log("💀 GameOverController: Activating UI for GameOver");

            if (!gameObject.activeSelf)
                Debug.LogWarning("⚠️ GameOverController GameObject is not active!");

            if (!gameOverPanel.activeSelf)
                Debug.LogWarning("⚠️ GameOverPanel GameObject is still inactive after SetActive(true)!");

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (firstSelectedButton != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(firstSelectedButton);
                Debug.Log("🎮 Gamepad selected button set.");
            }
            else
            {
                Debug.LogWarning("❌ FirstSelectedButton is not assigned!");
            }
        }
    }

    private void OnRetry()
    {
        Debug.Log("🔁 GameOver: Retry clicked");

        UIManager.Instance.ShowPauseMenu(false);
        UIManager.Instance.ShowHUD(true);
        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    private void OnMainMenu()
    {
        Debug.Log("🏠 GameOver: Main Menu clicked");

        UIManager.Instance.ShowPauseMenu(false);
        UIManager.Instance.ShowHUD(false);
        GameStateManager.Instance.SetState(GameState.MainMenu);
    }
}