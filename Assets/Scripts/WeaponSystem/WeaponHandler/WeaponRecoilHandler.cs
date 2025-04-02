using UnityEngine;
using System.Collections;

public class WeaponRecoilHandler : MonoBehaviour
{
    private Weapon weapon;
    private Vector3 originalPosition;
    private CrosshairScaler crosshairScaler;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
        if (weapon == null)
        {
            Debug.LogError("❌ Weapon reference not found on WeaponRecoilHandler.");
            return;
        }

        originalPosition = transform.localPosition;

        // Try to find the crosshair scaler in the scene
        crosshairScaler = FindObjectOfType<CrosshairScaler>();
        if (crosshairScaler == null)
        {
            Debug.LogWarning("⚠️ CrosshairScaler not found in the scene. Crosshair will not scale with recoil.");
        }
    }

    public IEnumerator ApplyRecoil()
    {
        // Recoil visual movement
        float recoilStrength = weapon.weaponData.accuracyAndRecoil.recoilAmount * 0.2f;
        Vector3 recoilPosition = originalPosition - new Vector3(0, 0, recoilStrength);

        float recoilDuration = 0.1f;
        float recoveryDuration = 0.15f;
        float frameDelay = 1f / 30f;

        float elapsedTime = 0f;

        // 🔥 Apply recoil crosshair scaling
        // 🔥 Bump the crosshair based on recoil amount
        if (crosshairScaler != null)
        {
            float intensity = Mathf.Clamp01(weapon.weaponData.accuracyAndRecoil.recoilAmount / 3f);
            crosshairScaler.Bump(intensity);
        }

        // Recoil Phase
        while (elapsedTime < recoilDuration)
        {
            float t = elapsedTime / recoilDuration;
            transform.localPosition = Vector3.Lerp(transform.localPosition, recoilPosition, t);
            elapsedTime += frameDelay;
            yield return new WaitForSeconds(frameDelay);
        }

        elapsedTime = 0f;

        // Recovery Phase
        while (elapsedTime < recoveryDuration)
        {
            float t = elapsedTime / recoveryDuration;
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, t);
            elapsedTime += frameDelay;
            yield return new WaitForSeconds(frameDelay);
        }

        transform.localPosition = originalPosition;
    }
}