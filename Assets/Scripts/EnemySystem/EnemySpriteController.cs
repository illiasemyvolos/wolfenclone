using UnityEngine;

public class EnemySpriteController : MonoBehaviour
{
    public Transform enemyRoot; // Reference to Enemy_X
    public Transform player;
    public SpriteRenderer spriteRenderer;

    public Sprite frontSprite; // You’ll later assign 8 directional sprites

    void Start()
    {
        if (!player)
            player = GameObject.FindWithTag("Player").transform;
    }

    void LateUpdate()
    {
        // Face the player on Y-axis only (like billboard)
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0; // Ignore vertical

        if (toPlayer.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(toPlayer);
            transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
        }

        // Use front sprite for now (you’ll later add directional logic)
        spriteRenderer.sprite = frontSprite;
    }
}