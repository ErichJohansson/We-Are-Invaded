using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementController : MonoBehaviour
{
    GameController gc;

    public static AchievementController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        gc = GameController.Instance;
    }

    public void CheckEligibleAchievements()
    {
        Debug.Log("achievement check");
        if (gc.DistanceTraveled >= 100 && gc.DistanceTraveled < 1000)            
        {
            Debug.Log("first steps");
            UnlockAchievement(GPGSIds.achievement_first_steps);         
        }
        else if(gc.DistanceTraveled >= 1000 && gc.DistanceTraveled < 1337.42f)
        {
            UnlockAchievement(GPGSIds.achievement_sunday_driver);
        }
        else if (gc.DistanceTraveled >= 1337.42f && gc.DistanceTraveled < 5000)
        {
            UnlockAchievement(GPGSIds.achievement_1337_d21v32);
        }
        else if (gc.DistanceTraveled >= 5000)
        {
            UnlockAchievement(GPGSIds.achievement_roadmaster);
        }

        if (gc.MoneyEarned >= 100 && gc.MoneyEarned < 250)
        {
            UnlockAchievement(GPGSIds.achievement_monsters_arent_that_bad);
        }
        else if (gc.MoneyEarned >= 250 && gc.MoneyEarned < 500)
        {
            UnlockAchievement(GPGSIds.achievement_i_dont_like_monsters);
        }
        else if (gc.MoneyEarned >= 500 && gc.MoneyEarned < 1000)
        {
            UnlockAchievement(GPGSIds.achievement_i_hate_monsters);
        }
        else if (gc.MoneyEarned >= 1000)
        {
            UnlockAchievement(GPGSIds.achievement_monster_kill);
        }

        if (gc.ModifiersCollected >= 10 && gc.ModifiersCollected < 25)
        {
            UnlockAchievement(GPGSIds.achievement_i_like_powerups);
        }
        else if (gc.ModifiersCollected >= 25 && gc.ModifiersCollected < 50)
        {
            UnlockAchievement(GPGSIds.achievement_i_love_powerups);
        }
        else if (gc.ModifiersCollected >= 50)
        {
            UnlockAchievement(GPGSIds.achievement_powerup_pro);
        }
        Debug.Log("check done");
    }

    public void ReportToLeaderboard(long score)
    {
        Debug.Log("report score");
        Social.ReportScore(score, GPGSIds.leaderboard_longest_run, (bool success) => { });
    }

    public void UnlockAchievement(string tag)
    {
        Social.ReportProgress(tag, 100.0f, (bool success) => { });
    }
}
