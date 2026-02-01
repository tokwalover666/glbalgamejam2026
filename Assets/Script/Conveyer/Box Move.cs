using UnityEngine;
using System;

public class BoxMove : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform checkpoint;
    public Transform finalPoint;

    [Header("Movement")]
    public float speed = 3f;
    public float arriveDistance = 0.05f;

    [Header("UI Panels")]
    public TopViewPanelUI ui;

    [Header("Swap Prefab (taped box)")]
    public GameObject completedBoxPrefab; // assign your taped box prefab here

    public enum BoxState
    {
        MoveToCheckpoint,
        Wait,
        MoveToFinal
    }

    public BoxState state = BoxState.MoveToCheckpoint;

    // Spawner subscribes to this
    public event Action<BoxMove> OnReachedFinal;

    // ✅ store handler so we can reattach it after swap
    private Action<BoxMove> cachedReachedFinalHandler;

    public void Init(Transform checkpointTarget, Transform finalTarget, TopViewPanelUI uiRef)
    {
        checkpoint = checkpointTarget;
        finalPoint = finalTarget;
        ui = uiRef;
        state = BoxState.MoveToCheckpoint;
    }

    // ✅ Spawner calls this after subscribing so swaps keep working
    public void CacheReachedFinalHandler(Action<BoxMove> handler)
    {
        cachedReachedFinalHandler = handler;
    }

    void Update()
    {
        if (checkpoint == null || finalPoint == null) return;

        switch (state)
        {
            case BoxState.MoveToCheckpoint:
                Move(checkpoint.position);

                if (Arrived(checkpoint.position))
                {
                    state = BoxState.Wait;
                    ui?.ShowBoth(this);
                }
                break;

            case BoxState.Wait:
                // waits until hold completes
                break;

            case BoxState.MoveToFinal:
                Move(finalPoint.position);

                if (Arrived(finalPoint.position))
                {
                    OnReachedFinal?.Invoke(this);
                    Destroy(gameObject);
                }
                break;
        }
    }

    void Move(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );
    }

    bool Arrived(Vector3 target)
    {
        return Vector3.Distance(transform.position, target) <= arriveDistance;
    }

    public void GoToFinal()
    {
        if (state == BoxState.Wait)
        {
            state = BoxState.MoveToFinal;
            ui?.HideBoth();
        }
    }

    /// <summary>
    /// Swap THIS box into completed prefab and continue moving to final.
    /// IMPORTANT: re-attach spawner handler to the new box.
    /// </summary>
    public void SwapToCompletedPrefabAndContinue()
    {
        if (completedBoxPrefab == null)
        {
            Debug.LogWarning("[BoxMove] completedBoxPrefab is not assigned. Using current box instead.");
            GoToFinal();
            return;
        }

        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        Transform parent = transform.parent;

        GameObject newBox = Instantiate(completedBoxPrefab, pos, rot, parent);

        BoxMove newMove = newBox.GetComponent<BoxMove>();
        if (newMove != null)
        {
            newMove.Init(checkpoint, finalPoint, ui);

            // ✅ keep spawner callback alive after swap
            if (cachedReachedFinalHandler != null)
            {
                newMove.OnReachedFinal += cachedReachedFinalHandler;
                newMove.CacheReachedFinalHandler(cachedReachedFinalHandler);
            }

            // Skip waiting, go straight to final
            newMove.state = BoxState.MoveToFinal;
            ui?.HideBoth();
        }
        else
        {
            Debug.LogWarning("[BoxMove] Completed prefab has no BoxMove component.");
        }

        Destroy(gameObject);
    }
}
