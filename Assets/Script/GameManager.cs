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
    public GameObject losePanel; // ✅ NEW

    [Header("Exit")]
    public string exitSceneName = "MainMenu";

    [Header("Systems")]
    public BoxSpawner boxSpawner;
    public HoldToProgressTimer holdTimer;

    float currentTime;
    bool isGameOver = false;

    // ✅ NEW: gameplay systems start later (after Character B first route)
    bool systemsStarted = false;

    void Start()
    {
        currentTime = startTime;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        Time.timeScale = 1f;

        // start disabled until Character B tells us to start
        if (holdTimer != null) holdTimer.enabled = false;

        UpdateTimerText();
    }

    void Update()
    {
        HandleEsc();

        if (isGameOver) return;

        // ✅ Don’t countdown until systems have started
        if (!systemsStarted) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            TriggerGameOver();
        }

        UpdateTimerText();
    }

    // ✅ Called by CharacterMoveB after its FIRST route ends
    public void StartGameplaySystems()
    {
        if (systemsStarted) return;
        systemsStarted = true;

        if (boxSpawner != null) boxSpawner.EnableSpawning();
        if (holdTimer != null) holdTimer.enabled = true;

        Debug.Log("[GM] Systems started: spawner + hold timer + countdown.");
    }

    // ✅ Called by CharacterMoveB when spotted happens
    public void TriggerLose()
    {
        if (isGameOver) return;

        isGameOver = true;

        if (boxSpawner != null) boxSpawner.StopSpawning();
        if (holdTimer != null) holdTimer.enabled = false;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(true);

        Time.timeScale = 0f;

        Debug.Log("[GM] LOSE triggered. Game stopped.");
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
    // GAME OVER (timeout)
    // =========================
    void TriggerGameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
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
    // EXIT / RESTART
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
