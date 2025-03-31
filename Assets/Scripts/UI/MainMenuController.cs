using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        startButton.onClick.AddListener(OnStartClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnStartClicked()
    {
        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    private void OnQuitClicked()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}