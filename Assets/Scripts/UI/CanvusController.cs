using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvusController : MonoBehaviour
{
    public GameObject AchievementUI;
    public void ShowAchievemntUI()
    {
        if(AchievementUI.active == true)
        {
            AchievementUI.SetActive(false);
        }
        else
            AchievementUI.SetActive(true);
    }
}
