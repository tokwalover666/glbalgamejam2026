using UnityEngine;

public class HoldToProgressTimer : MonoBehaviour
{
    [Header("Timing")]
    public float holdDuration = 2.5f;

    [Header("References")]
    public BoxMove boxMove;          // active box (waiting at checkpoint)
    public TopViewPanelUI ui;        // assign in inspector

    [Header("Character")]
    public CharacterMove characterA; // for working animation

    [Header("Debug")]
    public bool debugLogs = true;

    private float holdTimer = 0f;
    private bool isHolding = false;
    private bool completed = false;

    void Update()
    {
        if (completed) return;
        if (!isHolding) return;

        // Only progress if the box is actually waiting
        if (boxMove == null || boxMove.state != BoxMove.BoxState.Wait)
            return;

        holdTimer += Time.deltaTime;

        if (debugLogs)
            Debug.Log($"[HOLD] {holdTimer:F2}/{holdDuration:F2} ({GetProgress01() * 100f:F0}%) on {boxMove.name}");

        if (holdTimer >= holdDuration)
        {
            completed = true;
            isHolding = false;

            if (debugLogs) Debug.Log("[HOLD] FINISHED -> swap prefab + send to final");

            // stop working animation
            characterA?.SetWorking(false);

            // hide UI
            ui?.HideBoth();

            // ✅ SWAP PREFAB HERE (this swaps THIS instance, at its current position)
            boxMove.SwapToCompletedPrefabAndContinue();

            // clear reference (old box will be destroyed)
            boxMove = null;
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

    /// <summary>
    /// Call this whenever a new box reaches the checkpoint and becomes the active box.
    /// This resets the hold so you can work on the next box.
    /// </summary>
    public void SetActiveBox(BoxMove newBox)
    {
        boxMove = newBox;

        holdTimer = 0f;
        isHolding = false;
        completed = false;

        if (debugLogs)
            Debug.Log($"[HOLD] SetActiveBox -> {newBox?.name ?? "NULL"} (reset hold)");
    }

    public float GetProgress01()
    {
        return Mathf.Clamp01(holdTimer / holdDuration);
    }
}
