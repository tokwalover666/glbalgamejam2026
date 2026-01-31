using UnityEngine;
using TMPro;

public class BoxChecker : MonoBehaviour
{
    [Header("Win Condition")]
    public int boxesToWin = 10;

    [Header("UI ( reopening)")]
    public TMP_Text countText;          // optional: show "3/10"
    public GameObject winPanel;         // optional: show a win panel

    [Header("Detection")]
    public string boxTag = "Box";       // tag your box prefabs as "Box"
    public bool requireTag = true;      // turn off if you prefer layer check instead

    [Header("Debug")]
    public bool debugLogs = true;

    private int boxesCount = 0;
    private bool won = false;

    void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
        UpdateUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (won) return;

        // Filter: only count boxes
        if (requireTag)
        {
            if (!other.CompareTag(boxTag)) return;
        }

        // Prevent double-counting the same box if it re-enters the trigger
        // by marking it once it’s counted.
        CountedOnce marker = other.GetComponent<CountedOnce>();
        if (marker == null)
        {
            marker = other.gameObject.AddComponent<CountedOnce>();
        }

        if (marker.counted) return;

        marker.counted = true;

        boxesCount++;
        UpdateUI();

        if (debugLogs)
            Debug.Log($"[BoxChecker] Counted box: {boxesCount}/{boxesToWin} ({other.name})");

        if (boxesCount >= boxesToWin)
        {
            Win();
        }
    }

    void Win()
    {
        won = true;

        if (debugLogs)
            Debug.Log("[BoxChecker] WIN!");

        if (winPanel != null)
            winPanel.SetActive(true);

        // Optional: pause game
        // Time.timeScale = 0f;
    }

    void UpdateUI()
    {
        if (countText != null)
            countText.text = $"{boxesCount}/{boxesToWin}";
    }

    // Optional: call this if you want to restart counting
    public void ResetCounter()
    {
        boxesCount = 0;
        won = false;

        if (winPanel != null) winPanel.SetActive(false);
        UpdateUI();
    }

    public int GetCount() => boxesCount;
}

// Helper component to stop counting the same box multiple times
public class CountedOnce : MonoBehaviour
{
    public bool counted = false;
}
