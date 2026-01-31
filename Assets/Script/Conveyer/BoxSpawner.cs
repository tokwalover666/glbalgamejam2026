using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    [Header("Spawn")]
    public GameObject boxPrefab;
    public Transform spawnPoint;

    [Header("Waypoints")]
    public Transform checkpointWaypoint;
    public Transform finalWaypoint;

    [Header("Spawn Rotation")]
    public Vector3 spawnRotationEuler;

    [Header("UI")]
    public TopViewPanelUI ui;

    [Header("Options")]
    public bool spawnOnStart = true;

    private BoxMove currentBox;

    void Start()
    {
        if (spawnOnStart)
            Spawn();
    }

    public void Spawn()
    {
        if (boxPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("BoxSpawner missing boxPrefab or spawnPoint.");
            return;
        }

        GameObject obj = Instantiate(boxPrefab, spawnPoint.position, Quaternion.Euler(spawnRotationEuler));

        currentBox = obj.GetComponent<BoxMove>();
        if (currentBox == null)
        {
            Debug.LogWarning("Spawned prefab has no BoxMove component.");
            return;
        }

        if (checkpointWaypoint == null || finalWaypoint == null)
        {
            Debug.LogWarning("BoxSpawner missing waypoint references.");
            return;
        }

        currentBox.Init(checkpointWaypoint, finalWaypoint, ui);
    }

    // Optional helper if you need access
    public BoxMove GetCurrentBox()
    {
        return currentBox;
    }
}
