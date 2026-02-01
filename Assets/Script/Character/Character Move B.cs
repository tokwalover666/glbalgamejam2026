using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterMoveB : MonoBehaviour
{
    [Header("Wait for Character A to finish first")]
    public CharacterMove characterA;

    [Header("Waypoints")]
    public Transform firstWaypoint;
    public Transform lastWaypoint;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float arriveDistance = 0.1f;

    [Header("Rotations at First Waypoint")]
    public Vector3 firstRotateEuler = new Vector3(0, 90, 0);
    public float waitBetweenRotations = 2f;
    public Vector3 secondRotateEuler = new Vector3(0, 0, 0);
    public float rotationSpeed = 180f;

    [Header("End Rotation (used before new route starts)")]
    public Vector3 endRotateEuler = new Vector3(0, 180, 0);
    public float endRotationSpeed = 180f;

    [Header("Loop Range")]
    public int minExtraLoops = 1;
    public int maxExtraLoops = 5;

    [Header("Delays")]
    public float afterFirstRunDelay = 5f;
    public float minWaitAtLast = 3f;
    public float maxWaitAtLast = 5f;

    [Header("Spot Check - Working Requirement")]
    public float checkDuration = 5f;
    public float requiredWorkingTime = 3f;

    [Header("Animation")]
    public Animator animator;
    public string isMovingBool = "IsMoving";
    public string spottedTrigger = "Spotted";

    [Header("GameManager")]
    public GameManager gameManager;

    [Header("Debug")]
    public bool debugLogs = false;

    public int i { get; private set; }
    public bool firstRunCompleted { get; private set; }

    private Quaternion rot1;
    private Quaternion rot2;
    private Quaternion endRot;

    private float timer;
    private float currentWait;
    private bool lastIsMoving;
    private bool systemsStarted = false;

    private float checkTimer = 0f;
    private float workingTimer = 0f;

    private bool checkPassedThisLoop = false;
    private bool firstArrivalHandled = false;

    private enum State
    {
        WaitingForA,
        MoveToFirst,
        CheckAtFirst,
        Rotate1,
        WaitBetween,
        Rotate2,
        MoveToLast,
        WaitAfterLast,
        WaitAfterFirstRun,

        RotateToEndBeforeRoute, // ✅ NEW: rotate before starting a new route

        OnLoopEnd,
        Lose
    }

    private State state = State.WaitingForA;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        animator.applyRootMotion = false;

        rot1 = Quaternion.Euler(firstRotateEuler);
        rot2 = Quaternion.Euler(secondRotateEuler);
        endRot = Quaternion.Euler(endRotateEuler);

        SetIsMoving(false);
    }

    void Start()
    {
        firstRunCompleted = false;
        i = 0;
    }

    void Update()
    {
        // live tweak support
        rot1 = Quaternion.Euler(firstRotateEuler);
        rot2 = Quaternion.Euler(secondRotateEuler);
        endRot = Quaternion.Euler(endRotateEuler);

        // Reset arrival latch only when far enough away from first waypoint
        if (firstWaypoint != null &&
            Vector3.Distance(transform.position, firstWaypoint.position) > arriveDistance * 2f)
        {
            firstArrivalHandled = false;
        }

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

                if (!firstArrivalHandled && Arrived(firstWaypoint.position))
                {
                    firstArrivalHandled = true;
                    SetIsMoving(false);

                    if (!firstRunCompleted)
                    {
                        state = State.Rotate1;
                    }
                    else
                    {
                        if (!checkPassedThisLoop)
                        {
                            checkTimer = 0f;
                            workingTimer = 0f;
                            state = State.CheckAtFirst;
                        }
                        else
                        {
                            state = State.Rotate1;
                        }
                    }
                }
                break;

            case State.CheckAtFirst:
                SetIsMoving(false);

                if (characterA == null)
                {
                    TriggerSpottedAndLose();
                    break;
                }

                checkTimer += Time.deltaTime;

                if (characterA.IsWorking())
                    workingTimer += Time.deltaTime;

                if (workingTimer >= requiredWorkingTime)
                {
                    checkPassedThisLoop = true;
                    state = State.Rotate1;
                }
                else if (checkTimer >= checkDuration)
                {
                    TriggerSpottedAndLose();
                }
                break;

            case State.Rotate1:
                SetIsMoving(false);
                RotateTowards(rot1, rotationSpeed);

                if (RotationFinished(rot1))
                {
                    timer = 0f;
                    state = State.WaitBetween;
                }
                break;

            case State.WaitBetween:
                SetIsMoving(false);
                timer += Time.deltaTime;

                if (timer >= waitBetweenRotations)
                    state = State.Rotate2;
                break;

            case State.Rotate2:
                SetIsMoving(false);
                RotateTowards(rot2, rotationSpeed);

                if (RotationFinished(rot2))
                    state = State.MoveToLast;
                break;

            case State.MoveToLast:
                SetIsMoving(true);
                if (lastWaypoint == null) return;

                MoveTo(lastWaypoint.position);

                if (Arrived(lastWaypoint.position))
                {
                    SetIsMoving(false);

                    timer = 0f;
                    currentWait = Random.Range(minWaitAtLast, maxWaitAtLast);
                    state = State.WaitAfterLast;
                }
                break;

            case State.WaitAfterLast:
                SetIsMoving(false);
                timer += Time.deltaTime;

                if (timer >= currentWait)
                {
                    state = State.OnLoopEnd;
                }
                break;

            case State.OnLoopEnd:
                SetIsMoving(false);

                if (!firstRunCompleted)
                {
                    // ✅ First full route ended
                    firstRunCompleted = true;

                    if (!systemsStarted)
                    {
                        systemsStarted = true;
                        gameManager?.StartGameplaySystems();
                    }

                    // Delay before looping
                    timer = 0f;
                    state = State.WaitAfterFirstRun;
                }
                else
                {
                    // loop count
                    i--;

                    if (i > 0)
                    {
                        // ✅ before starting new route, rotate to end
                        state = State.RotateToEndBeforeRoute;
                    }
                    else
                    {
                        enabled = false;
                    }
                }
                break;

            case State.WaitAfterFirstRun:
                SetIsMoving(false);
                timer += Time.deltaTime;

                if (timer >= afterFirstRunDelay)
                {
                    // ✅ activate looping count
                    i = Random.Range(minExtraLoops, maxExtraLoops + 1);

                    // ✅ before starting first loop route, rotate to end
                    state = State.RotateToEndBeforeRoute;
                }
                break;

            case State.RotateToEndBeforeRoute:
                SetIsMoving(false);
                RotateTowards(endRot, endRotationSpeed);

                if (RotationFinished(endRot))
                {
                    // reset per-loop flags before starting the route again
                    checkPassedThisLoop = false;
                    firstArrivalHandled = false;

                    state = State.MoveToFirst;
                }
                break;

            case State.Lose:
                break;
        }
    }

    void TriggerSpottedAndLose()
    {
        SetIsMoving(false);

        if (animator != null && !string.IsNullOrEmpty(spottedTrigger))
            animator.SetTrigger(spottedTrigger);

        gameManager?.TriggerLose();
        state = State.Lose;
    }

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

    void RotateTowards(Quaternion targetRot, float speed)
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            speed * Time.deltaTime
        );
    }

    bool RotationFinished(Quaternion targetRot)
    {
        return Quaternion.Angle(transform.rotation, targetRot) < 1f;
    }

    void SetIsMoving(bool moving)
    {
        if (animator == null) return;
        if (moving == lastIsMoving) return;

        lastIsMoving = moving;
        animator.SetBool(isMovingBool, moving);
    }
}
