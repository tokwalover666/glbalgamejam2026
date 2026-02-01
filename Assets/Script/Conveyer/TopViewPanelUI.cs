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
            // ✅ Tell the hold system which box is currently being worked on
            holdTimer.ui = this; // so it can hide panel on finish
            holdTimer.SetActiveBox(box);
        }
    }

    public void HideBoth()
    {
        if (panel != null) panel.SetActive(false);
    }
}
