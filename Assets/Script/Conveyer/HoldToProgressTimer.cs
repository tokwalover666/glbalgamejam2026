using UnityEngine;

public class HoldToProgressTimer : MonoBehaviour
{
    public float holdDuration = 2.5f;

    float t = 0f;
    bool holding = false;
    bool done = false;

    void Update()
    {
        if (done) return;

        if (holding)
        {
            t += Time.deltaTime;

            if (t >= holdDuration)
            {
                done = true;
                holding = false;
                GameManager.Instance?.OnHoldComplete();
            }
        }
    }

    public void OnHoldStart() => holding = true;
    public void OnHoldEnd() => holding = false;

    public void ResetHold()
    {
        t = 0f;
        done = false;
        holding = false;
    }

    public float GetProgress01() => Mathf.Clamp01(t / holdDuration);
}
