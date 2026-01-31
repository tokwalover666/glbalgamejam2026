using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    [Header("Spawn Setup")]
    public GameObject boxPrefab;
    public Transform spawnPoint;

    [Header("Waypoints to assign")]
    public Transform firstWaypoint;
    public Transform lastWaypoint;

    [Header("Spawning")]
    public int totalToSpawn = 10;
    public float spawnDelay = 0.5f;

    private int spawnedCount = 0;
    private bool spawning = false;

    void Start()
    {
        SpawnNext();
    }

    void SpawnNext()
    {
        if (spawning) return;
        if (spawnedCount >= totalToSpawn) return;

        spawning = true;
        Invoke(nameof(DoSpawn), spawnDelay);
    }

    void DoSpawn()
    {
        spawning = false;

        GameObject obj = Instantiate(boxPrefab, spawnPoint.position, spawnPoint.rotation);

        BoxMover mover = obj.GetComponent<BoxMover>();
        if (mover != null)
        {
            // Force set the waypoints at spawn time
            mover.Init(firstWaypoint, lastWaypoint, OnActiveBoxFinished);
        }

        spawnedCount++;
    }

    void OnActiveBoxFinished()
    {
        // Active box is gone, spawn the next one
        SpawnNext();
    }
}
