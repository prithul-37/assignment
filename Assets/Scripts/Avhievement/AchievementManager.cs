using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance; // Singleton

    public List<Achievement> achievements = new List<Achievement>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadAchievements();
    }

    public void UnlockAchievement(string title)
    {
        Achievement achievement = achievements.Find(a => a.title == title);

        if (achievement != null && !achievement.isUnlocked)
        {
            achievement.isUnlocked = true;
            Debug.Log($"Achievement Unlocked: {achievement.title}");
            // Trigger UI Notification
            ShowAchievementPopup(achievement);
        }
        SaveAchievements();
    }

    public GameObject achievementPopup;
    private void ShowAchievementPopup(Achievement achievement)
    {
        // Implement UI pop-up or feedback here

        achievementPopup.SetActive(true);
        achievementPopup.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = achievement.title;
        achievementPopup.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = achievement.description;
        achievementPopup.transform.Find("Icon").GetComponent<Image>().sprite = achievement.icon;

        StartCoroutine(HidePopupAfterDelay());


        Debug.Log($"Show UI: {achievement.title}");
    }

    private IEnumerator HidePopupAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        achievementPopup.SetActive(false);
    }

    public void SaveAchievements()
    {
        foreach (var achievement in achievements)
        {
            PlayerPrefs.SetInt(achievement.title, achievement.isUnlocked ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    public void LoadAchievements()
    {
        foreach (var achievement in achievements)
        {
            achievement.isUnlocked = PlayerPrefs.GetInt(achievement.title, 0) == 1;
        }
    }


}
