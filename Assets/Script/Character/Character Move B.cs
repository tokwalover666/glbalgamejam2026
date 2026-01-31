using UnityEngine;

public class CharacterMoveB : MonoBehaviour
{
    [Header("Start Condition (Character A)")]
    public CharacterMove characterA;

    [Header("Waypoints")]
    public Transform waitWaypoint;
    public Transform lastWaypoint;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float arriveDistance = 0.1f;

    [Header("Rotations")]
    public Vector3 firstRotateEuler = new Vector3(0, 90, 0);
    public Vector3 secondRotateEuler = new Vector3(0, 0, 0);
    public float rotationSpeed = 180f;
    public float waitAfterFirstRotate = 2f;

    //  NEW
    public bool isMoving { get; private set; }
    Animator animator;

    float timer = 0f;
    Quaternion firstRotation;
    Quaternion secondRotation;

    enum State
    {
        WaitingForA,
        MoveToWait,
        RotateFirst,
        WaitAfterFirstRotate,
        RotateSecond,
        MoveToLast,
        Done
    }

    State state = State.WaitingForA;

    void Start()
    {
        animator = GetComponent<Animator>();
        firstRotation = Quaternion.Euler(firstRotateEuler);
        secondRotation = Quaternion.Euler(secondRotateEuler);
        isMoving = false;
    }

    void Update()
    {
        switch (state)
        {
            case State.WaitingForA:
                isMoving = false;

                if (characterA != null && characterA.finished)
                    state = State.MoveToWait;
                break;

            case State.MoveToWait:
                isMoving = true;
                MoveTo(waitWaypoint.position);

                if (Arrived(waitWaypoint.position))
                {
                    isMoving = false;
                    state = State.RotateFirst;
                }
                break;

            case State.RotateFirst:
                isMoving = false;
                RotateTowards(firstRotation);

                if (RotationFinished(firstRotation))
                {
                    timer = 0f;
                    state = State.WaitAfterFirstRotate;
                }
                break;

            case State.WaitAfterFirstRotate:
                isMoving = false;
                timer += Time.deltaTime;

                if (timer >= waitAfterFirstRotate)
                    state = State.RotateSecond;
                break;

            case State.RotateSecond:
                isMoving = false;
                RotateTowards(secondRotation);

                if (RotationFinished(secondRotation))
                    state = State.MoveToLast;
                break;

            case State.MoveToLast:
                isMoving = true;
                MoveTo(lastWaypoint.position);

                if (Arrived(lastWaypoint.position))
                {
                    isMoving = false;
                    state = State.Done;
                }
                break;

            case State.Done:
                isMoving = false;
                enabled = false;
                break;
        }

        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
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
