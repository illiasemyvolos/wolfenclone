using UnityEngine;

public class AISenses : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewRadius = 15f;
    [Range(0, 360)] public float viewAngle = 120f;

    public LayerMask playerMask;
    public LayerMask obstructionMask;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check distance
        if (distanceToPlayer > viewRadius)
            return false;

        // Check FOV angle
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > viewAngle / 2f)
            return false;

        // ✅ Check for LOS (line of sight)
        Vector3 eyePos = transform.position + Vector3.up * 1.5f; // Eye height
        Vector3 playerPos = player.position + Vector3.up * 1f;

        if (Physics.Raycast(eyePos, (playerPos - eyePos).normalized, out RaycastHit hit, viewRadius, obstructionMask | playerMask))
        {
            if (hit.collider.CompareTag("Player"))
                return true; // Player is visible
        }

        return false; // Player is obstructed or missed
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 forward = transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRadius);
    }
}