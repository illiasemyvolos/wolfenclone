using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Pickup Settings")]
    public Weapon weaponPrefab;
    public WeaponData weaponData; // ✅ Integrated WeaponData
    public int ammoAmount = 30;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        WeaponManager weaponManager = other.GetComponentInChildren<WeaponManager>();

        if (weaponManager != null)
        {
            if (weaponManager.weapons.Count == 0)
            {
                weaponManager.AddOrMergeWeapon(weaponPrefab, ammoAmount);
            }
            else
            {
                bool weaponExists = false;

                foreach (Weapon weapon in weaponManager.weapons)
                {
                    if (weapon.weaponData != null && weapon.weaponData.weaponName == weaponData.weaponName)
                    {
                        weapon.MergeAmmo(ammoAmount);
                        weaponExists = true;
                        Debug.Log($"{weaponData.weaponName} merged with {ammoAmount} ammo.");
                        break;
                    }
                }

                if (!weaponExists)
                {
                    weaponManager.AddOrMergeWeapon(weaponPrefab, ammoAmount);
                    Debug.Log($"New weapon added: {weaponData.weaponName} with {ammoAmount} ammo.");
                }
            }

            Destroy(gameObject); // ✅ Destroy pickup after successful add/merge
        }
        else
        {
            Debug.LogWarning("❌ WeaponManager not found on player!");
        }
    }
}