using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("Health UI")]
    public TextMeshProUGUI healthText;

    [Header("Armor UI")]
    public TextMeshProUGUI armorText;

    [Header("Weapon Info UI")]
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI ammoText;

    [Header("Reloading UI")]
    public TextMeshProUGUI reloadingText;

    [Header("Crosshair UI")]
    public GameObject crosshair;

    private PlayerController playerController;
    private WeaponManager weaponManager;

    private void Start()
    {
        InitializationManager initManager = FindObjectOfType<InitializationManager>();

        playerController = initManager?.playerController;
        weaponManager = initManager?.weaponManager;

        if (playerController == null)
        {
            Debug.LogError("❌ PlayerController not found via InitializationManager!");
        }

        if (weaponManager == null)
        {
            Debug.LogError("❌ WeaponManager not found via InitializationManager!");
        }

        if (reloadingText != null)
        {
            reloadingText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        UpdatePlayerStats();
        UpdateWeaponInfo();
    }

    // Combines Health and Armor updates into a single method for clarity
    public void UpdatePlayerStats()
    {
        if (playerController == null) return;

        if (healthText != null)
        {
            healthText.text = $"{playerController.GetCurrentHealth()}";
        }

        if (armorText != null)
        {
            armorText.text = $"{playerController.GetCurrentArmor()}";
        }
    }

    private void UpdateWeaponInfo()
    {
        if (weaponManager == null) return;

        Weapon currentWeapon = weaponManager.weapons[weaponManager.currentWeaponIndex];

        if (weaponNameText != null && ammoText != null)
        {
            weaponNameText.text = currentWeapon.weaponName;
            ammoText.text = $"{currentWeapon.currentAmmo} / {currentWeapon.totalAmmo}";
        }
    }

    public void ShowReloadingText(bool isReloading)
    {
        if (reloadingText != null)
        {
            reloadingText.gameObject.SetActive(isReloading);
        }
    }
}
