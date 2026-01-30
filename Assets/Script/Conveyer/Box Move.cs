using UnityEngine;
using UnityEngine.UIElements;

public class BoxMove : MonoBehaviour
{
    public Transform checkpoint;
    public Transform finalPoint;
    public float speed = 3f;
    public float arriveDistance = 0.05f;

    public enum BoxState
    {
        MoveToCheckpoint,
        Wait,
        MoveToFinal
    }

    public BoxState state = BoxState.MoveToCheckpoint;

    void Update()
    {
        switch (state)
        {
            case BoxState.MoveToCheckpoint:
                Move(checkpoint.position);

                if (Arrived(checkpoint.position))
                {
                    state = BoxState.Wait;
                }
                break;

            case BoxState.Wait:
                // do nothing, external systems (UI, trigger, etc.)
                // will change the state
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

    // Call this from UI / trigger / other script later
    public void GoToFinal()
    {
        if (state == BoxState.Wait)
        {
            state = BoxState.MoveToFinal;
        }
    }
}
