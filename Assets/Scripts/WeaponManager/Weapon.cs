using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Data Reference")]
    public WeaponData weaponData; // New Integration

    [Header("UI Reference")]
    private PlayerUI playerUI;

    [Header("Ammo Management")]
    public int currentAmmo;
    public int totalAmmo;
    
    [Header("Muzzle Flash Settings")]
    public Transform muzzleFlashPoint; // ✅ Reference for the muzzle flash position
    
    [Header("Input System Reference")]
    private PlayerInput playerInput;
    private InputAction fireAction;
    
    private float swayUpdateRate = 1f / 30f; // 30 FPS
    private float swayTimer = 0f;
    
    private float currentAccuracy;
    private float nextFireTime;
    private Vector3 originalPosition;
    private bool isReloading = false;
    private Coroutine reloadCoroutine;
    
    [Header("Weapon SoundSorce")]
    private AudioSource fireAudioSource;
    private AudioSource reloadAudioSource;
    private AudioSource emptyAudioSource;
    private AudioSource equipAudioSource;
    private AudioSource dropShellsAudioSource;

    private void Start()
    {
        if (weaponData == null)
        {
            Debug.LogError("❌ WeaponData not assigned to weapon: " + gameObject.name);
            return;
        }

        InitializeWeaponFromData();
        originalPosition = transform.localPosition;
        currentAccuracy = weaponData.accuracyAndRecoil.baseAccuracy;

        // ✅ Apply muzzle flash offset adjustment
        if (muzzleFlashPoint != null)
        {
            muzzleFlashPoint.localPosition += weaponData.muzzleFlashOffset;
            Debug.Log($"✅ Muzzle flash position adjusted by {weaponData.muzzleFlashOffset} for {weaponData.weaponName}");
        }
        else
        {
            Debug.LogWarning("⚠️ MuzzleFlashPoint not assigned — offset skipped.");
        }

        InitializationManager initManager = FindObjectOfType<InitializationManager>();
        playerUI = initManager?.playerUI;

        if (playerUI == null)
        {
            Debug.LogError("❌ PlayerUI not found via InitializationManager!");
        }
    }


    private void InitializeWeaponFromData()
    {
        currentAmmo = weaponData.clipSize;
        totalAmmo = weaponData.maxAmmo;
    }

    private void Awake()
    {
        playerInput = GetComponentInParent<PlayerInput>(); // Ensure this references the correct player
        fireAction = playerInput.actions["Player/Fire"];   // Match this to your Input System mapping
        fireAudioSource = transform.Find("AudioSource_Fire")?.GetComponent<AudioSource>();
        reloadAudioSource = transform.Find("AudioSource_Reload")?.GetComponent<AudioSource>();
        emptyAudioSource = transform.Find("AudioSource_Empty")?.GetComponent<AudioSource>();
        equipAudioSource = transform.Find("AudioSource_Equip")?.GetComponent<AudioSource>();
        dropShellsAudioSource = transform.Find("AudioSource_DropShells")?.GetComponent<AudioSource>();
    }

    private void Update()
    {
        RecoverAccuracy();
        ApplyWeaponSway(); // 🔄 Add sway logic to Update()

        // 🔥 Auto-Fire Logic with Input System
        if (weaponData.isAutoFire && fireAction.IsPressed()) 
        {
            if (CanShoot()) 
            {
                Shoot();
            }
        }
        else if (!weaponData.isAutoFire && fireAction.triggered)
        {
            if (CanShoot()) 
            {
                Shoot();
            }
        }
    }


    public bool CanShoot()
    {
        return currentAmmo > 0 && Time.time >= nextFireTime && !isReloading;
    }

    public void Shoot()
    {
        if (!CanShoot())
        {
            if (currentAmmo <= 0 && weaponData.emptyClickSound != null && emptyAudioSource != null)
            {
                emptyAudioSource.PlayOneShot(weaponData.emptyClickSound);
            }
            return;
        }

        currentAmmo--;
        nextFireTime = Time.time + weaponData.fireRate;

        ShowMuzzleFlash();

        // 🔊 Play fire sound here
        if (weaponData.fireSound != null && fireAudioSource != null)
        {
            fireAudioSource.PlayOneShot(weaponData.fireSound);
        }
        
        if (weaponData.dropShellsSound != null && dropShellsAudioSource != null)
        {
            StartCoroutine(PlayShellDropDelayed(0.5f));
        }

        if (weaponData.shotgunSettings.isShotgun)
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
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        RaycastHit hit;
        Vector3 shootDirection = ApplyAccuracyToDirection(mainCamera.transform.forward);

        // ✅ Added QueryTriggerInteraction.Ignore to skip trigger colliders
        if (Physics.Raycast(
                mainCamera.transform.position, 
                shootDirection, 
                out hit, 
                weaponData.fireRange, 
                ~0, 
                QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyHP enemyHP = hit.collider.GetComponent<EnemyHP>();
                if (enemyHP != null)
                {
                    enemyHP.TakeDamage((int)weaponData.damage); // 👈 Cast if damage is float
                    Debug.Log($"Hit {hit.collider.gameObject.name} for {weaponData.damage} damage.");
                }
            }
            else
            {
                CreateBulletHole(hit); // ✅ Now ignores trigger colliders
            }
        }

        ShowMuzzleFlash();
    }



    private void FireShotgunBlast()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        for (int i = 0; i < weaponData.shotgunSettings.pelletsPerShot; i++)
        {
            Vector3 pelletDirection = ApplyAccuracyToDirection(mainCamera.transform.forward);

            if (Physics.Raycast(
                    mainCamera.transform.position, 
                    pelletDirection, 
                    out RaycastHit hit, 
                    weaponData.fireRange, 
                    ~0, 
                    QueryTriggerInteraction.Ignore)) // ✅ Ignore trigger colliders
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    EnemyHP enemyHP = hit.collider.GetComponent<EnemyHP>();
                    if (enemyHP != null)
                    {
                        enemyHP.TakeDamage((int)(weaponData.damage / 2f)); // 👈 Half-damage per pellet
                        Debug.Log($"Shotgun pellet hit {hit.collider.gameObject.name} for {weaponData.damage / 2f} damage.");
                    }
                }
                else
                {
                    CreateBulletHole(hit); // ✅ Correct bullet hole placement
                }
            }
        }

        ShowMuzzleFlash();
    }
    
    private void CreateBulletHole(RaycastHit hit)
    {
        if (weaponData.bulletHolePrefab != null)
        {
            GameObject bulletHole = Instantiate(weaponData.bulletHolePrefab, hit.point + (hit.normal * 0.01f), Quaternion.LookRotation(hit.normal));
            bulletHole.transform.SetParent(hit.collider.transform);
            Destroy(bulletHole, weaponData.bulletHoleLifetime);
        }
    }

    private Vector3 ApplyAccuracyToDirection(Vector3 direction)
    {
        // Spread calculation using the dedicated spread multiplier
        float spread = (1f - currentAccuracy) * weaponData.spreadMultiplier * 10f;

        Quaternion spreadRotation = Quaternion.Euler(
            Random.Range(-spread, spread),
            Random.Range(-spread, spread),
            0f
        );

        return spreadRotation * direction;
    }



    private void ApplyAccuracyPenalty()
    {
        currentAccuracy = Mathf.Clamp(
            currentAccuracy - weaponData.accuracyAndRecoil.movementAccuracyPenalty,
            0.2f,
            weaponData.accuracyAndRecoil.baseAccuracy
        );

        
    }


    private void RecoverAccuracy()
    {
        if (currentAccuracy < weaponData.accuracyAndRecoil.baseAccuracy)
        {
            currentAccuracy = Mathf.Min(
                currentAccuracy + weaponData.accuracyAndRecoil.accuracyRecoveryRate * Time.deltaTime,
                weaponData.accuracyAndRecoil.baseAccuracy
            );
            
        }
    }


    public void Reload()
    {
        if (isReloading) return;

        int ammoNeeded = weaponData.clipSize - currentAmmo;
        if (totalAmmo <= 0 || ammoNeeded == 0) return;

        reloadCoroutine = StartCoroutine(ReloadCoroutine(ammoNeeded));
    }

    private IEnumerator ReloadCoroutine(int ammoNeeded)
    {
        isReloading = true;

        if (playerUI != null)
        {
            playerUI.ShowReloadingText(true);
        }
        
        if (weaponData.reloadSound != null && reloadAudioSource != null)
        {
            reloadAudioSource.PlayOneShot(weaponData.reloadSound);
        }
        
        yield return new WaitForSeconds(weaponData.reloadTime);

        int ammoToReload = Mathf.Min(ammoNeeded, totalAmmo);
        totalAmmo -= ammoToReload;
        currentAmmo += ammoToReload;

        if (playerUI != null)
        {
            playerUI.ShowReloadingText(false);
        }

        isReloading = false;
    }

// 🔄 New Method: Stop Reload Coroutine
    public void StopReload()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;

            if (playerUI != null)
            {
                playerUI.ShowReloadingText(false); // Ensure "reloading" text disappears
            }

            isReloading = false; // ✅ Ensure reloading state resets
            Debug.Log($"🛑 Reload canceled on {weaponData.weaponName}");
        }
    }

    private IEnumerator ApplyRecoil()
    {
        float recoilStrength = weaponData.accuracyAndRecoil.recoilAmount * 0.2f;
        Vector3 recoilPosition = originalPosition - new Vector3(0, 0, recoilStrength);

        float recoilDuration = 0.1f;
        float recoveryDuration = 0.15f;
        float frameDelay = 1f / 30f; // 30 FPS simulation

        float elapsedTime = 0f;

        // Recoil Phase (choppy movement)
        while (elapsedTime < recoilDuration)
        {
            float t = elapsedTime / recoilDuration;
            transform.localPosition = Vector3.Lerp(transform.localPosition, recoilPosition, t);
            elapsedTime += frameDelay;
            yield return new WaitForSeconds(frameDelay);
        }

        elapsedTime = 0f;

        // Recovery Phase (choppy return)
        while (elapsedTime < recoveryDuration)
        {
            float t = elapsedTime / recoveryDuration;
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, t);
            elapsedTime += frameDelay;
            yield return new WaitForSeconds(frameDelay);
        }

        transform.localPosition = originalPosition;
    }



    // ✅ Merges new ammo with the current ammo count
    public void MergeAmmo(int ammoAmount)
    {
        totalAmmo = Mathf.Min(totalAmmo + ammoAmount, weaponData.maxAmmo);
        Debug.Log($"{weaponData.weaponName} merged with {ammoAmount} ammo! Total ammo: {totalAmmo}");
    }

// ✅ Checks if the weapon's ammo is full
    public bool IsAmmoFull()
    {
        return totalAmmo >= weaponData.maxAmmo;
    }

// ✅ Initializes the weapon with specified data values
    public void InitializeWeaponData(WeaponData data, int ammoAmount)
    {
        weaponData = data;

        currentAmmo = weaponData.clipSize;
        totalAmmo = Mathf.Min(totalAmmo + ammoAmount, weaponData.maxAmmo);

        Debug.Log($"{weaponData.weaponName} initialized with {currentAmmo}/{totalAmmo} ammo.");
    }

    
    public void AddAmmo(int amount)
    {
        if (totalAmmo >= weaponData.maxAmmo) return;
        totalAmmo = Mathf.Min(totalAmmo + amount, weaponData.maxAmmo);
    }

    private void ShowMuzzleFlash()
    {
        if (weaponData.muzzleFlashPrefab && muzzleFlashPoint)
        {
            GameObject muzzleFlash = Instantiate(
                weaponData.muzzleFlashPrefab,         // Prefab reference
                muzzleFlashPoint.position,            // ✅ Exact position of muzzle flash point
                muzzleFlashPoint.rotation             // ✅ Same rotation as muzzle flash point
            );

            muzzleFlash.transform.SetParent(muzzleFlashPoint); // ✅ Optional: Attach for smoother control
            Destroy(muzzleFlash, 0.1f); // Auto-destroy after visual effect completes
        }
        else
        {
            Debug.LogWarning("⚠️ Muzzle Flash Prefab or Muzzle Flash Point missing.");
        }
    }
    
    private void ApplyWeaponSway()
    {
        swayTimer += Time.deltaTime;
        if (swayTimer < swayUpdateRate) return;
        swayTimer = 0f;

        float movementX = Input.GetAxis("Horizontal");
        float movementY = Input.GetAxis("Vertical");

        float swayX = Mathf.Sin(Time.time * weaponData.swaySpeed) * weaponData.swayAmount * movementX;
        float swayY = Mathf.Sin(Time.time * weaponData.swaySpeed * 0.5f) * weaponData.swayAmount * movementY;

        Vector3 targetPosition = new Vector3(swayX, swayY, 0);

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            originalPosition + targetPosition,
            Time.deltaTime * weaponData.swaySmoothness
        );
    }
    
    private IEnumerator PlayShellDropDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (weaponData.dropShellsSound != null && weaponData.dropShellsSound.Length > 0 && dropShellsAudioSource != null)
        {
            int index = Random.Range(0, weaponData.dropShellsSound.Length);
            dropShellsAudioSource.PlayOneShot(weaponData.dropShellsSound[index]);
        }
    }


}
