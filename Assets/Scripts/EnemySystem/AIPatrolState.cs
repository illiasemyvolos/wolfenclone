using UnityEngine;
using UnityEngine.AI;

public class AIPatrolState : AIState
{
    private int currentWaypointIndex = 0;
    private Transform[] waypoints;
    private Transform selfTransform;

    public AIPatrolState(AIController ai) : base(ai)
    {
        // You could inject or fetch patrol points from a component on the enemy
        PatrolPath patrolPath = ai.GetComponent<PatrolPath>();
        waypoints = patrolPath != null ? patrolPath.waypoints : new Transform[0];
        selfTransform = ai.transform;
    }

    public override void Enter()
    {
        if (waypoints.Length == 0) return;

        ai.movement.MoveTo(waypoints[currentWaypointIndex].position);
    }

    public override void Update()
    {
        if (waypoints.Length == 0) return;

        // Check if player is visible
        if (ai.senses.CanSeePlayer())
        {
            ai.ChangeState(new AIChaseState(ai));
            return;
        }

        // Move to next waypoint when close enough
        if (ai.movement.IsAtDestination())
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            ai.movement.MoveTo(waypoints[currentWaypointIndex].position);
        }
    }

    public override void Exit()
    {
        ai.movement.Stop();
    }
}