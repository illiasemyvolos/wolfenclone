using UnityEngine;

public class AIChaseState : AIState
{
    private Transform player;

    public AIChaseState(AIController ai) : base(ai)
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public override void Enter()
    {
        // Optional: play chase animation, alert sound, etc.
    }

    public override void Update()
    {
        if (player == null)
            return;

        // Lost sight of the player → transition to search
        if (!ai.senses.CanSeePlayer())
        {
            Vector3 lastSeen = player.position;
            ai.ChangeState(new AISearchState(ai, lastSeen));
            return;
        }

        // Chase the player
        ai.movement.MoveTo(player.position);
        ai.movement.LookAt(player.position);

        // Attack if in range
        if (ai.combat.IsPlayerInRange())
        {
            ai.combat.Attack();
        }
    }

    public override void Exit()
    {
        ai.movement.Stop();
    }
}