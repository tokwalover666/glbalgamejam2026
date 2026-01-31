using UnityEngine;

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

    public enum BoxState
    {
        MoveToCheckpoint,
        Wait,
        MoveToFinal
    }

    public BoxState state = BoxState.MoveToCheckpoint;

    // Called by spawner or other scripts after Instantiate()
    public void Init(Transform checkpointTarget, Transform finalTarget, TopViewPanelUI uiRef)
    {
        checkpoint = checkpointTarget;
        finalPoint = finalTarget;
        ui = uiRef;

        state = BoxState.MoveToCheckpoint;
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
                    ui?.ShowBoth();
                }
                break;

            case BoxState.Wait:
                // waits until button calls GoToFinal()
                break;

            case BoxState.MoveToFinal:
                Move(finalPoint.position);

                if (Arrived(finalPoint.position))
                {
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

    // Call from UI button
    public void GoToFinal()
    {
        if (state == BoxState.Wait)
        {
            state = BoxState.MoveToFinal;
            ui?.HideBoth();
        }
    }

    //  Optional: call this if you want to reopen the panel while waiting
    public void ReOpenPanelIfWaiting()
    {
        if (state == BoxState.Wait)
        {
            ui?.ShowBoth();
        }
    }
}
