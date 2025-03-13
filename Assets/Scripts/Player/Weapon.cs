using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public string weaponName;
    public AmmoType ammoType;
    public float fireRate = 0.1f;
    public int damage = 10;
    public int clipSize = 30;
    public int maxAmmo = 120;
    public float reloadTime = 2f;

    [Header("UI Reference")]
    private PlayerUI playerUI; // NEW: Reference to PlayerUI

    [Header("Recoil Settings")]
    public float recoilAmount = 0.1f;
    public float recoilSpeed = 10f;

    [Header("References")]
    public GameObject muzzleFlashPrefab;
    public Transform muzzleFlashPoint;

    [Header("Ammo Management")]
    public int currentAmmo;
    public int totalAmmo;

    private float nextFireTime;
    private Vector3 originalPosition;

    private bool isReloading = false;

    private void Start()
    {
        currentAmmo = clipSize;
        totalAmmo = maxAmmo;

        originalPosition = transform.localPosition;

        InitializationManager initManager = FindObjectOfType<InitializationManager>();
        playerUI = initManager?.playerUI;

        if (playerUI == null)
        {
            Debug.LogError("❌ PlayerUI not found via InitializationManager!");
        }
    }

    public bool CanShoot()
    {
        return currentAmmo > 0 && Time.time >= nextFireTime && !isReloading;
    }

    public void Shoot()
    {
        if (!CanShoot()) return;

        currentAmmo--;
        nextFireTime = Time.time + fireRate;

        Debug.Log($"{weaponName} fired! Ammo left: {currentAmmo}");

        ShowMuzzleFlash();

        // Add Raycast Damage Logic
        RaycastHit hit;
        if (Physics.Raycast(muzzleFlashPoint.position, muzzleFlashPoint.forward, out hit, 100f))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage); // Apply weapon damage
                    Debug.Log($"Hit {hit.collider.gameObject.name} for {damage} damage.");
                }
            }
        }

    StartCoroutine(ApplyRecoil());
    }

    public void Reload()
    {
        if (isReloading) return;

        int ammoNeeded = clipSize - currentAmmo;
        if (totalAmmo <= 0 || ammoNeeded == 0) return;

        StartCoroutine(ReloadCoroutine(ammoNeeded));
    }

    private IEnumerator ReloadCoroutine(int ammoNeeded)
    {
        isReloading = true;

        if (playerUI != null)
        {
            playerUI.ShowReloadingText(true); // Show Reloading Text
        }

        Debug.Log($"{weaponName} is reloading...");

        yield return new WaitForSeconds(reloadTime);

        int ammoToReload = Mathf.Min(ammoNeeded, totalAmmo);
        totalAmmo -= ammoToReload;
        currentAmmo += ammoToReload;

        Debug.Log($"{weaponName} reloaded! Ammo in clip: {currentAmmo}, Total ammo: {totalAmmo}");

        if (playerUI != null)
        {
            playerUI.ShowReloadingText(false); // Hide Reloading Text
        }

        isReloading = false;
    }

    private IEnumerator ApplyRecoil()
    {
        Vector3 recoilPosition = originalPosition - new Vector3(0, 0, recoilAmount);
        float elapsedTime = 0f;

        // Recoil Kickback Phase
        while (elapsedTime < 0.05f) // Fast kickback for impactful recoil
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, recoilPosition, elapsedTime * recoilSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Smooth Recovery Phase
        elapsedTime = 0f;
        while (elapsedTime < 0.1f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, elapsedTime * recoilSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition; // Ensure final correction
    }


    public void AddAmmo(int amount)
    {
        if (IsAmmoFull()) return;

        totalAmmo = Mathf.Min(totalAmmo + amount, maxAmmo);
        Debug.Log($"{weaponName} collected {amount} ammo! Total ammo: {totalAmmo}");
    }

    public bool IsAmmoFull()
    {
        return totalAmmo >= maxAmmo;
    }

    private void ShowMuzzleFlash()
    {
        if (muzzleFlashPrefab && muzzleFlashPoint)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, muzzleFlashPoint.position, muzzleFlashPoint.rotation);
            Destroy(muzzleFlash, 0.1f);
        }
    }
}
