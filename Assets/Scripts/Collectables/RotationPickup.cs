using UnityEngine;

public class RotationPickup : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationAxis = new Vector3(0f, 1f, 0f);
    [SerializeField] private float rotationSpeed = 50f;

    void Update()
    {
        RotatePickup();
    }

    private void RotatePickup()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}