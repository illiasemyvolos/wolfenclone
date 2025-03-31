using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHP : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Death Settings")]
    public GameObject deathEffect;
    public AudioClip deathSound;
    public bool destroyOnDeath = true;
    public float destroyDelay = 2f;

    private void Start()
    {
        currentHealth = maxHealth;
    }
    
    private void OnEnable()
    {
        EnemyManager.Instance?.RegisterEnemy();
    }

    private void OnDisable()
    {
        Debug.Log($"{gameObject.name} unregistered from EnemyManager.");
        EnemyManager.Instance?.UnregisterEnemy();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        AIController ai = GetComponent<AIController>();
        if (ai != null) ai.enabled = false;

        NavMeshAgent nav = GetComponent<NavMeshAgent>();
        if (nav != null) nav.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
        // ✅ Add this
        LevelController.Instance?.AddKill();
    }
}