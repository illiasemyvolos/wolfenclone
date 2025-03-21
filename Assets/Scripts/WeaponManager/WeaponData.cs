using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/New Weapon")]
public class WeaponData : ScriptableObject
{
    [System.Serializable]
    public struct ShotgunSettings
    {
        public bool isShotgun;
        [Range(1, 20)] public int pelletsPerShot;
        [Range(0f, 30f)] public float spreadAngle;
    }

    [System.Serializable]
    public struct AccuracyAndRecoil
    {
        [Range(0f, 1f)] public float baseAccuracy;
        [Range(0f, 1f)] public float movementAccuracyPenalty;
        [Range(0f, 2f)] public float accuracyRecoveryRate;
        [Range(0f, 2f)] public float recoilAmount;
        [Range(1f, 20f)] public float recoilSpeed;
    }

    [Header("Basic Info")]
    public string weaponName;
    public AmmoType ammoType;

    [Header("Weapon Stats")]
    [Range(1, 200)] public int damage = 10;
    [Range(0.01f, 5f)] public float fireRate = 0.1f;
    [Range(1, 100)] public int clipSize = 30;
    [Range(1, 500)] public int maxAmmo = 120;
    [Range(0.5f, 5f)] public float reloadTime = 2f;
    [Range(1f, 100f)] public float fireRange = 50f;

    [Header("Shotgun Settings")]
    public ShotgunSettings shotgunSettings;

    [Header("Fire Mode Settings")]
    public bool isAutoFire = false; // 🔥 Auto-fire toggle (true = hold to shoot)
    
    [Header("Accuracy & Recoil")]
    public AccuracyAndRecoil accuracyAndRecoil;
    
    [Header("Spread Settings")]
    [Range(0f, 5f)] public float spreadMultiplier = 1f;  // New spread control
    
    [Header("Weapon Sway Settings")]
    [Range(0f, 0.1f)] public float swayAmount = 0.02f;        // 🔄 Amount of horizontal sway
    [Range(1f, 10f)] public float swaySpeed = 4f;             // ⏳ Speed of sway
    [Range(5f, 20f)] public float swaySmoothness = 10f;       // ⚙️ Smooths sway movement

    [Header("Muzzle Flash Settings")]
    public Vector3 muzzleFlashOffset = Vector3.zero; // 🔥 Adjustable offset for muzzle position

    [Header("Effects")]
    public GameObject muzzleFlashPrefab;
    public GameObject bulletHolePrefab;
    [Range(1f, 30f)] public float bulletHoleLifetime = 10f;
}