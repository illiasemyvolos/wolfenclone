using UnityEngine;

public class WeaponUtility : MonoBehaviour
{
    [HideInInspector] public Weapon weapon;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
        if (weapon == null)
        {
            Debug.LogError("❌ WeaponUtility: Weapon component not found!");
        }
    }

    public void MergeAmmo(int ammoAmount)
    {
        weapon.totalAmmo = Mathf.Min(weapon.totalAmmo + ammoAmount, weapon.weaponData.maxAmmo);
        Debug.Log($"{weapon.weaponData.weaponName} merged with {ammoAmount} ammo! Total ammo: {weapon.totalAmmo}");
    }

    public bool IsAmmoFull()
    {
        return weapon.totalAmmo >= weapon.weaponData.maxAmmo;
    }

    public void AddAmmo(int amount)
    {
        if (weapon.totalAmmo >= weapon.weaponData.maxAmmo) return;

        weapon.totalAmmo = Mathf.Min(weapon.totalAmmo + amount, weapon.weaponData.maxAmmo);
    }

    public void InitializeWeaponData(WeaponData data, int ammoAmount)
    {
        if (weapon == null)
        {
            Debug.LogError("❌ Weapon reference not set in WeaponUtility!");
            return;
        }

        weapon.weaponData = data;
        weapon.currentAmmo = weapon.weaponData.clipSize;
        weapon.totalAmmo = Mathf.Min(weapon.totalAmmo + ammoAmount, weapon.weaponData.maxAmmo);

        Debug.Log($"{weapon.weaponData.weaponName} initialized with {weapon.currentAmmo}/{weapon.totalAmmo} ammo.");
    }

    public void InitializeAmmoFromData()
    {
        weapon.currentAmmo = weapon.weaponData.clipSize;
        weapon.totalAmmo = weapon.weaponData.maxAmmo;
    }
}