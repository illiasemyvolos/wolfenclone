using UnityEngine;

[RequireComponent(typeof(AIController))]
public class AICombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackRange = 10f;
    public float fireRate = 1f;
    public float damage = 10f;
    public LayerMask hitMask;

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
        return distance <= attackRange;
    }

    public void Attack()
    {
        if (player == null || fireCooldown > 0f || projectilePrefab == null || firePoint == null)
            return;

        fireCooldown = 1f / fireRate;

        // Ensure the firePoint is facing the player
        ai.movement.LookAt(player.position);

        // Instantiate projectile using firePoint's rotation (accurate!)
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        EnemyProjectile proj = projectile.GetComponent<EnemyProjectile>();
        if (proj != null)
        {
            proj.damage = (int)damage;
        }

        // Optional: play VFX or SFX here
    }
}