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
    public Image weaponIconImage;

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
        if (weaponManager == null || 
            weaponManager.weapons.Count == 0 || 
            weaponManager.currentWeaponIndex < 0 || 
            weaponManager.currentWeaponIndex >= weaponManager.weapons.Count)
        {
            if (weaponNameText != null) weaponNameText.text = "No Weapon";
            if (ammoText != null) ammoText.text = " 0 / 0";
            if (weaponIconImage != null) weaponIconImage.sprite = null;
            return;
        }

        Weapon currentWeapon = weaponManager.weapons[weaponManager.currentWeaponIndex];

        if (weaponNameText != null)
            weaponNameText.text = currentWeapon.weaponData.weaponName;

        if (ammoText != null)
            ammoText.text = $"{currentWeapon.currentAmmo} / {currentWeapon.totalAmmo}";

        if (weaponIconImage != null)
            weaponIconImage.sprite = currentWeapon.weaponData.weaponIcon;
    }

    public void ShowReloadingText(bool isReloading)
    {
        if (reloadingText != null)
        {
            reloadingText.gameObject.SetActive(isReloading);
        }
    }

    
}