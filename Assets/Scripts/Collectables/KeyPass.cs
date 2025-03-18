using UnityEngine;

public class KeyPass : MonoBehaviour
{
    [SerializeField] private KeyPassType keyType; // Select key type in Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.AddKey(keyType);
                Destroy(gameObject); // Remove the key from the scene
            }
        }
    }
}
