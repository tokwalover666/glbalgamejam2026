using UnityEngine;
using System.Collections;
using VFX;

public class TransitionScreen : MonoBehaviour
{
    [SerializeField] GameObject mainScreen;
    [SerializeField] Animator animator;

    private bool isTransitioning = false;

    private void Update()
    {
        

        if (Input.GetMouseButtonDown(0)  && !isTransitioning)
        {
            //CameraShake.Shake(0.5f, 1f);
            Debug.Log("Pressed left click.");
            StartCoroutine(ClickStart());
        }
    }

    public IEnumerator ClickStart()
    {
        isTransitioning = true;

        if (mainScreen.activeSelf)
        {
            animator.SetTrigger("Transition");

            yield return new WaitForSeconds(3.5f);

            mainScreen.SetActive(false);
        }

        isTransitioning = false;
    }
}