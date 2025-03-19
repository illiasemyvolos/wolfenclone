using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Pickup Settings")]
    public Weapon weaponPrefab;
    public int ammoAmount = 30;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WeaponManager weaponManager = other.GetComponentInChildren<WeaponManager>();

            if (weaponManager != null)
            {
                // ✅ Corrected from .Length to .Count for List<Weapon>
                if (weaponManager.weapons.Count == 0)
                {
                    weaponManager.AddOrMergeWeapon(weaponPrefab, ammoAmount);
                }
                else if (weaponManager.weapons.Count > 0)
                {
                    bool weaponExists = false;

                    foreach (Weapon weapon in weaponManager.weapons)
                    {
                        if (weapon.weaponName == weaponPrefab.weaponName)
                        {
                            weapon.MergeAmmo(ammoAmount);
                            weaponExists = true;
                            Debug.Log($"{weapon.weaponName} merged with {ammoAmount} ammo.");
                            break;
                        }
                    }

                    if (!weaponExists)
                    {
                        weaponManager.AddOrMergeWeapon(weaponPrefab, ammoAmount);
                        Debug.Log($"New weapon added: {weaponPrefab.weaponName} with {ammoAmount} ammo.");
                    }
                }

                Destroy(gameObject); // ✅ Destroy pickup after adding
            }
            else
            {
                Debug.LogWarning("❌ WeaponManager not found on player!");
            }
        }
    }
}