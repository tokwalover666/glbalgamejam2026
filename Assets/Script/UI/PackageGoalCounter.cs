using UnityEngine;
using TMPro;

public class PackageGoalCounter : MonoBehaviour
{
    [Header("Condition")]
    public int requiredCount = 10;   // change to 5, 10, etc.
    public string packageTag = "Package";

    [Header("UI")]
    public TMP_Text counterText;
    public GameObject winPanel; // optional (can be null)

    private int currentCount = 0;
    private bool hasWon = false;

    void Start()
    {
        UpdateText();

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasWon) return;

        if (!other.CompareTag(packageTag)) return;

        currentCount++;
        UpdateText();

        // Optional: prevent same package from counting again
        other.enabled = false;

        if (currentCount >= requiredCount)
        {
            PlayerWin();
        }
    }

    void UpdateText()
    {
        if (counterText != null)
        {
            counterText.text = $"{currentCount}/{requiredCount}";
        }
    }

    void PlayerWin()
    {
        hasWon = true;
        Debug.Log("PLAYER WINS!");

        if (winPanel != null)
            winPanel.SetActive(true);

        // Optional: stop time
         Time.timeScale = 0f;
    }
}
