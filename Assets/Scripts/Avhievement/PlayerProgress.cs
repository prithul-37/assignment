using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    private int enemiesKilled = 0;

    public void EnemyKilled()
    {
        enemiesKilled++;
        if (enemiesKilled == 10)
        {
            AchievementManager.Instance.UnlockAchievement("First 10 Kills");
        }
    }
}
