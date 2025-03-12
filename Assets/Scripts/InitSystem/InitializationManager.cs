using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializationManager : MonoBehaviour
{
    [Header("Game References")]
    public PlayerController playerController;
    public WeaponManager weaponManager;
    public PlayerUI playerUI;

    private void Awake()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        if (weaponManager == null)
            weaponManager = FindObjectOfType<WeaponManager>();

        if (playerUI == null)
            playerUI = FindObjectOfType<PlayerUI>();

        if (playerController != null && playerUI != null)
        {
            playerUI.UpdateHealthUI();
        }

        Debug.Log("✅ Game Initialized Successfully.");
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
