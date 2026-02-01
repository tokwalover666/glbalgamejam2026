using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterMove : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform firstWaypoint;
    public Transform lastWaypoint;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float arriveDistance = 0.1f;

    [Header("Rotation at First Waypoint")]
    public Vector3 rotateToEuler = new Vector3(0, 90, 0);
    public float rotationSpeed = 180f;

    [Header("Animation")]
    public Animator animator;

    // IMPORTANT: These must match Animator parameters (case-sensitive)
    public string isWalkingBool = "isWalking";
    public string isWorkingBool = "isWorking";

    [Header("Debug")]
    public bool debugLogs = false;

    public bool finished { get; private set; }

    private bool isWorking = false;

    private enum State
    {
        MoveToFirst,
        RotateAtFirst,
        MoveToLast,
        Done
    }

    private State state = State.MoveToFirst;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator != null)
            animator.applyRootMotion = false;

        // ✅ Auto-fix parameter name if your Animator uses different casing (ex: IsWorking vs isWorking)
        isWalkingBool = ResolveAnimatorBoolName(isWalkingBool);
        isWorkingBool = ResolveAnimatorBoolName(isWorkingBool);
    }

    void Start()
    {
        if (TransitionScreen.startGameplay == true)
        {
            finished = false;
        SetWalking(true);
        SetWorking(false);
        }

        
    }

    void Update()
    {

        if (finished) return;
        if (firstWaypoint == null || lastWaypoint == null) return;

        switch (state)
        {
            case State.MoveToFirst:
                if (!isWorking && TransitionScreen.startGameplay == true)
                {
                    SetWalking(true);
                    MoveTo(firstWaypoint.position);

                    if (Arrived(firstWaypoint.position))
                        state = State.RotateAtFirst;
                }
                break;

            case State.RotateAtFirst:
                // Rotation phase: stop walking (still not "working")
                SetWalking(false);
                RotateToTarget();

                if (RotationFinished())
                    state = State.MoveToLast;
                break;

            case State.MoveToLast:
                if (!isWorking)
                {
                    SetWalking(true);
                    MoveTo(lastWaypoint.position);

                    if (Arrived(lastWaypoint.position))
                    {
                        finished = true;
                        state = State.Done;
                        SetWalking(false);
                        SetWorking(false);
                    }
                }
                break;

            case State.Done:
                SetWalking(false);
                SetWorking(false);
                break;
        }
    }

    // =====================
    // Public API
    // =====================

    public void SetWorking(bool working)
    {
        if (isWorking == working) return;
        isWorking = working;

        if (animator != null && !string.IsNullOrEmpty(isWorkingBool))
            animator.SetBool(isWorkingBool, working);

        // Stop walking animation while working
        if (working)
            SetWalking(false);

        if (debugLogs)
            Debug.Log($"[CharacterMove] SetWorking({working}) | workingParam='{isWorkingBool}'");
    }

    public bool IsWorking()
    {
        return isWorking;
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

    void RotateToTarget()
    {
        Quaternion targetRot = Quaternion.Euler(rotateToEuler);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }

    bool Arrived(Vector3 target)
    {
        return Vector3.Distance(transform.position, target) <= arriveDistance;
    }

    bool RotationFinished()
    {
        Quaternion targetRot = Quaternion.Euler(rotateToEuler);
        return Quaternion.Angle(transform.rotation, targetRot) < 1f;
    }

    void SetWalking(bool walking)
    {
        if (animator == null || string.IsNullOrEmpty(isWalkingBool)) return;
        animator.SetBool(isWalkingBool, walking);

        if (debugLogs)
            Debug.Log($"[CharacterMove] SetWalking({walking}) | walkingParam='{isWalkingBool}'");
    }

    // ✅ Finds correct bool name even if casing differs
    string ResolveAnimatorBoolName(string desiredName)
    {
        if (animator == null || string.IsNullOrEmpty(desiredName))
            return desiredName;

        // Exact match
        foreach (var p in animator.parameters)
        {
            if (p.type == AnimatorControllerParameterType.Bool && p.name == desiredName)
                return desiredName;
        }

        // Case-insensitive match
        foreach (var p in animator.parameters)
        {
            if (p.type != AnimatorControllerParameterType.Bool) continue;

            if (string.Equals(p.name, desiredName, System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogWarning($"[CharacterMove] Animator bool '{desiredName}' not found. Using '{p.name}' instead.");
                return p.name;
            }
        }

        Debug.LogWarning($"[CharacterMove] Animator bool '{desiredName}' not found at all. Working/Walking may not animate.");
        return desiredName;
    }
}
