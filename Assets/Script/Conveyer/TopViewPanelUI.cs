using UnityEngine;

public class TopViewPanelUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject topDownPanel;
    public GameObject otherPanel; // optional second panel

    public void ShowBoth()
    {
        if (topDownPanel != null) topDownPanel.SetActive(true);
        if (otherPanel != null) otherPanel.SetActive(true);
    }

    public void HideBoth()
    {
        if (topDownPanel != null) topDownPanel.SetActive(false);
        if (otherPanel != null) otherPanel.SetActive(false);
    }

    public void ShowTopDownOnly()
    {
        if (topDownPanel != null) topDownPanel.SetActive(true);
    }

    public void HideTopDownOnly()
    {
        if (topDownPanel != null) topDownPanel.SetActive(false);
    }

    public bool IsTopDownOpen()
    {
        return topDownPanel != null && topDownPanel.activeSelf;
    }
}
