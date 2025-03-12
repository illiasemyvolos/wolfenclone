using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapons")]
    public Weapon[] weapons;
    public int currentWeaponIndex = 0;

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

        SelectWeapon(currentWeaponIndex);
    }

    private void Update()
    {
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
                if (!weapon.IsAmmoFull())  // Only collect if total ammo is not full
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
        weapons[currentWeaponIndex].gameObject.SetActive(false);
        currentWeaponIndex += direction;

        if (currentWeaponIndex >= weapons.Length)
            currentWeaponIndex = 0;
        else if (currentWeaponIndex < 0)
            currentWeaponIndex = weapons.Length - 1;

        SelectWeapon(currentWeaponIndex);
    }

    void SelectWeapon(int index)
    {
        weapons[index].gameObject.SetActive(true);
        Debug.Log($"Switched to {weapons[index].weaponName}");
    }

    
}
