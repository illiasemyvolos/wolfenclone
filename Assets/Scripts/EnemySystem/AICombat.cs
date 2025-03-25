using UnityEngine;

[RequireComponent(typeof(AIController))]
public class AICombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackRange = 10f;
    public float fireRate = 1f;
    public float damage = 10f;
    public LayerMask hitMask;

    private float fireCooldown = 0f;
    private Transform player;
    private AIController ai;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        ai = GetComponent<AIController>(); // ✅ Link to controller
    }

    private void Update()
    {
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;
    }

    /// <summary>
    /// Returns true if the player is within shooting range.
    /// </summary>
    public bool IsPlayerInRange()
    {
        if (player == null)
            return false;

        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= attackRange;
    }

    /// <summary>
    /// Fires at the player using raycast hitscan logic.
    /// </summary>
    public void Attack()
    {
        if (player == null || fireCooldown > 0f)
            return;

        fireCooldown = 1f / fireRate;

        // Optional: look at player for realism
        ai.movement.LookAt(player.position);

        Vector3 origin = transform.position + Vector3.up * 1.5f; // Eye level
        Vector3 direction = (player.position + Vector3.up * 1f - origin).normalized;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, attackRange, hitMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage((int)damage);
                    Debug.Log($"{gameObject.name} shot player for {damage} damage.");
                }
            }
        }

        // Optional: Add muzzle flash, sound, animation here
    }
}