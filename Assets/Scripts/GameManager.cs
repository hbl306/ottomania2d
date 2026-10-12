using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int score = 0;
    private int item = 0;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI itemText;

    [SerializeField] private GameObject gameOverUi;
    [SerializeField] private TextMeshProUGUI scoreEnd;
    [SerializeField] private TextMeshProUGUI itemEnd;
    [SerializeField] private TextMeshProUGUI gameOverTimeText;

    [SerializeField] private GameObject victoryUi;
    [SerializeField] private TextMeshProUGUI VscoreEnd;
    [SerializeField] private TextMeshProUGUI VitemEnd;
    [SerializeField] private TextMeshProUGUI victoryTimeText;

    private bool isGameOver = false;
    private bool isVictory = false;

    private float playTime = 0f;

    private void Start()
    {
        Time.timeScale = 1f;

        score = 0;
        item = 0;

        UpdateScoreText();
        UpdateScoreTextItem();

        isGameOver = false;
        isVictory = false;

        if (gameOverUi != null)
            gameOverUi.SetActive(false);

        if (victoryUi != null)
            victoryUi.SetActive(false);

        if (SaveSystem.HasSave())
        {
            playTime = SaveSystem.GetSavedPlayTime();
        }
        else
        {
            playTime = 0f;
        }
    }

    private void Update()
    {
        UpdatePlayTime();
    }

    private void UpdatePlayTime()
    {
        if (isGameOver || isVictory)
            return;

        playTime += Time.deltaTime;
    }

    private string GetFormattedTime()
    {
        int totalSeconds = Mathf.FloorToInt(playTime);

        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        if (hours > 0)
        {
            return string.Format(
                "{0:00}:{1:00}:{2:00}",
                hours,
                minutes,
                seconds
            );
        }

        return string.Format(
            "{0:00}:{1:00}",
            minutes,
            seconds
        );
    }

    public float GetPlayTime()
    {
        return playTime;
    }

    public void SetPlayTime(float time)
    {
        playTime = Mathf.Max(0f, time);
    }

    public void AddScore(int points)
    {
        if (isGameOver || isVictory)
            return;

        score += points;

        UpdateScoreText();
    }

    public void AddScoreItem(int points)
    {
        if (isGameOver || isVictory)
            return;

        item += points;

        UpdateScoreTextItem();
    }

    public void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void UpdateScoreTextItem()
    {
        if (itemText != null)
        {
            itemText.text = item.ToString();
        }
    }

    public void GameOver()
    {
        if (isGameOver || isVictory)
            return;

        isGameOver = true;

        if (scoreEnd != null)
            scoreEnd.text = score.ToString();

        if (itemEnd != null)
            itemEnd.text = item.ToString();

        if (gameOverTimeText != null)
            gameOverTimeText.text = GetFormattedTime();

        if (gameOverUi != null)
            gameOverUi.SetActive(true);

        AudioManager.Instance?
            .PlayGameOverMusic();

        Time.timeScale = 0f;
    }

    public void Victory()
    {
        if (isGameOver || isVictory)
            return;

        isVictory = true;

        if (VscoreEnd != null)
            VscoreEnd.text = score.ToString();

        if (VitemEnd != null)
            VitemEnd.text = item.ToString();

        if (victoryTimeText != null)
            victoryTimeText.text = GetFormattedTime();

        if (victoryUi != null)
            victoryUi.SetActive(true);

        AudioManager.Instance?
            .PlayVictoryMusic();

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        SaveSystem.DeleteSave();

        isGameOver = false;
        isVictory = false;

        score = 0;
        item = 0;
        playTime = 0f;

        Time.timeScale = 1f;

        SceneManager.LoadScene("Game");
    }

    public void ReturnMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Menu");
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public bool IsVictory()
    {
        return isVictory;
    }
}