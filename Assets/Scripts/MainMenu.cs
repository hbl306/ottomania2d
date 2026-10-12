
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button continueButton;

    [Header("Story Introduction")]
    [SerializeField] private GameObject storyPanel;
    [SerializeField] private Image storyImage;
    [SerializeField] private Sprite[] storySlides = new Sprite[3];
    [SerializeField] private TMP_Text nextButtonText;

    private int currentStoryIndex = 0;

    private void Start()
    {
        Time.timeScale = 1f;

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (storyPanel != null)
            storyPanel.SetActive(false);

        UpdateContinueButton();
    }

    private void UpdateContinueButton()
    {
        if (continueButton != null)
        {
            continueButton.interactable = SaveSystem.HasSave();
        }
    }

    // NEW GAME: mo cot truyen
    public void PlayGame()
    {
        if (storyPanel == null || storyImage == null ||
            storySlides == null || storySlides.Length != 3)
        {
            Debug.LogError("Story UI is not configured!");
            return;
        }

        foreach (Sprite slide in storySlides)
        {
            if (slide == null)
            {
                Debug.LogError("Missing story image!");
                return;
            }
        }

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        currentStoryIndex = 0;

        storyPanel.SetActive(true);
        ShowStorySlide();
    }

    // Hien thi anh cot truyen
    private void ShowStorySlide()
    {
        storyImage.sprite = storySlides[currentStoryIndex];

        if (nextButtonText != null)
        {
            if (currentStoryIndex == storySlides.Length - 1)
                nextButtonText.text = "START GAME";
            else
                nextButtonText.text = "NEXT";
        }
    }

    // NEXT: chuyen anh hoac vao game
    public void NextStory()
    {
        if (storyPanel == null || !storyPanel.activeSelf)
            return;

        if (currentStoryIndex < storySlides.Length - 1)
        {
            currentStoryIndex++;
            ShowStorySlide();
        }
        else
        {
            BeginNewGame();
        }
    }

    // Ket thuc cot truyen, bat dau game moi
    private void BeginNewGame()
    {
        SaveSystem.DeleteSave();
        SaveSystem.LoadFromContinue = false;

        SceneManager.LoadScene("Game");
    }

    // BACK: quay ve menu
    public void CloseStory()
    {
        if (storyPanel != null)
            storyPanel.SetActive(false);

        currentStoryIndex = 0;
    }

    // CONTINUE: choi tiep save cu
    public void ContinueGame()
    {
        if (!SaveSystem.HasSave())
            return;

        SaveSystem.LoadFromContinue = true;

        SceneManager.LoadScene("Game");
    }

    // SETTINGS
    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    // QUIT
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
