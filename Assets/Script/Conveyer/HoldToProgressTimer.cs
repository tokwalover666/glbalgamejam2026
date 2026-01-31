using UnityEngine;

public class HoldToProgressTimer : MonoBehaviour
{
    [Header("Timing")]
    public float holdDuration = 2.5f; // seconds required to complete

    [Header("Box Logic")]
    public BoxMove boxMove;

    [Header("Debug")]
    public bool debugLogs = true;
    public float debugTickRate = 0.25f; // logs progress every X seconds while holding

    private float holdTimer = 0f;
    private bool isHolding = false;
    private bool completed = false;

    private float nextDebugTime = 0f;

    void Update()
    {
        if (completed) return;

        if (isHolding)
        {
            holdTimer += Time.deltaTime;

            // Debug: progress tick
            if (debugLogs && Time.time >= nextDebugTime)
            {
                nextDebugTime = Time.time + debugTickRate;
                Debug.Log($"[HOLD] Holding... {holdTimer:F2}/{holdDuration:F2} ({GetProgress01() * 100f:F0}%)");
            }

            // Finish
            if (holdTimer >= holdDuration)
            {
                completed = true;
                isHolding = false;

                if (debugLogs)
                    Debug.Log($"[HOLD] FINISHED! Timer reached {holdDuration:F2}s. Calling boxMove.GoToFinal()");

                if (boxMove != null)
                {
                    boxMove.GoToFinal();
                }
                else
                {
                    Debug.LogWarning("[HOLD] boxMove is NULL. Assign BoxMove in Inspector.");
                }
            }
        }
        // else: do nothing (timer pauses)
    }

    // UI Button -> EventTrigger -> PointerDown
    public void OnHoldStart()
    {
        if (completed)
        {
            if (debugLogs) Debug.Log("[HOLD] OnHoldStart called, but already completed.");
            return;
        }

        isHolding = true;
        nextDebugTime = Time.time; // so it logs immediately

        if (debugLogs)
            Debug.Log("[HOLD] START holding.");
    }

    // UI Button -> EventTrigger -> PointerUp
    // (also add PointerExit if you want to stop when cursor leaves button)
    public void OnHoldEnd()
    {
        if (!isHolding) return;

        isHolding = false;

        if (debugLogs)
            Debug.Log($"[HOLD] STOP holding at {holdTimer:F2}/{holdDuration:F2} ({GetProgress01() * 100f:F0}%).");
    }

    // Optional: reset if needed
    public void ResetHold()
    {
        holdTimer = 0f;
        completed = false;
        isHolding = false;

        if (debugLogs)
            Debug.Log("[HOLD] ResetHold() called. Timer reset to 0.");
    }

    public float GetProgress01()
    {
        return Mathf.Clamp01(holdTimer / holdDuration);
    }
}
