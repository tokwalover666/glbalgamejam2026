using UnityEngine;

public class TopViewPanelUI : MonoBehaviour
{
    public GameObject panel;
    public HoldToProgressTimer holdTimer;

    public void ShowBoth(BoxMove box)
    {
        if (panel != null) panel.SetActive(true);

        if (holdTimer != null)
        {
            holdTimer.boxMove = box;
            holdTimer.ui = this;     // so it can hide the panel on finish
            holdTimer.ResetHold();
        }
    }

    public void HideBoth()
    {
        if (panel != null) panel.SetActive(false);
    }
}
