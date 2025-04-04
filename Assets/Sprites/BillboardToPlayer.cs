using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BillboardToPlayer : MonoBehaviour
{
    private Transform cameraTransform;

    void Start()
    {
        // Cache the camera transform for performance
        if (Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Make the sprite face the camera
        Vector3 direction = cameraTransform.position - transform.position;
        direction.y = 0f; // Optional: lock Y-axis for side-facing only

        transform.forward = -direction.normalized; // Flip to face camera
    }
}