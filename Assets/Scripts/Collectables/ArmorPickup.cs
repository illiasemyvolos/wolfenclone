using UnityEngine;

public class ArmorPickup : MonoBehaviour
{
    [SerializeField] private float armorAmount = 25f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Pickup only if current armor is less than max armor
                if (playerController.GetCurrentArmor() < playerController.GetMaxArmor())
                {
                    playerController.AddArmor(armorAmount);
                    Destroy(gameObject); // Remove pickup after collection
                }
                else
                {
                    Debug.Log("🛡️ Armor is already full. Pickup ignored.");
                }
            }
        }
    }
}
