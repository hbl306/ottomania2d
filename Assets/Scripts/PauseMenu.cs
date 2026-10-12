using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField]
    private GameFlowManager gameFlowManager;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Scene")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    private bool isPaused = false;

    private void Start()
    {
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // =========================
    // PAUSE
    // =========================

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;

        pausePanel.SetActive(true);

        Time.timeScale = 0f;
    }

    // =========================
    // RESUME
    // =========================

    public void ResumeGame()
    {
        isPaused = false;

        pausePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    // =========================
    // SAVE
    // =========================
public void SaveGame()
{
    if (player == null)
    {
        Debug.LogWarning(
            "PauseMenu chưa được gán Player!"
        );

        return;
    }

    if (gameFlowManager == null)
    {
        Debug.LogWarning(
            "PauseMenu chưa được gán GameFlowManager!"
        );

        return;
    }

    int mapIndex =
        gameFlowManager.GetCurrentMapIndex();

    SaveSystem.SaveGame(
        player.position,
        mapIndex
    );

    Debug.Log(
        "Saved tại Map " +
        (mapIndex + 1)
    );
}

    // =========================
    // SETTINGS
    // =========================

    public void OpenSettings()
    {
        if (settingsPanel == null)
            return;

        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel == null)
            return;

        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    // =========================
    // MAIN MENU
    // =========================

    public void GoToMainMenu()
{
    Time.timeScale = 1f;

    SceneManager.LoadScene("Menu");
}
}