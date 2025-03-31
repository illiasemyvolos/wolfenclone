using UnityEngine;
using System.Collections;

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

    [Header("Activation Settings")]
    public bool isActiveAtStart = false;

    [Header("Debug")]
    public string currentStateName;

    private float decisionTimer;
    private bool isRegistered = false; // ✅ Track if this enemy was already registered

    private void Awake()
    {
        senses = GetComponent<AISenses>();
        movement = GetComponent<AIMovement>();
        combat = GetComponent<AICombat>();
    }

    private void Start()
    {
        if (!isActiveAtStart)
        {
            DeactivateAI();
        }
        else
        {
            RegisterEnemyIfNeeded();
            StartCoroutine(DelayedStartState());
        }
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

    private IEnumerator DelayedStartState()
    {
        yield return null;

        if (movement != null && senses != null && combat != null)
        {
            ChangeState(new AIPatrolState(this));
        }

        decisionTimer = 0f;
    }

    public void ChangeState(AIState newState)
    {
        currentState?.Exit();
        currentState = newState;

        currentStateName = currentState?.GetType().Name ?? "None";
        Debug.Log($"🧠 {gameObject.name} switched to state: {currentStateName}");

        currentState?.Enter();
    }

    public AIState GetCurrentState() => currentState;

    public void ActivateAI()
    {
        RegisterEnemyIfNeeded(); // ✅ Register this enemy if not already done

        enabled = true;
        if (senses != null) senses.enabled = true;
        if (movement != null) movement.enabled = true;
        if (combat != null) combat.enabled = true;

        StartCoroutine(DelayedStartState());
    }

    public void DeactivateAI()
    {
        if (senses != null) senses.enabled = false;
        if (movement != null) movement.enabled = false;
        if (combat != null) combat.enabled = false;

        currentState = null; // Clear active AI logic
    }

    // ✅ Register enemy exactly once
    private void RegisterEnemyIfNeeded()
    {
        if (!isRegistered)
        {
            EnemyManager.Instance?.RegisterEnemy();
            isRegistered = true;
        }
    }
}