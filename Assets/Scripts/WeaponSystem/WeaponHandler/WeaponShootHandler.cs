using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem; // Required for haptics

public class WeaponShootHandler : MonoBehaviour
{
    private Weapon weapon;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
        if (weapon == null)
        {
            Debug.LogError("❌ Weapon reference not found on WeaponShootHandler.");
        }
    }

    public void HandleShoot()
    {
        if (!weapon.CanShoot())
        {
            if (weapon.currentAmmo <= 0 && weapon.weaponData.emptyClickSound != null && weapon.emptyAudioSource != null)
            {
                weapon.emptyAudioSource.PlayOneShot(weapon.weaponData.emptyClickSound);
            }
            return;
        }

        weapon.currentAmmo--;
        weapon.nextFireTime = Time.time + weapon.weaponData.fireRate;
        weapon.ShowMuzzleFlash();

        if (weapon.weaponData.fireSound != null && weapon.fireAudioSource != null)
        {
            weapon.fireAudioSource.PlayOneShot(weapon.weaponData.fireSound);
        }

        if (weapon.weaponData.dropShellsSound != null && weapon.dropShellsAudioSource != null)
        {
            StartCoroutine(weapon.PlayShellDropDelayed(0.5f));
        }

        if (weapon.weaponData.shotgunSettings.isShotgun)
        {
            FireShotgunBlast();
        }
        else
        {
            FireSingleShot();
        }

        TriggerHaptics(); // 🟡 New: haptics

        weapon.ApplyAccuracyPenalty();
        StartCoroutine(weapon.ApplyRecoil());
    }

    private void FireSingleShot()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        RaycastHit hit;
        Vector3 shootDirection = weapon.ApplyAccuracyToDirection(mainCamera.transform.forward);

        if (Physics.Raycast(mainCamera.transform.position, shootDirection, out hit, weapon.weaponData.fireRange, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyHP enemyHP = hit.collider.GetComponent<EnemyHP>();
                if (enemyHP != null)
                {
                    enemyHP.TakeDamage((int)weapon.weaponData.damage);
                    Debug.Log($"Hit {hit.collider.gameObject.name} for {weapon.weaponData.damage} damage.");
                }
            }
            else
            {
                weapon.CreateBulletHole(hit);
            }
        }

        weapon.ShowMuzzleFlash();
    }

    private void FireShotgunBlast()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        for (int i = 0; i < weapon.weaponData.shotgunSettings.pelletsPerShot; i++)
        {
            Vector3 pelletDirection = weapon.ApplyAccuracyToDirection(mainCamera.transform.forward);

            if (Physics.Raycast(mainCamera.transform.position, pelletDirection, out RaycastHit hit, weapon.weaponData.fireRange, ~0, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    EnemyHP enemyHP = hit.collider.GetComponent<EnemyHP>();
                    if (enemyHP != null)
                    {
                        enemyHP.TakeDamage((int)(weapon.weaponData.damage / 2f));
                        Debug.Log($"Shotgun pellet hit {hit.collider.gameObject.name} for {weapon.weaponData.damage / 2f} damage.");
                    }
                }
                else
                {
                    weapon.CreateBulletHole(hit);
                }
            }
        }

        weapon.ShowMuzzleFlash();
    }

    private void TriggerHaptics()
    {
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            // Example rumble values: tweak for effect
            gamepad.SetMotorSpeeds(0.4f, 0.8f); // (lowFreq, highFreq)
            StartCoroutine(StopHapticsAfterDelay(0.2f));
        }
    }

    private IEnumerator StopHapticsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Gamepad.current?.SetMotorSpeeds(0f, 0f);
    }
}