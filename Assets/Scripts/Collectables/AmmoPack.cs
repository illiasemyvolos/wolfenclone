using UnityEngine;

public class AmmoPack : MonoBehaviour
{
    [Header("Ammo Pack Settings")]
    public AmmoType ammoType;     // Ammo type this pack restores
    public int ammoAmount = 30;   // Amount of ammo restored

    private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        WeaponManager weaponManager = other.GetComponentInChildren<WeaponManager>();
        if (weaponManager != null)
        {
            bool ammoAdded = weaponManager.CollectAmmo(ammoType, ammoAmount);

            if (ammoAdded)  // ✅ Only destroy the pack if ammo was added
            {
                Destroy(gameObject);
            }
            else
            {
                Debug.Log($"{ammoType} ammo is already full. AmmoPack NOT destroyed.");
            }
        }
    }
}

}
