using UnityEngine;

public class AISearchState : AIState
{
    private Vector3 searchPosition;
    private float searchTimer;
    private float searchDuration;
    private float rotationSpeed;

    public AISearchState(AIController ai, Vector3 lastKnownPosition) : base(ai)
    {
        searchPosition = lastKnownPosition;
        searchDuration = ai.behaviorData.searchDuration;
        rotationSpeed = ai.behaviorData.searchRotationSpeed;
    }

    public override void Enter()
    {
        searchTimer = searchDuration;
        ai.movement.MoveTo(searchPosition);
        
        Debug.Log($"🟡 {ai.gameObject.name} entering Search State");
    }

    public override void Update()
    {
        if (ai.senses.CanSeePlayer())
        {
            ai.ChangeState(new AIChaseState(ai));
            return;
        }

        if (!ai.movement.IsAtDestination())
            return;

        ai.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
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