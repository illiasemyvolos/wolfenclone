using UnityEngine;

[RequireComponent(typeof(AISenses))]
[RequireComponent(typeof(AIMovement))]
[RequireComponent(typeof(AICombat))]
public class AIController : MonoBehaviour
{
    private AIState currentState;

    [HideInInspector] public AISenses senses;
    [HideInInspector] public AIMovement movement;
    [HideInInspector] public AICombat combat;

    [Header("AI Settings")]
    public float decisionRate = 0.2f;

    [Header("AI Behavior Data")]
    public AIBehaviorData behaviorData;

    private float decisionTimer;

    private void Awake()
    {
        senses = GetComponent<AISenses>();
        movement = GetComponent<AIMovement>();
        combat = GetComponent<AICombat>();
    }

    private void Start()
    {
        ChangeState(new AIPatrolState(this));
    }

    private void Update()
    {
        decisionTimer -= Time.deltaTime;
        if (decisionTimer <= 0f)
        {
            decisionTimer = decisionRate;
            currentState?.Update();
        }
    }

    public void ChangeState(AIState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public AIState GetCurrentState() => currentState;
}