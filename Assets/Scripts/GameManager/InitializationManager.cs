using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializationManager : MonoBehaviour
{
    public static InitializationManager Instance { get; private set; }

    [Header("Game References")]
    public PlayerController playerController;
    public WeaponManager weaponManager;
    public PlayerUI playerUI;

    public static event Action OnGameInitialized;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeGame(); // Re-run on scene reload (e.g., retry)
    }

    private void InitializeGame()
    {
        // Link core references
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
            if (playerController == null)
                Debug.LogWarning("⚠️ PlayerController not found in the scene!");
        }

        if (weaponManager == null)
        {
            weaponManager = FindObjectOfType<WeaponManager>();
            if (weaponManager == null)
                Debug.LogWarning("⚠️ WeaponManager not found in the scene!");
        }

        if (playerUI == null)
        {
            playerUI = FindObjectOfType<PlayerUI>();
            if (playerUI == null)
                Debug.LogWarning("⚠️ PlayerUI not found in the scene!");
        }

        if (playerController != null && playerUI != null)
        {
            playerUI.UpdatePlayerStats();
        }

        // Debug summary
        Debug.Log($"✅ Initialization Summary:\n" +
                  $"- PlayerController: {(playerController != null ? "OK" : "Missing")}\n" +
                  $"- WeaponManager: {(weaponManager != null ? "OK" : "Missing")}\n" +
                  $"- PlayerUI: {(playerUI != null ? "OK" : "Missing")}");

        // Reconnect and update UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.TryAutoFindUI();
            UIManager.Instance.ShowHUD(true);
            UIManager.Instance.ShowPauseMenu(false);
        }

        OnGameInitialized?.Invoke();
    }

    public void RestartScene()
    {
        StartCoroutine(RestartSceneRoutine());
    }

    private IEnumerator RestartSceneRoutine()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // Clear stale UI references before reloading
        UIManager.Instance?.ClearUIReferences();

        // 1. Unload current scene
        yield return SceneManager.UnloadSceneAsync(currentScene);

        // 2. Reload it
        yield return SceneManager.LoadSceneAsync(currentScene, LoadSceneMode.Additive);

        // 3. Set back to gameplay
        GameStateManager.Instance.SetState(GameState.Gameplay);
    }
}