using UnityEngine;

public class WeaponPushback : MonoBehaviour
{
    [Header("Settings")]
    public Transform weaponHolder;           // Reference to your Weapon Holder
    public float pushbackDistance = 0.3f;    // How far back the weapon should move
    public float detectionRange = 0.5f;      // How close the weapon must be to a wall
    public float pushbackSpeed = 10f;        // Speed for smooth movement
    public LayerMask environmentLayer;       // Layers considered "wall"

    private Vector3 originalPosition;

    private void Start()
    {
        if (weaponHolder == null)
        {
            weaponHolder = transform; // Default to self if reference missing
        }

        originalPosition = weaponHolder.localPosition;
    }

    private void Update()
    {
        RaycastHit hit;

        // Check if there's an object (wall) directly in front of the player
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, detectionRange, environmentLayer))
        {
            Vector3 pushbackPosition = originalPosition - new Vector3(0, 0, pushbackDistance);

            // Smoothly move the weapon holder backward
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, pushbackPosition, Time.deltaTime * pushbackSpeed);
        }
        else
        {
            // Smoothly return to the original position
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, originalPosition, Time.deltaTime * pushbackSpeed);
        }
    }
}