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
    public float fireRange = 50f;  

    [Header("Shotgun Settings")]
    public bool isShotgun = false;
    public int pelletsPerShot = 6;
    public float spreadAngle = 10f;

    [Header("Accuracy Settings")]
    public float baseAccuracy = 1f;
    public float movementAccuracyPenalty = 0.5f;
    public float recoveryRate = 0.1f;

    [Header("UI Reference")]
    private PlayerUI playerUI;

    [Header("Recoil Settings")]
    public float recoilAmount = 0.1f;
    public float recoilSpeed = 10f;

    [Header("References")]
    public GameObject muzzleFlashPrefab;
    public Transform muzzleFlashPoint;

    [Header("Ammo Management")]
    public int currentAmmo;
    public int totalAmmo;

    [Header("Bullet Hole System")]  // ✅ Added bullet hole system
    public GameObject bulletHolePrefab;
    public float bulletHoleLifetime = 10f;

    private float currentAccuracy;
    private float nextFireTime;
    private Vector3 originalPosition;
    private bool isReloading = false;

    private void Start()
    {
        currentAmmo = clipSize;
        totalAmmo = maxAmmo;

        originalPosition = transform.localPosition;
        currentAccuracy = baseAccuracy;

        InitializationManager initManager = FindObjectOfType<InitializationManager>();
        playerUI = initManager?.playerUI;

        if (playerUI == null)
        {
            Debug.LogError("❌ PlayerUI not found via InitializationManager!");
        }
    }

    private void Update()
    {
        RecoverAccuracy();
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

        ShowMuzzleFlash();

        if (isShotgun)
        {
            FireShotgunBlast();
        }
        else
        {
            FireSingleShot();
        }

        ApplyAccuracyPenalty();
        StartCoroutine(ApplyRecoil());
    }

    private void FireSingleShot()
    {
        RaycastHit hit;
        Vector3 directionWithSpread = ApplyAccuracyToDirection(muzzleFlashPoint.forward);

        if (Physics.Raycast(muzzleFlashPoint.position, directionWithSpread, out hit, fireRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                    Debug.Log($"Hit {hit.collider.gameObject.name} for {damage} damage.");
                }
            }
            else
            {
                CreateBulletHole(hit);  // ✅ Create bullet hole on non-enemy surfaces
            }
        }
    }

    private void FireShotgunBlast()
    {
        for (int i = 0; i < pelletsPerShot; i++)
        {
            Vector3 pelletDirection = ApplyAccuracyToDirection(muzzleFlashPoint.forward);

            if (Physics.Raycast(muzzleFlashPoint.position, pelletDirection, out RaycastHit hit, fireRange))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(damage / 2);
                        Debug.Log($"Shotgun pellet hit {hit.collider.gameObject.name} for {damage / 2} damage.");
                    }
                }
                else
                {
                    CreateBulletHole(hit);  // ✅ Add bullet hole for shotgun pellets too
                }
            }
        }
    }

    private void CreateBulletHole(RaycastHit hit)
    {
        if (bulletHolePrefab != null)
        {
            GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point + (hit.normal * 0.01f), Quaternion.LookRotation(hit.normal));
            bulletHole.transform.SetParent(hit.collider.transform); // Stick to the surface
            Destroy(bulletHole, bulletHoleLifetime);  // Auto-cleanup
        }
    }

    private Vector3 ApplyAccuracyToDirection(Vector3 direction)
    {
        float spread = (1f - currentAccuracy) * spreadAngle;

        Quaternion spreadRotation = Quaternion.Euler(
            Random.Range(-spread, spread),
            Random.Range(-spread, spread),
            0f
        );

        return spreadRotation * direction;
    }

    private void ApplyAccuracyPenalty()
    {
        currentAccuracy = Mathf.Clamp(currentAccuracy - 0.1f, 0.2f, baseAccuracy);
    }

    private void RecoverAccuracy()
    {
        if (currentAccuracy < baseAccuracy)
        {
            currentAccuracy = Mathf.Min(currentAccuracy + recoveryRate * Time.deltaTime, baseAccuracy);
        }
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
            playerUI.ShowReloadingText(true);
        }

        yield return new WaitForSeconds(reloadTime);

        int ammoToReload = Mathf.Min(ammoNeeded, totalAmmo);
        totalAmmo -= ammoToReload;
        currentAmmo += ammoToReload;

        if (playerUI != null)
        {
            playerUI.ShowReloadingText(false);
        }

        isReloading = false;
    }

    private IEnumerator ApplyRecoil()
    {
        Vector3 recoilPosition = originalPosition - new Vector3(0, 0, recoilAmount);
        float elapsedTime = 0f;

        while (elapsedTime < 0.05f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, recoilPosition, elapsedTime * recoilSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < 0.1f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, elapsedTime * recoilSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
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
