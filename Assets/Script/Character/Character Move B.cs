using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterMoveB : MonoBehaviour
{
    [Header("Dependency")]
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

    private Animator animator;
    public bool isMoving { get; private set; }

    Quaternion rotA;
    Quaternion rotB;
    float timer;

    enum State
    {
        WaitingForA,
        MoveToWait,
        RotateFirst,
        Wait,
        RotateSecond,
        MoveToLast,
        Done
    }

    State state = State.WaitingForA;

    void Start()
    {
        animator = GetComponent<Animator>(); // important
        rotA = Quaternion.Euler(firstRotateEuler);
        rotB = Quaternion.Euler(secondRotateEuler);

        // optional: make sure it starts idle
        animator.SetBool("isMoving", false);
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
                    state = State.RotateFirst;
                break;

            case State.RotateFirst:
                isMoving = false;
                Rotate(rotA);
                if (RotationFinished(rotA))
                {
                    timer = 0f;
                    state = State.Wait;
                }
                break;

            case State.Wait:
                isMoving = false;
                timer += Time.deltaTime;
                if (timer >= waitAfterFirstRotate)
                    state = State.RotateSecond;
                break;

            case State.RotateSecond:
                isMoving = false;
                Rotate(rotB);
                if (RotationFinished(rotB))
                    state = State.MoveToLast;
                break;

            case State.MoveToLast:
                isMoving = true;
                MoveTo(lastWaypoint.position);
                if (Arrived(lastWaypoint.position))
                    state = State.Done;
                break;

            case State.Done:
                isMoving = false;
                enabled = false;
                break;
        }

        animator.SetBool("isMoving", isMoving);
    }

    void MoveTo(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );
    }

    void Rotate(Quaternion target)
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            target,
            rotationSpeed * Time.deltaTime
        );
    }

    bool Arrived(Vector3 target)
    {
        return Vector3.Distance(transform.position, target) <= arriveDistance;
    }

    bool RotationFinished(Quaternion target)
    {
        return Quaternion.Angle(transform.rotation, target) < 1f;
    }
}
