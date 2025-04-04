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
            foreach (var t in Resources.FindObjectsOfTypeAll<Transform>())
            {
                if (t.CompareTag("HUD"))
                {
                    hud = t.gameObject;
                    Debug.Log("✅ HUD auto-assigned: " + hud.name);
                    break;
                }
            }
        }

        if (pauseMenu == null)
        {
            foreach (var t in Resources.FindObjectsOfTypeAll<Transform>())
            {
                if (t.CompareTag("PauseMenu"))
                {
                    pauseMenu = t.gameObject;
                    Debug.Log("✅ PauseMenu auto-assigned: " + pauseMenu.name);
                    break;
                }
            }
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