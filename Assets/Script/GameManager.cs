using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Countdown Timer")]
    public float startTime = 30f;
    public TMP_Text timerText;

    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject gameOverPanel;

    [Header("Exit")]
    public string exitSceneName = "MainMenu";

    // =========================
    // NEW: HOLD UI
    // =========================
    [Header("Hold UI")]
    public GameObject holdPanel;                // the mini panel that opens
    public HoldToProgressTimer holdTimer;       // your hold timer script

    private BoxMover activeBox;                 // the box currently waiting

    float currentTime;
    bool isGameOver = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentTime = startTime;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // NEW
        if (holdPanel != null) holdPanel.SetActive(false);

        Time.timeScale = 1f;
        UpdateTimerText();
    }

    void Update()
    {
        HandleEsc();

        if (isGameOver) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            TriggerGameOver();
        }

        UpdateTimerText();
    }

    // =========================
    // NEW: OPEN HOLD FOR BOX
    // =========================
    public void OpenHoldForBox(BoxMover box)
    {
        activeBox = box;

        if (holdTimer != null)
            holdTimer.ResetHold();

        if (holdPanel != null)
            holdPanel.SetActive(true);
    }

    // Called by HoldToProgressTimer when completed
    public void OnHoldComplete()
    {
        if (holdPanel != null)
            holdPanel.SetActive(false);

        if (activeBox != null)
        {
            activeBox.ContinueToLast();
            activeBox = null;
        }
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

        // NEW: hide hold UI if game over
        if (holdPanel != null) holdPanel.SetActive(false);
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
