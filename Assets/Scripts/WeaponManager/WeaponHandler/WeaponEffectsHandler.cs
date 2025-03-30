using UnityEngine;
using System.Collections;

public class WeaponEffectsHandler : MonoBehaviour
{
    private Weapon weapon;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
        if (weapon == null)
        {
            Debug.LogError("❌ Weapon reference not found on WeaponEffectsHandler.");
        }
    }

    public void ShowMuzzleFlash()
    {
        if (weapon.weaponData.muzzleFlashPrefab && weapon.muzzleFlashPoint)
        {
            GameObject muzzleFlash = Instantiate(
                weapon.weaponData.muzzleFlashPrefab,
                weapon.muzzleFlashPoint.position,
                weapon.muzzleFlashPoint.rotation
            );

            muzzleFlash.transform.SetParent(weapon.muzzleFlashPoint);
            Destroy(muzzleFlash, 0.1f);
        }
        else
        {
            Debug.LogWarning("⚠️ Muzzle Flash Prefab or Muzzle Flash Point missing.");
        }
    }

    public void CreateBulletHole(RaycastHit hit)
    {
        if (weapon.weaponData.bulletHolePrefab != null)
        {
            GameObject bulletHole = Instantiate(
                weapon.weaponData.bulletHolePrefab,
                hit.point + (hit.normal * 0.01f),
                Quaternion.LookRotation(hit.normal)
            );

            bulletHole.transform.SetParent(hit.collider.transform);
            Destroy(bulletHole, weapon.weaponData.bulletHoleLifetime);
        }
    }

    public void PlayFireSound()
    {
        if (weapon.weaponData.fireSound != null && weapon.fireAudioSource != null)
        {
            weapon.fireAudioSource.PlayOneShot(weapon.weaponData.fireSound);
        }
    }

    public IEnumerator PlayShellDropDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (weapon.weaponData.dropShellsSound != null && weapon.weaponData.dropShellsSound.Length > 0)
        {
            AudioClip randomClip = weapon.weaponData.dropShellsSound[Random.Range(0, weapon.weaponData.dropShellsSound.Length)];
            weapon.dropShellsAudioSource.PlayOneShot(randomClip);
        }
    }

    public void PlayEmptyClick()
    {
        if (weapon.weaponData.emptyClickSound != null && weapon.emptyAudioSource != null)
        {
            weapon.emptyAudioSource.PlayOneShot(weapon.weaponData.emptyClickSound);
        }
    }

    public void PlayReloadSound()
    {
        if (weapon.weaponData.reloadSound != null && weapon.reloadAudioSource != null)
        {
            weapon.reloadAudioSource.PlayOneShot(weapon.weaponData.reloadSound);
        }
    }
}