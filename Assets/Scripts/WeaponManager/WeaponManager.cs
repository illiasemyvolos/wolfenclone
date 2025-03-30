using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapons")]
    public List<Weapon> weapons = new List<Weapon>();
    public int currentWeaponIndex = 0;
    public Transform weaponHolder;

    [Header("Input Settings")]
    private PlayerInput playerInput;
    private InputAction fireAction;
    private InputAction reloadAction;
    private InputAction nextWeaponAction;
    private InputAction previousWeaponAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        fireAction = playerInput.actions["Player/Fire"];
        reloadAction = playerInput.actions["Player/Reload"];
        nextWeaponAction = playerInput.actions["Player/NextWeapon"];
        previousWeaponAction = playerInput.actions["Player/PreviousWeapon"];
    }

    private void Start()
    {
        InitializationManager initManager = FindObjectOfType<InitializationManager>();
        if (initManager == null)
        {
            Debug.LogError("❌ InitializationManager not found!");
            return;
        }

        if (weapons.Count > 0)
        {
            SelectWeapon(currentWeaponIndex);
        }
        else
        {
            Debug.LogWarning("⚠️ No starting weapons assigned in WeaponManager!");
        }
    }

    private void Update()
    {
        if (weapons.Count == 0) return;  // ✅ Prevent errors when no weapons exist

        if (fireAction.triggered)
        {
            weapons[currentWeaponIndex].HandleShoot();
        }

        if (reloadAction.triggered)
        {
            weapons[currentWeaponIndex].Reload();
        }

        if (nextWeaponAction.triggered)
        {
            SwitchWeapon(1);
        }

        if (previousWeaponAction.triggered)
        {
            SwitchWeapon(-1);
        }
    }

    public bool CollectAmmo(AmmoType ammoType, int amount)
    {
        foreach (Weapon weapon in weapons)
        {
            if (weapon.weaponData.ammoType == ammoType)
            {
                if (!weapon.IsAmmoFull())
                {
                    weapon.AddAmmo(amount);
                    Debug.Log($"{weapon.weaponData.weaponName} received {amount} ammo.");
                    return true;  // ✅ Successfully added ammo
                }
                else
                {
                    Debug.Log($"{weapon.weaponData.weaponName} ammo is already full.");
                    return false; // ❌ Ammo full, no pickup
                }
            }
        }

        Debug.LogWarning($"No matching weapon found for {ammoType} ammo.");
        return false; // ❌ No valid weapon found
    }

    void SwitchWeapon(int direction)
    {
        if (weapons.Count == 0) return; // ✅ Prevent switching error when no weapons exist

        // 🔄 Stop Reload on Current Weapon Before Switching
        weapons[currentWeaponIndex].StopReload();

        weapons[currentWeaponIndex].gameObject.SetActive(false);
        currentWeaponIndex += direction;

        if (currentWeaponIndex >= weapons.Count)
            currentWeaponIndex = 0;
        else if (currentWeaponIndex < 0)
            currentWeaponIndex = weapons.Count - 1;

        SelectWeapon(currentWeaponIndex);
    }


    void SelectWeapon(int index)
    {
        weapons[index].gameObject.SetActive(true);  // ✅ Ensure weapon activates
        Debug.Log($"Switched to {weapons[index].weaponData.weaponName}");
        
        Transform equipSource = weapons[index].transform.Find("AudioSource_Equip");
        if (equipSource && weapons[index].weaponData.equipSound != null &&
            equipSource.TryGetComponent(out AudioSource source))
        {
            source.PlayOneShot(weapons[index].weaponData.equipSound);
        }
    }

    // ✅ New Method: Add Weapon or Merge Ammo
    public void AddOrMergeWeapon(Weapon newWeaponPrefab, int ammoAmount)
    {
        // Merge ammo if weapon already exists
        foreach (Weapon weapon in weapons)
        {
            if (weapon.weaponData.weaponName == newWeaponPrefab.weaponData.weaponName)
            {
                weapon.MergeAmmo(ammoAmount);
                Debug.Log($"{weapon.weaponData.weaponName} merged with {ammoAmount} ammo.");
                return;
            }
        }

        // Deactivate current weapon
        if (weapons.Count > 0)
        {
            weapons[currentWeaponIndex].gameObject.SetActive(false);
        }

        // Instantiate and set up new weapon
        Weapon newWeaponInstance = Instantiate(
            newWeaponPrefab,
            weaponHolder.position,
            weaponHolder.rotation,
            weaponHolder
        );

        // ✅ Manually cache all handler references immediately
        newWeaponInstance.CacheHandlers(); // 🔥 THIS FIXES THE NULL EXCEPTION

        // Initialize ammo and data
        newWeaponInstance.InitializeWeaponData(newWeaponPrefab.weaponData, ammoAmount);

        // Reset transform
        newWeaponInstance.transform.localPosition = Vector3.zero;
        newWeaponInstance.transform.localRotation = Quaternion.identity;
        newWeaponInstance.gameObject.SetActive(false);

        weapons.Add(newWeaponInstance);
        currentWeaponIndex = Mathf.Clamp(weapons.Count - 1, 0, weapons.Count - 1);
        SelectWeapon(currentWeaponIndex);

        Debug.Log($"New weapon added: {newWeaponInstance.weaponData.weaponName} with {ammoAmount} ammo.");
    }
}
