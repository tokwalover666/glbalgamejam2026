using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class KissZone : MonoBehaviour
{
    public float holdTimeThreshold = 0.5f;

    private bool isPressing;
    private bool hasTriggered;
    private float totalDownTime;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverZone()) return;

            totalDownTime = 0f;
            isPressing = true;
            hasTriggered = false;
        }

        if (isPressing && Input.GetMouseButton(0))
        {
            totalDownTime += Time.deltaTime;

            if (!hasTriggered && totalDownTime >= holdTimeThreshold)
            {
                hasTriggered = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!isPressing) return;

            Debug.Log(totalDownTime);
            isPressing = false;
        }
    }

    bool IsPointerOverZone()
    {
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        foreach (var r in results)
        {
            if (r.gameObject.CompareTag("Zone"))
                return true;
        }

        return false;
    }
}
