using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform firstWaypoint;
    public Transform lastWaypoint;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float arriveDistance = 0.1f;

    [Header("Rotation")]
    public Vector3 rotateToEuler = new Vector3(0, 90, 0);
    public float rotationSpeed = 180f;

    public bool finished { get; private set; } = false; // added

    enum State
    {
        MoveToFirst,
        RotateAtFirst,
        MoveToLast,
        Done
    }

    State state = State.MoveToFirst;

    void Update()
    {
        if (finished) return;

        switch (state)
        {
            case State.MoveToFirst:
                MoveTo(firstWaypoint.position);

                if (Arrived(firstWaypoint.position))
                    state = State.RotateAtFirst;
                break;

            case State.RotateAtFirst:
                RotateInPlace();

                if (RotationFinished())
                    state = State.MoveToLast;
                break;

            case State.MoveToLast:
                MoveTo(lastWaypoint.position);

                if (Arrived(lastWaypoint.position))
                {
                    state = State.Done;
                    finished = true; // signal done
                }
                break;
        }
    }

    void MoveTo(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    bool Arrived(Vector3 target)
    {
        return Vector3.Distance(transform.position, target) <= arriveDistance;
    }

    void RotateInPlace()
    {
        Quaternion targetRot = Quaternion.Euler(rotateToEuler);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }

    bool RotationFinished()
    {
        Quaternion targetRot = Quaternion.Euler(rotateToEuler);
        return Quaternion.Angle(transform.rotation, targetRot) < 1f;
    }
}
