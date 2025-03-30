using UnityEngine;

public class EnemySpriteController : MonoBehaviour
{
    public Transform enemyRoot; // Reference to Enemy_X
    public Transform player;
    public SpriteRenderer spriteRenderer;

    public Sprite frontSprite;
    public Sprite frontLeftSprite;
    public Sprite leftSprite;
    public Sprite backLeftSprite;
    public Sprite backSprite;
    public Sprite backRightSprite;
    public Sprite rightSprite;
    public Sprite frontRightSprite;

    private string currentDirection;
    private string targetDirection;
    private float directionTimer = 0f;
    public float directionSmoothTime = 0.2f; // Time to hold new direction before switching

    void Start()
    {
        if (!player)
            player = GameObject.FindWithTag("Player").transform;

        currentDirection = "F";
        targetDirection = "F";
        SetSprite(currentDirection);
    }

    void LateUpdate()
    {
        // Billboard: face player on Y axis only
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0;

        if (toPlayer.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(toPlayer);
            transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
        }

        // Direction detection
        Vector3 dir = player.position - enemyRoot.position;
        dir.y = 0;
        float angle = Vector3.SignedAngle(enemyRoot.forward, dir, Vector3.up);

        targetDirection = GetDirectionLabel(angle);

        // Smooth transition
        if (targetDirection != currentDirection)
        {
            directionTimer += Time.deltaTime;
            if (directionTimer >= directionSmoothTime)
            {
                currentDirection = targetDirection;
                SetSprite(currentDirection);
                directionTimer = 0f;
            }
        }
        else
        {
            directionTimer = 0f; // Reset if direction matches
        }
    }

    string GetDirectionLabel(float angle)
    {
        if (angle > 157.5f || angle <= -157.5f) return "F";
        if (angle > 112.5f) return "FL";
        if (angle > 67.5f) return "L";
        if (angle > 22.5f) return "BL";
        if (angle > -22.5f) return "B";
        if (angle > -67.5f) return "BR";
        if (angle > -112.5f) return "R";
        if (angle > -157.5f) return "FR";
        return "F";
    }

    void SetSprite(string dir)
    {
        switch (dir)
        {
            case "F":  spriteRenderer.sprite = frontSprite; break;
            case "FL": spriteRenderer.sprite = frontLeftSprite; break;
            case "L":  spriteRenderer.sprite = leftSprite; break;
            case "BL": spriteRenderer.sprite = backLeftSprite; break;
            case "B":  spriteRenderer.sprite = backSprite; break;
            case "BR": spriteRenderer.sprite = backRightSprite; break;
            case "R":  spriteRenderer.sprite = rightSprite; break;
            case "FR": spriteRenderer.sprite = frontRightSprite; break;
        }
    }
}