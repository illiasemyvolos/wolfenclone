using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance { get; private set; }

    public float LevelTime { get; private set; } = 0f;
    public int EnemiesKilled { get; private set; } = 0;

    private bool levelEnded = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(InitializeLevelAfterOneFrame());
    }

    private IEnumerator InitializeLevelAfterOneFrame()
    {
        yield return new WaitUntil(() => EnemyManager.Instance.AliveEnemies > 0);

        EnemyManager.Instance.MarkAllEnemiesSpawned();
        Debug.Log("🧠 LevelController: All enemies marked as spawned.");
    }

    private void Update()
    {
        if (!levelEnded)
        {
            LevelTime += Time.deltaTime;
        }
    }

    public void AddKill()
    {
        EnemiesKilled++;
    }

    public void OnLevelFinished()
    {
        levelEnded = true;
        Debug.Log($"🏁 Level completed! Time: {LevelTime:F2}s | Kills: {EnemiesKilled}/{EnemyManager.Instance.TotalEnemies}");
    }

    // ✅ Easy-to-use stat access for UI

    public string GetFormattedTime()
    {
        return $"{LevelTime:F1}s";
    }

    public string GetFormattedKills()
    {
        int total = EnemyManager.Instance?.TotalEnemies ?? 0;
        return $"{EnemiesKilled} / {total}";
    }
}