using UnityEngine;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Wiring")]
    public HoldToProgressTimer holdTimer;

    [Header("Behavior")]
    public bool stopOnPointerExit = true;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (holdTimer != null)
            holdTimer.OnHoldStart();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (holdTimer != null)
            holdTimer.OnHoldEnd();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!stopOnPointerExit) return;

        if (holdTimer != null)
            holdTimer.OnHoldEnd();
    }
}
