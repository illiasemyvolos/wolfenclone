using UnityEngine;

public class AISearchState : AIState
{
    private Vector3 searchPosition;
    private float searchDuration = 3f;
    private float searchTimer;

    public AISearchState(AIController ai, Vector3 lastKnownPosition) : base(ai)
    {
        searchPosition = lastKnownPosition;
    }

    public override void Enter()
    {
        searchTimer = searchDuration;
        ai.movement.MoveTo(searchPosition);
    }

    public override void Update()
    {
        // If we see the player again, chase!
        if (ai.senses.CanSeePlayer())
        {
            ai.ChangeState(new AIChaseState(ai));
            return;
        }

        // Wait until reaching the spot
        if (!ai.movement.IsAtDestination())
            return;

        // Rotate to simulate searching
        ai.transform.Rotate(0f, 60f * Time.deltaTime, 0f);

        // Countdown
        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0f)
        {
            ai.ChangeState(new AIPatrolState(ai));
        }
    }

    public override void Exit()
    {
        ai.movement.Stop();
    }
}