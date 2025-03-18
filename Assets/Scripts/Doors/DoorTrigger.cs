using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private AutomaticDoor automaticDoor;

    [Header("Key Requirement Settings")]
    [SerializeField] private bool requiresKey = true;  // Toggle for requiring a key
    [SerializeField] private KeyPassType requiredKeyType;  // Key Type Enum
    [SerializeField] private GameObject lockedMessageUI; // Optional UI for feedback

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            // If 'requiresKey' is disabled, skip key check and open
            if (!requiresKey)
            {
                automaticDoor.OpenDoor();
                return;
            }

            // Requires a key
            if (inventory != null && inventory.HasKey(requiredKeyType))
            {
                automaticDoor.OpenDoor();
            }
            else
            {
                ShowLockedMessage();
            }
        }
    }

    private void ShowLockedMessage()
    {
        if (lockedMessageUI != null)
        {
            lockedMessageUI.SetActive(true);
            Invoke(nameof(HideLockedMessage), 2f); // Auto-hide message
        }

        Debug.Log("❌ Door Locked! Key Required.");
    }

    private void HideLockedMessage()
    {
        if (lockedMessageUI != null)
        {
            lockedMessageUI.SetActive(false);
        }
    }
}
