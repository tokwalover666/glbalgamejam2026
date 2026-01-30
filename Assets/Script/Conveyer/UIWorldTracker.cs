using UnityEngine;

public class UIWorldTracker : MonoBehaviour
{
    [Header("Target to track (3D box)")]
    public Transform worldTarget;

    [Header("UI container (your bottom panel RectTransform)")]
    public RectTransform containerPanel;

    [Header("Canvas (needed for correct conversion)")]
    public Canvas canvas;

    [Header("Camera used to render the 3D world")]
    public Camera worldCamera;

    [Header("Clamp inside panel")]
    public bool clampInsidePanel = true;

    RectTransform uiIcon;

    void Awake()
    {
        uiIcon = GetComponent<RectTransform>();

        if (worldCamera == null) worldCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (worldTarget == null || containerPanel == null || canvas == null || worldCamera == null)
            return;

        // 1) World -> Screen
        Vector3 screenPos = worldCamera.WorldToScreenPoint(worldTarget.position);

        // If target is behind camera, hide the icon (optional)
        if (screenPos.z < 0f)
        {
            uiIcon.gameObject.SetActive(false);
            return;
        }
        else
        {
            if (!uiIcon.gameObject.activeSelf) uiIcon.gameObject.SetActive(true);
        }

        // 2) Screen -> Local point inside the panel
        Vector2 localPoint;
        Camera uiCam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;

        bool ok = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            containerPanel,
            screenPos,
            uiCam,
            out localPoint
        );

        if (!ok) return;

        // 3) Optional clamp so it stays inside the panel
        if (clampInsidePanel)
        {
            Rect r = containerPanel.rect;
            float halfW = uiIcon.rect.width * 0.5f;
            float halfH = uiIcon.rect.height * 0.5f;

            localPoint.x = Mathf.Clamp(localPoint.x, r.xMin + halfW, r.xMax - halfW);
            localPoint.y = Mathf.Clamp(localPoint.y, r.yMin + halfH, r.yMax - halfH);
        }

        uiIcon.anchoredPosition = localPoint;
    }
}
