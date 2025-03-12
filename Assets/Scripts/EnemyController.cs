using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("AI Settings")]
    public float visionRange = 15f;
    public float attackRange = 3f;
    public float moveSpeed = 3.5f;

    [Header("Attack Settings")]
    public float attackDamage = 10f;
    public float attackCooldown = 1f;

    [Header("Stagger Settings")]
    public float staggerDuration = 1f;  // Time enemy stays staggered

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    private Transform player;
    private NavMeshAgent agent;
    private EnemyHealth enemyHealth;
    private float lastAttackTime;

    private bool isStaggered = false; // Stagger state flag

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (agent != null)
        {
            agent.speed = moveSpeed;
        }
    }

    private void Update()
    {
        if (enemyHealth == null || player == null || isStaggered) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= visionRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void ChasePlayer()
    {
        if (player != null && agent.isActiveAndEnabled)
        {
            agent.isStopped = false; 
            agent.SetDestination(player.position);
        }
    }

    void AttackPlayer()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            agent.isStopped = true;

            if (player == null) return;

            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(attackDamage);
            }

            lastAttackTime = Time.time;

            // Resume movement after attack cooldown
            Invoke(nameof(ResumeMovement), attackCooldown);
        }
    }

    void ResumeMovement()
    {
        if (agent.isActiveAndEnabled)
        {
            agent.isStopped = false;
        }
    }

    // 🚨 New Stagger Logic
    public void Stagger()
    {
        if (!isStaggered) // Prevent stacking stagger effects
        {
            isStaggered = true;
            agent.isStopped = true;

            // Optional stagger animation or effect
            // Example: animator.SetTrigger("Stagger");

            Invoke(nameof(EndStagger), staggerDuration);
        }
    }

    void EndStagger()
    {
        isStaggered = false;
        if (agent.isActiveAndEnabled)
        {
            agent.isStopped = false;
        }
    }
}
