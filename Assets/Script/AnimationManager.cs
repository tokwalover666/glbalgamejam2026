using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public KissZone kissZone;
    [SerializeField] Animator girlAnim;
    [SerializeField] Animator boyAnim;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (KissZone.isPressingZone == true)
        {
            girlAnim.Play("Kiss_Girl");
            boyAnim.Play("Kiss Boy");
        }
        else
        {
            girlAnim.Play("Idle_Girl");
            boyAnim.Play("Idle_001");
        }

    }
}
