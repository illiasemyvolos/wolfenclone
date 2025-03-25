using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ShootingEnemyController : MonoBehaviour
{
    [Header("AI Settings")]
    public float visionRange = 15f;
    public float attackRange = 10f;
    public float moveSpeed = 3.5f;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float shootingCooldown = 2f;

    [Header("Shooting Sector Settings")]
    public float shootingSectorAngle = 60f; // Shooting sector angle (degrees)

    [Header("Burst Fire Settings")]
    public bool useBurstFire = true;
    public int burstCount = 3;
    public float burstInterval = 0.2f;

    [Header("Aiming Settings")]
    public float rotationSpeed = 5f;

    [Header("Stagger Settings")]
    public float staggerDuration = 1f;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    private Transform player;
    private NavMeshAgent agent;
    private EnemyHealth enemyHealth;
    private float lastShotTime;

    private bool isStaggered = false;

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

        if (distanceToPlayer <= attackRange && IsPlayerInShootingSector())
        {
            FacePlayer();
            if (Time.time >= lastShotTime + shootingCooldown)
            {
                if (useBurstFire)
                {
                    StartCoroutine(BurstFire());
                }
                else
                {
                    ShootAtPlayer();
                }
            }
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

    IEnumerator BurstFire()
    {
        for (int i = 0; i < burstCount; i++)
        {
            if (IsPlayerInShootingSector())
            {
                ShootAtPlayer();
            }
            yield return new WaitForSeconds(burstInterval); 
        }

        lastShotTime = Time.time; 
    }

    void ShootAtPlayer()
    {
        if (firePoint != null && bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 shootDirection = (player.position - firePoint.position).normalized;
                rb.linearVelocity = shootDirection * bulletSpeed;
            }

            Destroy(bullet, 5f);  
        }

        lastShotTime = Time.time;
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    bool IsPlayerInShootingSector()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        return angleToPlayer <= shootingSectorAngle / 2;
    }

    public void Stagger()
    {
        if (!isStaggered)
        {
            isStaggered = true;
            agent.isStopped = true;

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
