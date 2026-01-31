using UnityEngine;

public class BoxMove : MonoBehaviour
{
    public Transform checkpoint;
    public Transform finalPoint;
    public float speed = 3f;
    public float arriveDistance = 0.05f;

    [Header("UI Panels")]
    public TopViewPanelUI ui; // drag the object with TwoPanelUI here

    public enum BoxState
    {
        MoveToCheckpoint,
        Wait,
        MoveToFinal
    }

    public BoxState state = BoxState.MoveToCheckpoint;

    void Update()
    {
        switch (state)
        {
            case BoxState.MoveToCheckpoint:
                Move(checkpoint.position);

                if (Arrived(checkpoint.position))
                {
                    state = BoxState.Wait;
                    ui?.ShowBoth(); // show BOTH panels when waiting
                }
                break;

            case BoxState.Wait:
                // waiting for UI interaction later
                break;

            case BoxState.MoveToFinal:
                Move(finalPoint.position);

                if (Arrived(finalPoint.position))
                {
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

    // Call from UI button
    public void GoToFinal()
    {
        if (state == BoxState.Wait)
        {
            state = BoxState.MoveToFinal;
            ui?.HideBoth(); // hide both once it continues
        }
    }
}
