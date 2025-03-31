using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public int AliveEnemies { get; private set; } = 0;
    public int TotalEnemies { get; private set; } = 0;
    public bool AllEnemiesSpawned { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterEnemy()
    {
        AliveEnemies++;
        TotalEnemies++;
        Debug.Log($"🧟 Enemy Registered — Alive: {AliveEnemies}, Total: {TotalEnemies}");
    }

    public void UnregisterEnemy()
    {
        AliveEnemies = Mathf.Max(0, AliveEnemies - 1);
        Debug.Log($"💀 Enemy Unregistered — Remaining: {AliveEnemies}");

        // ✅ Only check objectives after all enemies are known
        if (AllEnemiesSpawned)
        {
            ObjectivesManager.Instance?.CheckObjectives();
        }
    }

    public void MarkAllEnemiesSpawned()
    {
        AllEnemiesSpawned = true;
        Debug.Log("✅ All enemies are now spawned.");

        // Re-check now in case all enemies are already dead
        ObjectivesManager.Instance?.CheckObjectives();
    }
}