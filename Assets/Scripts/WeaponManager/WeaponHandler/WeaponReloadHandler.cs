using UnityEngine;
using System.Collections;

public class WeaponReloadHandler : MonoBehaviour
{
    public bool IsReloading => reloadCoroutine != null;
    private Weapon weapon;
    private Coroutine reloadCoroutine;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
        if (weapon == null)
        {
            Debug.LogError("❌ Weapon reference not found on WeaponReloadHandler.");
        }
    }

    public void Reload()
    {
        if (reloadCoroutine != null || weapon.totalAmmo <= 0 || weapon.currentAmmo == weapon.weaponData.clipSize)
            return;

        int ammoNeeded = weapon.weaponData.clipSize - weapon.currentAmmo;
        reloadCoroutine = StartCoroutine(ReloadCoroutine(ammoNeeded));
    }

    private IEnumerator ReloadCoroutine(int ammoNeeded)
    {
        if (weapon.playerUI != null)
        {
            weapon.playerUI.ShowReloadingText(true);
        }

        if (weapon.weaponData.reloadSound != null && weapon.reloadAudioSource != null)
        {
            weapon.reloadAudioSource.PlayOneShot(weapon.weaponData.reloadSound);
        }

        yield return new WaitForSeconds(weapon.weaponData.reloadTime);

        int ammoToReload = Mathf.Min(ammoNeeded, weapon.totalAmmo);
        weapon.totalAmmo -= ammoToReload;
        weapon.currentAmmo += ammoToReload;

        if (weapon.playerUI != null)
        {
            weapon.playerUI.ShowReloadingText(false);
        }

        reloadCoroutine = null;
    }

    public void StopReload()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;

            if (weapon.playerUI != null)
            {
                weapon.playerUI.ShowReloadingText(false);
            }

            Debug.Log($"🛑 Reload canceled on {weapon.weaponData.weaponName}");
        }
    }
}