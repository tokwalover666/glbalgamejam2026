using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterMove : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform firstWaypoint;   // go here first
    public Transform lastWaypoint;    // final destination

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float arriveDistance = 0.1f;

    [Header("Rotation at First Waypoint")]
    public Vector3 rotateToEuler = new Vector3(0, 90, 0);
    public float rotationSpeed = 180f;

    [Header("Animation")]
    public Animator animator;

    // Animator parameters
    public string isWalkingBool = "IsWalking";
    public string isWorkingBool = "IsWorking";

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

        // Prevent animation from overriding transform motion
        animator.applyRootMotion = false;
    }

    void Start()
    {
        finished = false;
        SetWalking(true);
        SetWorking(false);
    }

    void Update()
    {
        if (finished) return;
        if (firstWaypoint == null || lastWaypoint == null) return;

        switch (state)
        {
            case State.MoveToFirst:
                if (!isWorking)
                {
                    SetWalking(true);
                    MoveTo(firstWaypoint.position);

                    if (Arrived(firstWaypoint.position))
                        state = State.RotateAtFirst;
                }
                break;

            case State.RotateAtFirst:
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
    // Public API (IMPORTANT)
    // =====================

    /// <summary>
    /// Called by HoldToProgressTimer
    /// </summary>
    public void SetWorking(bool working)
    {
        if (isWorking == working) return;

        isWorking = working;

        if (animator != null)
            animator.SetBool(isWorkingBool, working);

        // Stop walking animation while working
        if (working)
            SetWalking(false);
    }

    /// <summary>
    /// Used by CharacterMoveB to check if player is safe
    /// </summary>
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
        if (animator == null) return;
        animator.SetBool(isWalkingBool, walking);
    }
}
