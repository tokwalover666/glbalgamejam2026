using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterMoveB : MonoBehaviour
{
    [Header("Wait for Character A to finish first")]
    public CharacterMove characterA; // Character A script (must have finished + IsWorking())

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

    [Header("Loop Range (public i range)")]
    public int minExtraLoops = 1;
    public int maxExtraLoops = 5;

    [Header("Delays")]
    public float afterFirstRunDelay = 5f;  // wait AFTER first full route, systems start during this wait
    public float minWaitAtLast = 3f;        // wait at last waypoint (min)
    public float maxWaitAtLast = 5f;        // wait at last waypoint (max)

    [Header("Animation")]
    public Animator animator;
    public string isMovingBool = "IsMoving";
    public string spottedTrigger = "Spotted";

    [Header("GameManager")]
    public GameManager gameManager; // must have StartGameplaySystems() + TriggerLose()

    [Header("Debug")]
    public bool debugLogs = false;

    // Public read
    public int i { get; private set; }                 // loops remaining (after-first-run only)
    public bool firstRunCompleted { get; private set; } // true after the first route is done

    // Internal
    private Quaternion rot1;
    private Quaternion rot2;

    private float timer;
    private float currentWait;
    private bool lastIsMoving;
    private bool systemsStarted = false;

    private enum State
    {
        WaitingForA,
        MoveToFirst,
        CheckAtFirst,        // only after first run
        Rotate1,
        WaitBetween,
        Rotate2,
        MoveToLast,
        WaitAfterLast,       // every time at last waypoint (3-5s)
        WaitAfterFirstRun,   // after first full route (5s) and systems start here
        OnLoopEnd,
        Lose
    }

    private State state = State.WaitingForA;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator != null)
            animator.applyRootMotion = false;

        rot1 = Quaternion.Euler(firstRotateEuler);
        rot2 = Quaternion.Euler(secondRotateEuler);

        SetIsMoving(false);
    }

    void Start()
    {
        firstRunCompleted = false;
        i = 0;
    }

    void Update()
    {
        // Update rotation targets (so you can tweak in inspector while playing)
        rot1 = Quaternion.Euler(firstRotateEuler);
        rot2 = Quaternion.Euler(secondRotateEuler);

        switch (state)
        {
            case State.WaitingForA:
                SetIsMoving(false);

                // ✅ Character B only starts when Character A finishes
                if (characterA != null && characterA.finished)
                {
                    state = State.MoveToFirst;
                    Log("A finished -> B starts MoveToFirst");
                }
                break;

            case State.MoveToFirst:
                SetIsMoving(true);
                if (firstWaypoint == null) return;

                MoveTo(firstWaypoint.position);

                if (Arrived(firstWaypoint.position))
                {
                    SetIsMoving(false);

                    // ✅ First run = no check
                    // ✅ After-first-run = check Character A IsWorking
                    state = firstRunCompleted ? State.CheckAtFirst : State.Rotate1;
                    Log(firstRunCompleted ? "Arrived First -> CheckAtFirst" : "Arrived First -> Rotate1 (first run, no check)");
                }
                break;

            case State.CheckAtFirst:
                SetIsMoving(false);

                if (characterA != null && characterA.IsWorking())
                {
                    state = State.Rotate1; // keep same routine
                    Log("A IsWorking TRUE -> Rotate1");
                }
                else
                {
                    TriggerSpottedAndLose();
                }
                break;

            case State.Rotate1:
                SetIsMoving(false);
                RotateTowards(rot1);

                if (RotationFinished(rot1))
                {
                    timer = 0f;
                    state = State.WaitBetween;
                    Log("Rotate1 done -> WaitBetween");
                }
                break;

            case State.WaitBetween:
                SetIsMoving(false);
                timer += Time.deltaTime;

                if (timer >= waitBetweenRotations)
                {
                    state = State.Rotate2;
                    Log("WaitBetween done -> Rotate2");
                }
                break;

            case State.Rotate2:
                SetIsMoving(false);
                RotateTowards(rot2);

                if (RotationFinished(rot2))
                {
                    state = State.MoveToLast;
                    Log("Rotate2 done -> MoveToLast");
                }
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

                    Log($"Arrived Last -> WaitAfterLast ({currentWait:F1}s)");
                }
                break;

            case State.WaitAfterLast:
                SetIsMoving(false);
                timer += Time.deltaTime;

                if (timer >= currentWait)
                {
                    state = State.OnLoopEnd;
                    Log("WaitAfterLast done -> OnLoopEnd");
                }
                break;

            case State.OnLoopEnd:
                SetIsMoving(false);

                if (!firstRunCompleted)
                {
                    // ✅ First full route just finished
                    firstRunCompleted = true;

                    // ✅ Start spawner + hold timer + countdown DURING the 5s delay
                    if (!systemsStarted)
                    {
                        systemsStarted = true;
                        gameManager?.StartGameplaySystems();
                        Log("Systems started (during afterFirstRunDelay)");
                    }

                    timer = 0f;
                    state = State.WaitAfterFirstRun;
                    Log("First run complete -> WaitAfterFirstRun");
                }
                else
                {
                    // ✅ After-first-run loops
                    i--;

                    Log($"Loop ended. i now={i}");

                    if (i > 0)
                    {
                        state = State.MoveToFirst;
                        Log("Repeat -> MoveToFirst");
                    }
                    else
                    {
                        Log("No more loops. Character B stops.");
                        enabled = false;
                    }
                }
                break;

            case State.WaitAfterFirstRun:
                SetIsMoving(false);
                timer += Time.deltaTime;

                if (timer >= afterFirstRunDelay)
                {
                    // ✅ Now activate the after-first-run looping count
                    i = Random.Range(minExtraLoops, maxExtraLoops + 1);

                    Log($"afterFirstRunDelay done -> i randomized = {i} -> MoveToFirst");
                    state = State.MoveToFirst;
                }
                break;

            case State.Lose:
                // locked
                break;
        }
    }

    // =====================
    // Lose
    // =====================

    void TriggerSpottedAndLose()
    {
        SetIsMoving(false);

        if (animator != null && !string.IsNullOrEmpty(spottedTrigger))
            animator.SetTrigger(spottedTrigger);

        gameManager?.TriggerLose();

        state = State.Lose;
        Log("Spotted -> Lose");
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
        if (moving == lastIsMoving) return;

        lastIsMoving = moving;
        animator.SetBool(isMovingBool, moving);
    }

    void Log(string msg)
    {
        if (debugLogs)
            Debug.Log("[CharacterMoveB] " + msg);
    }
}
