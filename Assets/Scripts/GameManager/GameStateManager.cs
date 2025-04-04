using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    public static event Action<GameState> OnGameStateChanged;

    [Header("Scene Names")]
    [SerializeField] private string gameplayScene = "Level_01";
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string blackoutScene = "Blackout";
    [SerializeField] private string gameOverScene = "GameOverScreen";
    [SerializeField] private string levelFinishScene = "LevelFinishScreen";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetState(GameState newState)
    {
        if (newState == CurrentState) return;

        CurrentState = newState;
        OnGameStateChanged?.Invoke(CurrentState);

        switch (newState)
        {
            case GameState.MainMenu:
                StartCoroutine(TransitionToScene(mainMenuScene));
                break;

            case GameState.Gameplay:
                if (!SceneManager.GetSceneByName(gameplayScene).isLoaded)
                {
                    StartCoroutine(TransitionToScene(gameplayScene));
                }
                else
                {
                    Debug.Log("🟢 Gameplay scene already loaded — skipping reload");
                    OnGameStateChanged?.Invoke(CurrentState); // re-trigger listeners
                }
                break;

            case GameState.Victory:
                StartCoroutine(TransitionToScene(levelFinishScene));
                break;

            case GameState.GameOver:
                StartCoroutine(TransitionToScene(gameOverScene));
                break;
        }

        Time.timeScale = (newState == GameState.Paused) ? 0 : 1;

        if (newState == GameState.Gameplay)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private IEnumerator TransitionToScene(string targetScene)
    {
        // 1. Load blackout scene if not already loaded
        if (!SceneManager.GetSceneByName(blackoutScene).isLoaded)
            yield return SceneManager.LoadSceneAsync(blackoutScene, LoadSceneMode.Additive);

        // 2. Fade to black
        if (BlackoutController.Instance != null)
            yield return BlackoutController.Instance.FadeIn();

        // 3. Unload all non-persistent scenes (excluding GameManager & Blackout)
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.name != "GameManager" && scene.name != blackoutScene)
            {
                yield return SceneManager.UnloadSceneAsync(scene);
            }
        }

        // 4. Load the target scene
        if (!SceneManager.GetSceneByName(targetScene).isLoaded)
            yield return SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);

        // ✅ AUTO-ASSIGN UI REFERENCES (IMPORTANT FIX)
        if (UIManager.Instance != null)
        {
            UIManager.Instance.TryAutoFindUI();
            UIManager.Instance.ShowPauseMenu(false);
            UIManager.Instance.ShowHUD(CurrentState == GameState.Gameplay);
        }

        yield return new WaitForSeconds(0.05f); // Optional buffer

        // 5. Fade from black
        if (BlackoutController.Instance != null)
        {
            yield return BlackoutController.Instance.FadeOut();

            while (BlackoutController.Instance.IsFading)
                yield return null;
        }

        // 6. Unload blackout
        if (SceneManager.GetSceneByName(blackoutScene).isLoaded)
            yield return SceneManager.UnloadSceneAsync(blackoutScene);
    }
}