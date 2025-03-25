using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Death Settings")]
    public GameObject deathEffectPrefab;

    private EnemyController enemyController;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        enemyController = GetComponent<EnemyController>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (enemyController != null)
        {
            enemyController.Stagger(); // Trigger stagger when hit
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);  
    }
}
