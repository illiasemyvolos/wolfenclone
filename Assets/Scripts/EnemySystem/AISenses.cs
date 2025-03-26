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

    public bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Distance check
        if (distanceToPlayer > viewRadius)
            return false;

        // FOV check
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > viewAngle / 2f)
            return false;

        // Line of sight raycast
        Vector3 eyePosition = transform.position + Vector3.up * 1.5f; // AI eye level
        Vector3 playerTarget = player.position + Vector3.up * 1f;     // Player center/upper chest

        Vector3 rayDir = (playerTarget - eyePosition).normalized;

        if (Physics.Raycast(eyePosition, rayDir, out RaycastHit hit, viewRadius, playerMask | obstructionMask))
        {
            Debug.Log($"Ray hit: {hit.collider.name}");

            if (hit.collider.CompareTag("Player"))
                return true;
        }

        return false; // ❌ Blocked or missed
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 forward = transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2f, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2f, 0) * forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRadius);
    }
}