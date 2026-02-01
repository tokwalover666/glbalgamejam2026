using UnityEngine;
using TMPro;

public class BoxChecker : MonoBehaviour
{
    [Header("Win Condition")]
    public int boxesToWin = 10;

    [Header("UI")]
    public TMP_Text countText;      // shows "3/10"
    public GameObject winPanel;     // shows win panel

    [Header("Detection")]
    public string boxTag = "Box";   // tag your box prefabs as "Box"
    public bool requireTag = true;  // if false, it counts anything entering

    [Header("Debug")]
    public bool debugLogs = true;

    private int boxesCount = 0;
    private bool won = false;

    void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
        UpdateUI();

        if (debugLogs)
            Debug.Log($"[BoxChecker] START | count={boxesCount}/{boxesToWin} | triggerObj={gameObject.name}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (won) return;

        // Filter: only count boxes
        if (requireTag && !other.CompareTag(boxTag))
            return;

        // Prevent double-counting the same box
        CountedOnce marker = other.GetComponent<CountedOnce>();
        if (marker == null)
            marker = other.gameObject.AddComponent<CountedOnce>();

        if (marker.counted) return;

        marker.counted = true;

        // ✅ FIX: increment count
        boxesCount++;

        UpdateUI();

        if (debugLogs)
            Debug.Log($"[BoxChecker] Counted box: {boxesCount}/{boxesToWin} ({other.name})");

        if (boxesCount >= boxesToWin)
            Win();
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

        if (debugLogs)
            Debug.Log("[BoxChecker] ResetCounter()");
    }

    public int GetCount() => boxesCount;
}

// Helper component to stop counting the same box multiple times
public class CountedOnce : MonoBehaviour
{
    public bool counted = false;
}
