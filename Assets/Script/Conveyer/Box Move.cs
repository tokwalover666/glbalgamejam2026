using System;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class BoxMove : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform checkpoint;
    public Transform finalPoint;

    [Header("Movement")]
    public float speed = 3f;
    public float arriveDistance = 0.05f;

    [Header("UI Panels")]
    public TopViewPanelUI ui;

    public CharacterMove character;

    public enum BoxState
    {
        MoveToCheckpoint,
        Wait,
        MoveToFinal
    }

    public BoxState state = BoxState.MoveToCheckpoint;

    // ✅ NEW: spawner can subscribe to this
    public event Action<BoxMove> OnReachedFinal;

    public void Init(Transform checkpointTarget, Transform finalTarget, TopViewPanelUI uiRef)
    {
        checkpoint = checkpointTarget;
        finalPoint = finalTarget;
        ui = uiRef;

        state = BoxState.MoveToCheckpoint;
    }

    void Update()
    {
        if (checkpoint == null || finalPoint == null) return;

        switch (state)
        {
            case BoxState.MoveToCheckpoint:
                Move(checkpoint.position);

                if (Arrived(checkpoint.position))
                {
                    state = BoxState.Wait;
                    ui?.ShowBoth(this); // make sure your UI version supports ShowBoth(BoxMove)
                }
                break;

            case BoxState.Wait:
                if (character != null)
                {

                }
                break;


            case BoxState.MoveToFinal:
                Move(finalPoint.position);

                if (Arrived(finalPoint.position))
                {
                    // ✅ Notify spawner BEFORE destroy
                    OnReachedFinal?.Invoke(this);

                    Destroy(gameObject);
                }
                break;
        }
    }

    void Move(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );
    }

    bool Arrived(Vector3 target)
    {
        return Vector3.Distance(transform.position, target) <= arriveDistance;
    }

    public void GoToFinal()
    {
        if (state == BoxState.Wait)
        {
            state = BoxState.MoveToFinal;
            ui?.HideBoth();
        }
    }
}
