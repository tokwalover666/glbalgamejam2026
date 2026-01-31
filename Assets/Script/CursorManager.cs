using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorHeart;
    [SerializeField] private Texture2D cursorDefault;
    [SerializeField] private Texture2D cursorMaskingTape;

    private Vector2 heartHotspot;
    private Vector2 defaultHotspot;
    private bool isOverZone;

    void Start()
    {
        heartHotspot = new Vector2(cursorHeart.width / 2f, cursorHeart.height / 2f);
        defaultHotspot = new Vector2(cursorDefault.width / 2f, cursorDefault.height / 2f);

        Cursor.SetCursor(cursorDefault, defaultHotspot, CursorMode.Auto);
    }

    void Update()
    {
        bool nowOverZone = KissZone.IsPointerOverZone();

        if (nowOverZone == isOverZone) return;

        isOverZone = nowOverZone;

        if (isOverZone)
        {
            Cursor.SetCursor(cursorHeart, heartHotspot, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(cursorDefault, defaultHotspot, CursorMode.Auto);
        }
    }
}
