using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Screens")]
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject pauseMenu;

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

    private void Start()
    {
        TryAutoFindUI();
    }

    public void ShowHUD(bool show)
    {
        if (hud == null)
        {
            Debug.LogWarning("⚠️ HUD reference is null. Attempting to re-link...");
            TryAutoFindUI();
        }

        if (hud != null)
        {
            hud.SetActive(show);
        }
    }

    public void ShowPauseMenu(bool show)
    {
        if (pauseMenu == null)
        {
            Debug.LogWarning("⚠️ PauseMenu reference is null. Attempting to re-link...");
            TryAutoFindUI();
        }

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(show);
        }
    }

    public void TryAutoFindUI()
    {
        if (hud == null)
        {
            var hudObj = GameObject.FindWithTag("HUD");
            if (hudObj != null) hud = hudObj;
        }

        if (pauseMenu == null)
        {
            var pauseObj = GameObject.FindWithTag("PauseMenu");
            if (pauseObj != null) pauseMenu = pauseObj;
        }

        // 🛑 Critical fix: make sure pause menu is hidden!
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
            Debug.Log("✅ UIManager: PauseMenu auto-found and hidden.");
        }
    }

    public void ClearUIReferences()
    {
        hud = null;
        pauseMenu = null;
    }
}