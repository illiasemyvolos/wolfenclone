using UnityEngine;

public class AISenses : MonoBehaviour
{
    public LayerMask playerMask;
    public LayerMask obstructionMask;

    private Transform player;
    private AIController ai;

    private void Awake()
    {
        ai = GetComponent<AIController>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public bool CanSeePlayer()
    {
        if (player == null) return false;

        float viewRadius = ai.behaviorData.viewRadius;
        float viewAngle = ai.behaviorData.viewAngle;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > viewRadius) return false;

        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > viewAngle / 2f) return false;

        Vector3 eyePos = transform.position + Vector3.up * 1.5f;
        Vector3 target = player.position + Vector3.up * 1f;
        Vector3 dir = (target - eyePos).normalized;

        if (Physics.Raycast(eyePos, dir, out RaycastHit hit, viewRadius, playerMask | obstructionMask))
        {
            if (hit.collider.CompareTag("Player"))
                return true;
        }

        return false;
    }
}