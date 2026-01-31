using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Header("Waypoint Index")]
    public int waypointIndex;

    private void OnTriggerEnter(Collider other)
    {
        CharacterMove mover = other.GetComponent<CharacterMove>();
        if (mover == null) return;
    }
}
