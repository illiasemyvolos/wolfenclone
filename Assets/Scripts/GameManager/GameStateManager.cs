using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }
    public event Action<GameState> OnGameStateChanged;

    [Header("Scene Names")]
    [SerializeField] private string gameplayScene = "Level_01";
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string blackoutScene = "Blackout";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
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
                StartCoroutine(TransitionToScene(gameplayScene));
                break;
            case GameState.Victory:
                // Add VictoryScene name if needed
                break;
            case GameState.GameOver:
                // Add GameOverScene name if needed
                break;
        }

        Time.timeScale = (newState == GameState.Paused) ? 0 : 1;
    }

    private IEnumerator TransitionToScene(string targetScene)
    {
        // Load blackout scene
        if (!SceneManager.GetSceneByName(blackoutScene).isLoaded)
            yield return SceneManager.LoadSceneAsync(blackoutScene, LoadSceneMode.Additive);

        // Fade to black
        yield return BlackoutController.Instance.FadeIn();

        // Unload all non-persistent scenes (except GameManager)
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.name != "GameManager" && scene.name != blackoutScene)
                yield return SceneManager.UnloadSceneAsync(scene);
        }

        // Load target scene
        if (!SceneManager.GetSceneByName(targetScene).isLoaded)
            yield return SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);

        // Fade from black
        yield return BlackoutController.Instance.FadeOut();

        // Unload blackout
        yield return SceneManager.UnloadSceneAsync(blackoutScene);
    }
}