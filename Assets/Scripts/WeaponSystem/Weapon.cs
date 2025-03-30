using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Data Reference")]
    public WeaponData weaponData;

    [Header("UI Reference")]
    public PlayerUI playerUI;

    [Header("Ammo Management")]
    public int currentAmmo { get; set; }
    public int totalAmmo;

    [Header("Muzzle Flash Settings")]
    public Transform muzzleFlashPoint;

    [Header("Input System Reference")]
    private PlayerInput playerInput;

    [Header("Weapon SoundSource")]
    public AudioSource fireAudioSource;
    public AudioSource reloadAudioSource;
    public AudioSource emptyAudioSource;
    public AudioSource equipAudioSource;
    public AudioSource dropShellsAudioSource;

    [HideInInspector] public float nextFireTime;

    // Handlers
    private WeaponShootHandler shootHandler;
    private WeaponAccuracyHandler accuracyHandler;
    private WeaponEffectsHandler effectsHandler;
    private WeaponRecoilHandler recoilHandler;
    private WeaponReloadHandler reloadHandler;
    private WeaponInputHandler inputHandler;
    private WeaponUtility utilityHandler;

    private float swayUpdateRate = 1f / 30f;
    private float swayTimer = 0f;
    private Vector3 originalPosition;

    private void Awake()
    {
        // Get references
        playerInput = GetComponentInParent<PlayerInput>();
        fireAudioSource = transform.Find("AudioSource_Fire")?.GetComponent<AudioSource>();
        reloadAudioSource = transform.Find("AudioSource_Reload")?.GetComponent<AudioSource>();
        emptyAudioSource = transform.Find("AudioSource_Empty")?.GetComponent<AudioSource>();
        equipAudioSource = transform.Find("AudioSource_Equip")?.GetComponent<AudioSource>();
        dropShellsAudioSource = transform.Find("AudioSource_DropShells")?.GetComponent<AudioSource>();

        // Get handler scripts
        shootHandler    = GetComponent<WeaponShootHandler>();
        accuracyHandler = GetComponent<WeaponAccuracyHandler>();
        effectsHandler  = GetComponent<WeaponEffectsHandler>();
        recoilHandler   = GetComponent<WeaponRecoilHandler>();
        reloadHandler   = GetComponent<WeaponReloadHandler>();
        inputHandler    = GetComponent<WeaponInputHandler>();
        utilityHandler  = GetComponent<WeaponUtility>();

        originalPosition = transform.localPosition;
    }

    private void Start()
    {
        if (weaponData == null)
        {
            Debug.LogError("❌ WeaponData not assigned to weapon: " + gameObject.name);
            return;
        }

        utilityHandler.InitializeAmmoFromData();
        currentAccuracy = weaponData.accuracyAndRecoil.baseAccuracy;

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

    public void CacheHandlers()
    {
        shootHandler    = GetComponent<WeaponShootHandler>();
        accuracyHandler = GetComponent<WeaponAccuracyHandler>();
        effectsHandler  = GetComponent<WeaponEffectsHandler>();
        recoilHandler   = GetComponent<WeaponRecoilHandler>();
        reloadHandler   = GetComponent<WeaponReloadHandler>();
        inputHandler    = GetComponent<WeaponInputHandler>();
        utilityHandler  = GetComponent<WeaponUtility>();

        if (utilityHandler != null)
            utilityHandler.weapon = this; // ✅ KEY FIX!
    }
    
    public float currentAccuracy { get; set; }

    public bool CanShoot()
    {
        return currentAmmo > 0 && Time.time >= nextFireTime && !reloadHandler.IsReloading;
    }

    public void HandleShoot()
    {
        shootHandler.HandleShoot();
    }

    public void Reload()
    {
        reloadHandler.Reload();
    }

    public void StopReload()
    {
        reloadHandler.StopReload();
    }

    public void RecoverAccuracy()
    {
        accuracyHandler.RecoverAccuracy();
    }

    public void ApplyAccuracyPenalty()
    {
        accuracyHandler.ApplyAccuracyPenalty();
    }

    public Vector3 ApplyAccuracyToDirection(Vector3 direction)
    {
        return accuracyHandler.ApplyAccuracyToDirection(direction);
    }

    public void ShowMuzzleFlash()
    {
        effectsHandler.ShowMuzzleFlash();
    }

    public void CreateBulletHole(RaycastHit hit)
    {
        effectsHandler.CreateBulletHole(hit);
    }

    public IEnumerator ApplyRecoil()
    {
        return recoilHandler.ApplyRecoil();
    }

    public IEnumerator PlayShellDropDelayed(float delay)
    {
        return effectsHandler.PlayShellDropDelayed(delay);
    }

    public void MergeAmmo(int ammoAmount)
    {
        utilityHandler.MergeAmmo(ammoAmount);
    }

    public bool IsAmmoFull()
    {
        return utilityHandler.IsAmmoFull();
    }

    public void AddAmmo(int amount)
    {
        utilityHandler.AddAmmo(amount);
    }

    public void InitializeWeaponData(WeaponData data, int ammoAmount)
    {
        utilityHandler.InitializeWeaponData(data, ammoAmount);
    }

    public void ApplyWeaponSway()
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
}