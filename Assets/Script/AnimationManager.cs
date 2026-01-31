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
        if (kissZone.isPressingZone == true)
        {
            girlAnim.Play("GirlRotate");
        }
        else
        {
            girlAnim.Play("GirlStanding");
        }

    }
}
