using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterMoveB : MonoBehaviour
{
    [Header("Starts after Character A finishes")]
    public CharacterMove characterA;

    [Header("Waypoints")]
    public Transform firstWaypoint;
    public Transform lastWaypoint;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float arriveDistance = 0.1f;

    [Header("Rotation at First Waypoint")]
    public Vector3 firstRotateEuler = new Vector3(0, 90, 0);

    [Header("Rotation at Final Waypoint")]
    public Vector3 finalRotateEuler = new Vector3(0, 180, 0);

    public float rotationSpeed = 180f;

    [Header("Wait Times")]
    public float waitAfterFirstRotate = 5f;
    public float waitAfterRotateBack = 5f;
    public float waitAfterFinalRotate = 0f;

    [Header("Animation")]
    public Animator animator;
    public string isMovingBool = "IsMoving"; // Bool parameter in Animator

    [Header("Debug")]
    public bool debugLogs = false;

    public bool finished { get; private set; }

    private Quaternion originalRotation;
    private Quaternion firstTargetRotation;
    private Quaternion finalTargetRotation;

    private float timer;
    private bool lastIsMoving = false;

    private enum State
    {
        WaitingForA,
        MoveToFirst,
        RotateAtFirst,
        WaitAfterFirstRotate,
        RotateBack,
        WaitAfterRotateBack,
        MoveToLast,
        RotateAtFinal,
        WaitAfterFinalRotate,
        Done
    }

    private State state = State.WaitingForA;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Start()
    {
        finished = false;

        originalRotation = transform.rotation;
        firstTargetRotation = Quaternion.Euler(firstRotateEuler);
        finalTargetRotation = Quaternion.Euler(finalRotateEuler);

        SetIsMoving(false);
    }

    void Update()
    {
        switch (state)
        {
            case State.WaitingForA:
                SetIsMoving(false);
                if (characterA != null && characterA.finished)
                    state = State.MoveToFirst;
                break;

            case State.MoveToFirst:
                SetIsMoving(true);
                if (firstWaypoint == null) return;

                MoveTo(firstWaypoint.position);

                if (Arrived(firstWaypoint.position))
                    state = State.RotateAtFirst;
                break;

            case State.RotateAtFirst:
                SetIsMoving(false);
                RotateTowards(firstTargetRotation);

                if (RotationFinished(firstTargetRotation))
                {
                    timer = 0f;
                    state = State.WaitAfterFirstRotate;
                }
                break;

            case State.WaitAfterFirstRotate:
                SetIsMoving(false);
                timer += Time.deltaTime;
                if (timer >= waitAfterFirstRotate)
                    state = State.RotateBack;
                break;

            case State.RotateBack:
                SetIsMoving(false);
                RotateTowards(originalRotation);

                if (RotationFinished(originalRotation))
                {
                    timer = 0f;
                    state = State.WaitAfterRotateBack;
                }
                break;

            case State.WaitAfterRotateBack:
                SetIsMoving(false);
                timer += Time.deltaTime;
                if (timer >= waitAfterRotateBack)
                    state = State.MoveToLast;
                break;

            case State.MoveToLast:
                SetIsMoving(true);
                if (lastWaypoint == null) return;

                MoveTo(lastWaypoint.position);

                if (Arrived(lastWaypoint.position))
                    state = State.RotateAtFinal;
                break;

            case State.RotateAtFinal:
                SetIsMoving(false);
                RotateTowards(finalTargetRotation);

                if (RotationFinished(finalTargetRotation))
                {
                    timer = 0f;
                    state = State.WaitAfterFinalRotate;
                }
                break;

            case State.WaitAfterFinalRotate:
                SetIsMoving(false);
                timer += Time.deltaTime;
                if (timer >= waitAfterFinalRotate)
                {
                    finished = true;
                    state = State.Done;
                    SetIsMoving(false);
                    enabled = false; // optional
                }
                break;

            case State.Done:
                SetIsMoving(false);
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

    void SetIsMoving(bool moving)
    {
        if (animator == null) return;

        // Only set when it actually changes (prevents spamming transitions)
        if (moving == lastIsMoving) return;

        lastIsMoving = moving;
        animator.SetBool(isMovingBool, moving);

        if (debugLogs)
            Debug.Log($"[CharacterMoveB] {name} IsMoving -> {moving} (state={state})");
    }
}
