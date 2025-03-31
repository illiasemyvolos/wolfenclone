using System;
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
            // Optional: persist across scenes
            // DontDestroyOnLoad(gameObject);

            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGame()
    {
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
            playerUI.UpdatePlayerStats(); // Update UI with initial stats
        }

        Debug.Log($"✅ Initialization Summary:\n" +
                  $"- PlayerController: {(playerController != null ? "OK" : "Missing")}\n" +
                  $"- WeaponManager: {(weaponManager != null ? "OK" : "Missing")}\n" +
                  $"- PlayerUI: {(playerUI != null ? "OK" : "Missing")}");

        OnGameInitialized?.Invoke();
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}