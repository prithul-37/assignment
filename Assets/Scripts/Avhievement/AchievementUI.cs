using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    public GameObject achievementPrefab;
    public Transform achievementList;

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        foreach (Transform child in achievementList)
        {
            Destroy(child.gameObject);
        }

        foreach (var achievement in AchievementManager.Instance.achievements)
        {
            GameObject obj = Instantiate(achievementPrefab, achievementList);
            obj.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = achievement.title;
            obj.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = achievement.description;
            obj.transform.Find("Icon").GetComponent<Image>().sprite = achievement.icon;
            obj.transform.Find("Unlocked").GetComponent<TextMeshProUGUI>().text = achievement.isUnlocked ? "" : "Locked";

            if (achievement.isUnlocked) obj.transform.Find("Locked").gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        UpdateUI();
    }
}
