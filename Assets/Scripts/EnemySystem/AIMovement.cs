using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIMovement : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Movement Settings")]
    public float stoppingDistance = 1f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
    }

    /// <summary>
    /// Tells the AI to move toward a world position.
    /// </summary>
    public void MoveTo(Vector3 destination)
    {
        if (agent.enabled)
        {
            agent.SetDestination(destination);
        }
    }

    /// <summary>
    /// Stops all movement immediately.
    /// </summary>
    public void Stop()
    {
        if (agent.enabled)
        {
            agent.ResetPath();
        }
    }

    /// <summary>
    /// Returns true if the AI is close enough to its destination.
    /// </summary>
    public bool IsAtDestination()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Faces the target direction smoothly (optional, useful for attacking).
    /// </summary>
    public void LookAt(Vector3 point, float turnSpeed = 5f)
    {
        Vector3 direction = (point - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        }
    }
}