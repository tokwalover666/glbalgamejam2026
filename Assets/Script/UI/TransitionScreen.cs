using UnityEngine;
using System.Collections;
using VFX;

public class TransitionScreen : MonoBehaviour
{
    [SerializeField] GameObject startScreen;
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

        if (startScreen.activeSelf)
        {
            animator.SetTrigger("Transition");

            yield return new WaitForSeconds(2.5f);

            startScreen.SetActive(false);
        }

        isTransitioning = false;
    }
}