using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Countdown Timer")]
    public float startTime = 30f;
    public TMP_Text timerText;

    [Header("Panels")]
    public GameObject pausePanel;     // contains Resume / Settings / Exit buttons
    public GameObject settingsPanel;  // contains Settings UI + Back button
    public GameObject gameOverPanel;  // shows when timer hits 0

    [Header("Exit")]
    public string exitSceneName = "MainMenu"; // scene to load when Exit is clicked

    float currentTime;
    bool isGameOver = false;

    void Start()
    {
        currentTime = startTime;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        Time.timeScale = 1f;
        UpdateTimerText();
    }

    void Update()
    {
        HandleEsc();

        if (isGameOver) return;

        // Countdown (will naturally pause when Time.timeScale = 0)
        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            TriggerGameOver();
        }

        UpdateTimerText();
    }

    // =========================
    // ESC KEY LOGIC
    // =========================
    void HandleEsc()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (isGameOver) return;

        // If settings is open, ESC goes back to pause menu
        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            CloseSettings();
            return;
        }

        // Otherwise toggle pause menu
        if (pausePanel != null && pausePanel.activeSelf)
            Resume();
        else
            OpenPause();
    }

    // =========================
    // TIMER UI
    // =========================
    void UpdateTimerText()
    {
        if (timerText == null) return;

        float t = Mathf.Max(0f, currentTime);
        int minutes = Mathf.FloorToInt(t / 60f);
        int seconds = Mathf.FloorToInt(t % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    // =========================
    // GAME OVER
    // =========================
    void TriggerGameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    // =========================
    // PAUSE MENU (buttons)
    // =========================
    public void OpenPause()
    {
        Time.timeScale = 0f;

        if (pausePanel != null) pausePanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void Resume()
    {
        Time.timeScale = 1f;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // =========================
    // SETTINGS (buttons)
    // =========================
    public void OpenSettings()
    {
        // Keep pausePanel ON if you want it behind settings,
        // or turn it OFF if you want settings alone. Here: settings alone.
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    // =========================
    // EXIT (button)
    // =========================
    public void ExitToScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(exitSceneName);
    }

    // Optional: restart current scene (button on GameOver panel)
    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
