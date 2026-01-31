using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Countdown Timer")]
    public float startTime = 30f;
    public TMP_Text timerText;

    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject gameOverPanel;

    [Header("Exit")]
    public string exitSceneName = "MainMenu";

    [Header("Start Conditions")]
    public CharacterMoveB boss;              // drag the boss object (CharacterMoveB)
    public BoxSpawner boxSpawner;            // drag your spawner
    public HoldToProgressTimer holdTimer;    // drag your hold-to-progress timer (if you have it)

    float currentTime;
    bool isGameOver = false;

    // NEW: gate
    bool gameStarted = false;

    void Start()
    {
        currentTime = startTime;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        Time.timeScale = 1f;

        // optional: disable hold timer until boss finishes
        if (holdTimer != null) holdTimer.enabled = false;

        UpdateTimerText();
    }

    void Update()
    {
        HandleEsc();

        if (isGameOver) return;

        // ✅ Wait for boss before starting systems + countdown
        if (!gameStarted)
        {
            if (boss != null && boss.finished)
            {
                gameStarted = true;

                if (boxSpawner != null)
                    boxSpawner.EnableSpawning();

                if (holdTimer != null)
                    holdTimer.enabled = true;

                Debug.Log("[GM] Boss finished. Spawner + hold timer + countdown started.");
            }

            return; // do NOT tick countdown yet
        }

        // Countdown (pauses naturally when Time.timeScale = 0)
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

        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            CloseSettings();
            return;
        }

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

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}