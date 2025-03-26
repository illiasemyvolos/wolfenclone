using UnityEngine;

[RequireComponent(typeof(AIController))]
public class AICombat : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    private float fireCooldown = 0f;
    private Transform player;
    private AIController ai;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        ai = GetComponent<AIController>();
    }

    private void Update()
    {
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;
    }

    public bool IsPlayerInRange()
    {
        if (player == null)
            return false;

        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= ai.behaviorData.attackRange;
    }

    public void Attack()
    {
        if (player == null || fireCooldown > 0f || projectilePrefab == null || firePoint == null)
            return;

        // If AI is not ranged, do not fire projectiles
        if (!ai.behaviorData.isRanged)
            return;

        fireCooldown = 1f / ai.behaviorData.fireRate;

        // Face the player
        ai.movement.LookAt(player.position);

        // Spawn projectile using firePoint's direction
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        EnemyProjectile proj = projectile.GetComponent<EnemyProjectile>();
        if (proj != null)
        {
            proj.damage = (int)ai.behaviorData.damage;
        }

        // Optional: add burst fire logic here in future
    }
}