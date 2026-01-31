using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Timer")]
    public float startTime = 30f;
    private float currentTime;
    private bool timerRunning = true;

    [Header("Timer UI")]
    public TMP_Text timerText;

    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject gameOverPanel;

    [Header("Exit Scene")]
    public string exitSceneName = "MainMenu"; // set in Inspector

    void Start()
    {
        currentTime = startTime;

        pausePanel?.SetActive(false);
        settingsPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);

        Time.timeScale = 1f;
        UpdateTimerText();
    }

    void Update()
    {
        HandleEscInput();

        if (!timerRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            GameOver();
        }

        UpdateTimerText();
    }

    // =========================
    // ESC KEY
    // =========================
    void HandleEscInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel != null && pausePanel.activeSelf)
                Resume();
            else
                OpenPause();
        }
    }

    // =========================
    // TIMER DISPLAY
    // =========================
    void UpdateTimerText()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    // =========================
    // GAME FLOW
    // =========================
    void GameOver()
    {
        timerRunning = false;
        Time.timeScale = 0f;
        gameOverPanel?.SetActive(true);
    }

    // =========================
    // PAUSE
    // =========================
    public void OpenPause()
    {
        Time.timeScale = 0f;
        timerRunning = false;
        pausePanel?.SetActive(true);
        settingsPanel?.SetActive(false);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        timerRunning = true;
        pausePanel?.SetActive(false);
        settingsPanel?.SetActive(false);
    }

    // =========================
    // SETTINGS
    // =========================
    public void OpenSettings()
    {
        settingsPanel?.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel?.SetActive(false);
    }

    // =========================
    // EXIT (LOAD SCENE)
    // =========================
    public void ExitToScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(exitSceneName);
    }

    // Optional: restart current level
    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
