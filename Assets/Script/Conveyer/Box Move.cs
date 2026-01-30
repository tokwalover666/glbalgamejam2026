using UnityEngine;
using UnityEngine.UIElements;

public class BoxMove : MonoBehaviour
{ 
    
    public float speed = 2f;
    public Vector3 direction = Vector3.right;
    private bool canMove = true;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Checkpoint"))
        {
            canMove = false;
        }
    }

    public void Update()
    {
        if (!canMove) return;
        transform.position += direction.normalized * speed * Time.deltaTime;
    }
}
