using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private AIController ai;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        ai = GetComponent<AIController>();
    }

    public void MoveTo(Vector3 destination)
    {
        agent.speed = ai.behaviorData.movementSpeed;
        agent.isStopped = false;
        agent.SetDestination(destination);
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }

    public bool IsAtDestination()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

    public void LookAt(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0f;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 8f);
        }
    }
}