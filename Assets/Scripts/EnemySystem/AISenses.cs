using UnityEngine;

public class AISenses : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewRadius = 15f;
    [Range(0, 360)]
    public float viewAngle = 120f;

    public LayerMask playerMask;
    public LayerMask obstructionMask;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    /// <summary>
    /// Checks if the player is within vision range and in line of sight.
    /// </summary>
    public bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check distance
        if (distanceToPlayer > viewRadius)
            return false;

        // Check angle
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > viewAngle / 2f)
            return false;

        // Check for obstructions
        if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer, distanceToPlayer, obstructionMask))
            return false;

        return true;
    }

    /// <summary>
    /// Debug visualization in editor.
    /// </summary>
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