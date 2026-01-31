using UnityEngine;

public class BossMove : MonoBehaviour
{
    [Header("Starts after Character A finishes")]
    public CharacterMove characterA;

    [Header("Waypoints")]
    public Transform firstWaypoint;
    public Transform lastWaypoint;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float arriveDistance = 0.1f;

    [Header("Rotation")]
    public Vector3 rotateToEuler = new Vector3(0, 90, 0);
    public float rotationSpeed = 180f;

    [Header("Wait Times")]
    public float waitAfterRotate = 5f;
    public float waitAfterRotateBack = 5f;

    Quaternion originalRotation;
    Quaternion targetRotation;
    float timer;

    enum State
    {
        WaitingForA,
        MoveToFirst,
        RotateToTarget,
        WaitAfterRotate,
        RotateBack,
        WaitAfterRotateBack,
        MoveToLast,
        Done
    }

    State state = State.WaitingForA;

    void Start()
    {
        originalRotation = transform.rotation;
        targetRotation = Quaternion.Euler(rotateToEuler);
    }

    void Update()
    {
        switch (state)
        {
            case State.WaitingForA:
                if (characterA != null && characterA.finished)
                    state = State.MoveToFirst;
                break;

            case State.MoveToFirst:
                MoveTo(firstWaypoint.position);

                if (Arrived(firstWaypoint.position))
                {
                    state = State.RotateToTarget;
                }
                break;

            case State.RotateToTarget:
                RotateTowards(targetRotation);

                if (RotationFinished(targetRotation))
                {
                    timer = 0f;
                    state = State.WaitAfterRotate;
                }
                break;

            case State.WaitAfterRotate:
                timer += Time.deltaTime;
                if (timer >= waitAfterRotate)
                {
                    state = State.RotateBack;
                }
                break;

            case State.RotateBack:
                RotateTowards(originalRotation);

                if (RotationFinished(originalRotation))
                {
                    timer = 0f;
                    state = State.WaitAfterRotateBack;
                }
                break;

            case State.WaitAfterRotateBack:
                timer += Time.deltaTime;
                if (timer >= waitAfterRotateBack)
                {
                    state = State.MoveToLast;
                }
                break;

            case State.MoveToLast:
                MoveTo(lastWaypoint.position);

                if (Arrived(lastWaypoint.position))
                {
                    state = State.Done;
                    enabled = false;
                }
                break;
        }
    }

    // =====================
    // Helpers
    // =====================

    void MoveTo(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );
    }

    bool Arrived(Vector3 target)
    {
        return Vector3.Distance(transform.position, target) <= arriveDistance;
    }

    void RotateTowards(Quaternion targetRot)
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }

    bool RotationFinished(Quaternion targetRot)
    {
        return Quaternion.Angle(transform.rotation, targetRot) < 1f;
    }
}
