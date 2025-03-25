using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public float damage = 10f;
    public float lifetime = 5f;

    [Header("Impact Settings")]
    public GameObject impactEffectPrefab;  // Optional VFX for impact

    private void Start()
    {
        // Automatically destroy the projectile after its lifetime
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move the projectile forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.TakeDamage(damage);
                Debug.Log($"💥 Player hit for {damage} damage!");
            }

            CreateImpactEffect();
            Destroy(gameObject); // Destroy projectile on impact
        }
        else if (!other.CompareTag("Enemy") && !other.isTrigger)
        {
            // Collision with environment (walls, floors, etc.)
            Debug.Log("🧱 Projectile hit a wall or object!");
            CreateImpactEffect();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Additional check to destroy projectile when hitting non-trigger colliders
        if (!collision.collider.CompareTag("Player") && !collision.collider.CompareTag("Enemy"))
        {
            Debug.Log("💥 Projectile collided with an object and was destroyed.");
            CreateImpactEffect();
            Destroy(gameObject);
        }
    }

    private void CreateImpactEffect()
    {
        if (impactEffectPrefab != null)
        {
            Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}
