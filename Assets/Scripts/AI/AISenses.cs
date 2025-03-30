using UnityEngine;

public class AISenses : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewRadius = 15f;
    public float viewAngle = 120f;
    public LayerMask playerMask;
    public LayerMask obstructionMask;

    private Transform player;
    private AIController ai;

    private void Awake()
    {
        ai = GetComponent<AIController>();
    }

    // Optional: still here if needed elsewhere
    private void Start()
    {
        EnsurePlayerReference();
    }

    private void EnsurePlayerReference()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    // Optional public method for external activation
    public void ForcePlayerRefresh()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public bool CanSeePlayer()
    {
        EnsurePlayerReference();
        if (player == null) return false;

        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 target = player.position + Vector3.up * 1.0f;

        Vector3 directionToPlayer = (target - origin).normalized;
        float distanceToPlayer = Vector3.Distance(origin, target);

        if (distanceToPlayer > viewRadius)
            return false;

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > viewAngle / 2f)
            return false;

        // Debug: draw line to player
        Debug.DrawRay(origin, directionToPlayer * viewRadius, Color.red, 0.2f);

        // Line of sight check
        if (Physics.Raycast(origin, directionToPlayer, out RaycastHit hit, viewRadius, playerMask | obstructionMask))
        {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }
}