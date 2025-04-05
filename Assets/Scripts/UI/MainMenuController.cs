using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    [Header("Gamepad Support")]
    [SerializeField] private GameObject firstSelectedButton;

    private void Start()
    {
        startButton.onClick.AddListener(OnStartClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        // 🎮 Select Start on load for gamepad navigation
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    private void OnStartClicked()
    {
        Debug.Log("🟢 MainMenu: Start clicked");
        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    private void OnQuitClicked()
    {
        Debug.Log("❌ MainMenu: Quit clicked");
        Application.Quit();
    }
}