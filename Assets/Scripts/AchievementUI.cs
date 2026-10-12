using UnityEngine;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject achievementPanel;

    private void Start()
    {
        achievementPanel.SetActive(false);
    }

    public void OpenAchievements()
    {
        gameOverPanel.SetActive(false);
        achievementPanel.SetActive(true);
    }

    public void CloseAchievements()
    {
        achievementPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }
}