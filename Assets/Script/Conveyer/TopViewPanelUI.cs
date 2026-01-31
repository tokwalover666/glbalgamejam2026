using UnityEngine;

public class TopViewPanelUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject panelA; // ex: Main panel
    [SerializeField] private GameObject panelB; // ex: Top view panel

    void Awake()
    {
        HideAll();
    }

    public void ShowBoth()
    {
        if (panelA != null) panelA.SetActive(true);
        if (panelB != null) panelB.SetActive(true);
    }

    public void HideBoth()
    {
        if (panelA != null) panelA.SetActive(false);
        if (panelB != null) panelB.SetActive(false);
    }

    public void ShowAOnly()
    {
        if (panelA != null) panelA.SetActive(true);
        if (panelB != null) panelB.SetActive(false);
    }

    public void ShowBOnly()
    {
        if (panelA != null) panelA.SetActive(false);
        if (panelB != null) panelB.SetActive(true);
    }

    public void HideAll()
    {
        if (panelA != null) panelA.SetActive(false);
        if (panelB != null) panelB.SetActive(false);
    }

}
