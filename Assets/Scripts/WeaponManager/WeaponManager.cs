using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapons")]
    public List<Weapon> weapons = new List<Weapon>();  // Changed to List for dynamic expansion
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
            weapons[currentWeaponIndex].Shoot();
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
            if (weapon.ammoType == ammoType)
            {
                if (!weapon.IsAmmoFull())
                {
                    weapon.AddAmmo(amount);
                    Debug.Log($"{weapon.weaponName} received {amount} ammo.");
                    return true;  // ✅ Successfully added ammo
                }
                else
                {
                    Debug.Log($"{weapon.weaponName} ammo is already full.");
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
        Debug.Log($"Switched to {weapons[index].weaponName}");
    }

    // ✅ New Method: Add Weapon or Merge Ammo
    public void AddOrMergeWeapon(Weapon newWeaponPrefab, int ammoAmount)
    {
        // Check if the weapon already exists
        foreach (Weapon weapon in weapons)
        {
            if (weapon.weaponName == newWeaponPrefab.weaponName)
            {
                // Weapon exists — merge ammo
                weapon.MergeAmmo(ammoAmount);
                Debug.Log($"{weapon.weaponName} merged with {ammoAmount} ammo.");
                return;
            }
        }

        // ✅ Deactivate the current weapon before switching
        if (weapons.Count > 0)
        {
            weapons[currentWeaponIndex].gameObject.SetActive(false);
        }

        // Weapon does not exist — instantiate and add
        Weapon newWeaponInstance = Instantiate(
            newWeaponPrefab,
            weaponHolder.position,
            weaponHolder.rotation,
            weaponHolder
        );

        newWeaponInstance.InitializeWeaponData(
            newWeaponPrefab.weaponName,
            newWeaponPrefab.ammoType,
            newWeaponPrefab.clipSize,
            newWeaponPrefab.maxAmmo,
            ammoAmount
        );

        newWeaponInstance.transform.localPosition = Vector3.zero;
        newWeaponInstance.transform.localRotation = Quaternion.identity;

        newWeaponInstance.gameObject.SetActive(false); // Keep deactivated before switching
        weapons.Add(newWeaponInstance);

        // ✅ Switch to the new weapon automatically
        currentWeaponIndex = weapons.Count - 1;  // Update to the newly added weapon
        SelectWeapon(currentWeaponIndex);

        Debug.Log($"New weapon added: {newWeaponInstance.weaponName} with {ammoAmount} ammo.");
    }


}
