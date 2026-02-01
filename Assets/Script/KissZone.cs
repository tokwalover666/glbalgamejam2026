using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class KissZone : MonoBehaviour
{
    [SerializeField] Slider holdSlider;
    [SerializeField] GameObject heartPrefab;
    [SerializeField] Transform heartSpawnParent;
    [SerializeField] GameObject kissWinScreen;

    private float holdTimer;
    private int score;

    public static bool isPressingZone;

    private const float maxHoldTime = 2f;
    private const int maxScore = 10;

    void Start()
    {
        kissWinScreen.SetActive(false);
        holdSlider.value = 0f;
    }

    void Update()
    {
        if (score >= maxScore) 
        {
            kissWinScreen.SetActive(true);
        }

        if (Input.GetMouseButtonDown(0) && IsPointerOverZone())
        {
            isPressingZone = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isPressingZone = false;
        }

        if (isPressingZone && Input.GetMouseButton(0))
        {
            holdTimer += Time.deltaTime;
            holdSlider.value = holdTimer / maxHoldTime;

            if (holdTimer >= maxHoldTime)
            {
                score++;

                GameObject heart = Instantiate(heartPrefab, heartSpawnParent);
                RectTransform rt = heart.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2((score - 1) * 150f, 0f);

                holdTimer = 0f;
            }
        }
    }

    public static bool IsPointerOverZone()
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
