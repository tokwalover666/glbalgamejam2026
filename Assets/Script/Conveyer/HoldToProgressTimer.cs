using UnityEngine;

public class HoldToProgressTimer : MonoBehaviour
{
    [Header("Timing")]
    public float holdDuration = 2.5f; // seconds required to complete

    [Header("Box Logic")]
    public BoxMove boxMove;

    private float holdTimer = 0f;
    private bool isHolding = false;
    private bool completed = false;

    void Update()
    {
        if (completed) return;

        if (isHolding)
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdDuration)
            {
                completed = true;
                boxMove.GoToFinal();
            }
        }
        // else: do nothing  timer pauses
    }

    // UI Button  Pointer Down
    public void OnHoldStart()
    {
        isHolding = true;
    }

    // UI Button  Pointer Up
    public void OnHoldEnd()
    {
        isHolding = false;
    }

    // Optional: reset if needed
    public void ResetHold()
    {
        holdTimer = 0f;
        completed = false;
    }

    // Debug helper (optional)
    public float GetProgress01()
    {
        return Mathf.Clamp01(holdTimer / holdDuration);
    }
}
