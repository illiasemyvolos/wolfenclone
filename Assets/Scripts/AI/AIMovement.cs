using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private AIController ai;

    private float updateRate = 1f / 30f; // 30 FPS
    private float updateTimer = 0f;

    private Vector3 currentDestination;
    private bool hasDestination = false;

    private Vector3? lookTarget = null;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        ai = GetComponent<AIController>();

        // Turn off automatic movement — we'll do it manually
        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    private void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer >= updateRate)
        {
            updateTimer = 0f;

            if (hasDestination)
            {
                Vector3 nextDirection = agent.desiredVelocity.normalized;
                float moveStep = ai.behaviorData.movementSpeed * updateRate;

                Vector3 newPosition = transform.position + nextDirection * moveStep;
                agent.nextPosition = newPosition; // keeps NavMeshAgent in sync
                transform.position = newPosition;

                // Optional: Face direction of movement
                if (nextDirection != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(nextDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, updateRate * 8f);
                }
            }

            if (lookTarget.HasValue)
            {
                Vector3 dir = (lookTarget.Value - transform.position).normalized;
                dir.y = 0f;
                if (dir != Vector3.zero)
                {
                    Quaternion lookRot = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, updateRate * 8f);
                }
            }
        }
    }

    public void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
        currentDestination = destination;
        hasDestination = true;
    }

    public void Stop()
    {
        hasDestination = false;
        agent.ResetPath();
    }

    public bool IsAtDestination()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

    public void LookAt(Vector3 target)
    {
        lookTarget = target;
    }
}