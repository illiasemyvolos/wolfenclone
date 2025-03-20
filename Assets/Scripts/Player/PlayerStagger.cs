using UnityEngine;

public class PlayerStagger : MonoBehaviour
{
    [Header("Stagger Settings")]
    public float staggerDuration = 1.5f;
    public float staggerSpeedReduction = 0.7f; // Adjusted for better balance

    private PlayerController playerController;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void Stagger()
    {
        if (playerController == null) return;
        playerController.ModifyMovementSpeed(staggerSpeedReduction, staggerDuration);
    }
}