using UnityEngine;

public class BoxMover : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform firstWaypoint;
    public Transform lastWaypoint;

    public float speed = 3f;
    public float arriveDistance = 0.1f;

    private System.Action onFinished;

    // NEW: states
    private enum State { MoveToFirst, Wait, MoveToLast }
    private State state = State.MoveToFirst;

    public void Init(Transform first, Transform last, System.Action finishedCallback)
    {
        firstWaypoint = first;
        lastWaypoint = last;
        onFinished = finishedCallback;

        state = State.MoveToFirst; // reset state on spawn
    }

    void Update()
    {
        if (firstWaypoint == null || lastWaypoint == null) return;

        switch (state)
        {
            case State.MoveToFirst:
                MoveTo(firstWaypoint.position);

                if (Arrived(firstWaypoint.position))
                {
                    state = State.Wait;

                    // Open hold UI and register THIS box as the active one
                    GameManager.Instance?.OpenHoldForBox(this);
                }
                break;

            case State.Wait:
                // do nothing, wait for player hold to complete
                break;

            case State.MoveToLast:
                MoveTo(lastWaypoint.position);

                if (Arrived(lastWaypoint.position))
                {
                    Destroy(gameObject);
                }
                break;
        }
    }

    public void ContinueToLast()
    {
        // Called by GameManager when hold completes
        if (state == State.Wait)
        {
            state = State.MoveToLast;
        }
    }

    void MoveTo(Vector3 target)
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

    void OnDestroy()
    {
        // Notify spawner that this active box is gone
        onFinished?.Invoke();
    }
}
