using UnityEngine;

public class MedPack : MonoBehaviour
{
    [Header("MedPack Settings")]
    public float healthAmount = 25f;  // Amount of health restored

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController != null)
            {
                // Only allow pickup if player's health is NOT full
                if (playerController.GetCurrentHealth() < playerController.GetMaxHealth())
                {
                    playerController.RestoreHealth(healthAmount);
                    Debug.Log($"🟢 Player healed for {healthAmount} HP.");
                    Destroy(gameObject);  // Destroy MedPack after pickup
                }
                else
                {
                    Debug.Log("❗ Player's health is already full. MedPack NOT used.");
                }
            }
        }
    }
}
