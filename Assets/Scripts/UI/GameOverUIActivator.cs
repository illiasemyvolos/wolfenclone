using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOverUIActivator : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button firstSelectedButton;

    private void OnEnable()
    {
        GameStateManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.GameOver)
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true); // ✅ Reactivate the disabled UI
                Debug.Log("✅ GameOverUIActivator: GameOver panel activated");
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (firstSelectedButton != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
            }
        }
    }
}