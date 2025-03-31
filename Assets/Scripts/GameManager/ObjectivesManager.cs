using UnityEngine;

public class ObjectivesManager : MonoBehaviour
{
    public static ObjectivesManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void CheckObjectives()
    {
        if (EnemyManager.Instance != null && EnemyManager.Instance.AliveEnemies <= 0)
        {
            if (LevelController.Instance != null)
            {
                LevelController.Instance.OnLevelFinished();

                // Save stats globally before scene transition
                GameStats.EnemiesKilled = LevelController.Instance.EnemiesKilled;
                GameStats.LevelTime = LevelController.Instance.LevelTime;
            }

            Debug.Log("Objectives complete — level finished!");
            GameStateManager.Instance.SetState(GameState.Victory);
        }
    }
}