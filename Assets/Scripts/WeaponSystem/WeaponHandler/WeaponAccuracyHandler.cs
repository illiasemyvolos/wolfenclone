using UnityEngine;

public class WeaponAccuracyHandler : MonoBehaviour
{
    private Weapon weapon;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
        if (weapon == null)
        {
            Debug.LogError("❌ Weapon reference not found on WeaponAccuracyHandler.");
        }
    }

    public void ApplyAccuracyPenalty()
    {
        weapon.currentAccuracy = Mathf.Clamp(
            weapon.currentAccuracy - weapon.weaponData.accuracyAndRecoil.movementAccuracyPenalty,
            0.2f,
            weapon.weaponData.accuracyAndRecoil.baseAccuracy
        );
    }

    public void RecoverAccuracy()
    {
        if (weapon.currentAccuracy < weapon.weaponData.accuracyAndRecoil.baseAccuracy)
        {
            weapon.currentAccuracy = Mathf.Min(
                weapon.currentAccuracy + weapon.weaponData.accuracyAndRecoil.accuracyRecoveryRate * Time.deltaTime,
                weapon.weaponData.accuracyAndRecoil.baseAccuracy
            );
        }
    }

    public Vector3 ApplyAccuracyToDirection(Vector3 direction)
    {
        float spread = (1f - weapon.currentAccuracy) * weapon.weaponData.spreadMultiplier * 10f;

        Quaternion spreadRotation = Quaternion.Euler(
            Random.Range(-spread, spread),
            Random.Range(-spread, spread),
            0f
        );

        return spreadRotation * direction;
    }
}