using UnityEngine;
using System.Collections.Generic;

public class EnemyRoomTrigger : MonoBehaviour
{
    [Header("Enemies in This Room")]
    public List<GameObject> enemies;

    [Header("Trigger Settings")]
    public bool triggerOnce = true;
    public float activationDelay = 1f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && triggerOnce)
            return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(ActivateEnemiesDelayed());
        }
    }

    private System.Collections.IEnumerator ActivateEnemiesDelayed()
    {
        Debug.Log($"⏳ Enemy activation will start in {activationDelay} seconds...");
        yield return new WaitForSeconds(activationDelay);

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            AIController ai = enemy.GetComponent<AIController>();
            if (ai != null)
            {
                ai.ActivateAI();
                Debug.Log($"⚡ Activated enemy: {enemy.name}");
            }
            else
            {
                Debug.LogWarning($"❌ Missing AIController on: {enemy.name}");
            }
        }

        // ✅ Wait until all enemies are active before marking all as spawned
        yield return new WaitUntil(() =>
        {
            int activeEnemies = 0;
            foreach (GameObject enemy in enemies)
            {
                if (enemy != null && enemy.activeInHierarchy)
                    activeEnemies++;
            }
            return activeEnemies == enemies.Count;
        });

        EnemyManager.Instance.MarkAllEnemiesSpawned();
        Debug.Log("✅ EnemyRoomTrigger activation complete.");
    }
}