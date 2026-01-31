using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform firstWaypoint;   // go here first
    public Transform lastWaypoint;    // final destination

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float arriveDistance = 0.1f;

    [Header("Rotation at First Waypoint")]
    public Vector3 rotateToEuler = new Vector3(0, 90, 0); // set any rotation you want
    public float rotationSpeed = 180f;

    [Header("Animation")]
    public Animator animator;

    // Recommended Animator setup:
    // Bool parameter: "IsWalking"
    public string isWalkingBool = "IsWalking";

    public bool finished { get; private set; }

    private enum State
    {
        MoveToFirst,
        RotateAtFirst,
        MoveToLast,
        Done
    }

    private State state = State.MoveToFirst;

    void Start()
    {
        finished = false;
        SetWalking(true); // start in walk
    }

    void Update()
    {
        if (finished) return;
        if (firstWaypoint == null || lastWaypoint == null) return;

        switch (state)
        {
            case State.MoveToFirst:
                SetWalking(true);
                MoveTo(firstWaypoint.position);

                if (Arrived(firstWaypoint.position))
                {
                    state = State.RotateAtFirst;
                }
                break;

            case State.RotateAtFirst:
                // rotating in place -> idle
                SetWalking(false);
                RotateToTarget();

                if (RotationFinished())
                {
                    state = State.MoveToLast;
                }
                break;

            case State.MoveToLast:
                SetWalking(true);
                MoveTo(lastWaypoint.position);

                if (Arrived(lastWaypoint.position))
                {
                    finished = true;
                    state = State.Done;
                    SetWalking(false); // final idle
                }
                break;

            case State.Done:
                SetWalking(false);
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
