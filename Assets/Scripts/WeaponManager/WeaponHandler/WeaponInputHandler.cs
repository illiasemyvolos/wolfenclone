using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponInputHandler : MonoBehaviour
{
    private Weapon weapon;
    private InputAction fireAction;
    private PlayerInput playerInput;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
        if (weapon == null)
        {
            Debug.LogError("❌ Weapon reference not found on WeaponInputHandler.");
            return;
        }

        playerInput = GetComponentInParent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("❌ PlayerInput not found in parent object.");
            return;
        }

        fireAction = playerInput.actions["Player/Fire"];
    }

    private void Update()
    {
        if (weapon.weaponData == null) return;

        // 🔥 Auto fire mode
        if (weapon.weaponData.isAutoFire && fireAction.IsPressed())
        {
            if (weapon.CanShoot())
            {
                weapon.HandleShoot();
            }
        }
        // 🔫 Semi-auto fire mode
        else if (!weapon.weaponData.isAutoFire && fireAction.triggered)
        {
            if (weapon.CanShoot())
            {
                weapon.HandleShoot();
            }
        }

        weapon.RecoverAccuracy(); // Can delegate to WeaponAccuracyHandler if exposed
        weapon.ApplyWeaponSway(); // Optional if sway isn't modularized yet
    }
}