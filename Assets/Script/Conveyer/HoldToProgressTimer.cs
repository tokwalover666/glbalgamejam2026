using UnityEngine;

public class HoldToProgressTimer : MonoBehaviour
{
    [Header("Timing")]
    public float holdDuration = 2.5f;

    [Header("References")]
    public BoxMove boxMove;          // assigned when box reaches checkpoint
    public TopViewPanelUI ui;        // assign in inspector (same UI that shows/hides)

    [Header("Debug")]
    public bool debugLogs = true;

    private float holdTimer = 0f;
    private bool isHolding = false;
    private bool completed = false;

    public CharacterMove characterA;

    void Update()
    {
        if (completed) return;
        if (!isHolding) return;

        // Only progress if the box is actually waiting
        if (boxMove == null || boxMove.state != BoxMove.BoxState.Wait)
            return;

        holdTimer += Time.deltaTime;

        if (debugLogs)
            Debug.Log($"[HOLD] {holdTimer:F2}/{holdDuration:F2} ({GetProgress01() * 100f:F0}%)");

        if (holdTimer >= holdDuration)
        {
            completed = true;
            isHolding = false;

            if (debugLogs) Debug.Log("[HOLD] FINISHED -> hide UI + box GoToFinal");

            ui?.HideBoth();        // hide panel
            boxMove.GoToFinal();   // move box again
        }
    }

    public void OnHoldStart()
    {
        if (completed) return;

        if (boxMove == null)
        {
            if (debugLogs) Debug.LogWarning("[HOLD] Can't hold: boxMove is null.");
            return;
        }

        if (boxMove.state != BoxMove.BoxState.Wait)
        {
            if (debugLogs) Debug.LogWarning($"[HOLD] Can't hold. Box state is {boxMove.state}, needs Wait.");
            return;
        }

        isHolding = true;

        if (debugLogs) Debug.Log("[HOLD] START");

        characterA?.SetWorking(true);
    }

    public void OnHoldEnd()
    {
        if (!isHolding) return;

        isHolding = false;

        if (debugLogs)
            Debug.Log($"[HOLD] STOP at {holdTimer:F2}/{holdDuration:F2} ({GetProgress01() * 100f:F0}%)");

        characterA?.SetWorking(false);
    }

    public void ResetHold()
    {
        holdTimer = 0f;
        isHolding = false;
        completed = false;

        if (debugLogs) Debug.Log("[HOLD] Reset");
    }

    public float GetProgress01()
    {
        return Mathf.Clamp01(holdTimer / holdDuration);
    }
}
