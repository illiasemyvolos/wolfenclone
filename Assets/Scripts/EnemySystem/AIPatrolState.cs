using UnityEngine;

public class AIPatrolState : AIState
{
    private int currentWaypointIndex = 0;
    private Transform[] waypoints;

    public AIPatrolState(AIController ai) : base(ai)
    {
        // Get waypoints from the enemy’s PatrolPath component
        PatrolPath path = ai.GetComponent<PatrolPath>();
        if (path != null)
        {
            waypoints = path.waypoints;
        }
    }

    public override void Enter()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        ai.movement.MoveTo(waypoints[currentWaypointIndex].position);
    }

    public override void Update()
    {
        // ✅ Player detection logic
        if (ai.senses.CanSeePlayer())
        {
            ai.ChangeState(new AIChaseState(ai));
            return;
        }

        // Patrol logic
        if (waypoints == null || waypoints.Length == 0)
            return;

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