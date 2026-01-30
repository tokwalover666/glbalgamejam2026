using UnityEngine;

public class ColliderDisable : MonoBehaviour
{
    public float holdMouseButtime = 5f;

    private float mouseHoldTimer = 0f;
    private bool isMouseHeld = false;

    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            mouseHoldTimer += Time.deltaTime;
            if (mouseHoldTimer >= holdMouseButtime)
            {
                holdMouseButtime += Time.deltaTime;

                if(mouseHoldTimer >= holdMouseButtime)
                {
                    DisableCollider();
                }
            }
        }
        else
        {
            mouseHoldTimer = 0f;
        }
    }

    void DisableCollider()
    {
        if (boxCollider != null && boxCollider.enabled)
        {
            boxCollider.enabled = false;
        }
    }
}
