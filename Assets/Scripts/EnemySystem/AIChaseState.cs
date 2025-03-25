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
        // No-op for now
    }

    public override void Update()
    {
        if (player == null)
            return;

        // Stop chasing if player is lost
        if (!ai.senses.CanSeePlayer())
        {
            ai.ChangeState(new AIPatrolState(ai));
            return;
        }

        // Chase player
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