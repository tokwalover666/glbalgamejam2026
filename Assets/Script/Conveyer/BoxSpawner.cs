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

    [Header("Limit")]
    public int maxBoxesToSpawn = 10;  // ✅ change this in inspector

    [Header("Debug")]
    public bool debugLogs = true;

    private BoxMove currentBox;
    private bool canSpawn = false;

    private int spawnedCount = 0;
    private int completedCount = 0;

    // Call this from GameManager when boss finishes
    public void EnableSpawning()
    {
        canSpawn = true;

        // Spawn the first box immediately
        TrySpawnNext();
    }

    void TrySpawnNext()
    {
        if (!canSpawn) return;

        if (spawnedCount >= maxBoxesToSpawn)
        {
            if (debugLogs) Debug.Log("[BoxSpawner] Reached maxBoxesToSpawn. No more spawns.");
            return;
        }

        SpawnInternal();
    }

    void SpawnInternal()
    {
        if (boxPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("[BoxSpawner] Missing boxPrefab or spawnPoint.");
            return;
        }

        if (checkpointWaypoint == null || finalWaypoint == null)
        {
            Debug.LogWarning("[BoxSpawner] Missing waypoint references.");
            return;
        }

        GameObject obj = Instantiate(
            boxPrefab,
            spawnPoint.position,
            Quaternion.Euler(spawnRotationEuler)
        );

        currentBox = obj.GetComponent<BoxMove>();
        if (currentBox == null)
        {
            Debug.LogWarning("[BoxSpawner] Spawned prefab has no BoxMove component.");
            Destroy(obj);
            return;
        }

        currentBox.Init(checkpointWaypoint, finalWaypoint, ui);

        // ✅ listen for completion
        currentBox.OnReachedFinal += HandleBoxReachedFinal;

        spawnedCount++;

        if (debugLogs)
            Debug.Log($"[BoxSpawner] Spawned box {spawnedCount}/{maxBoxesToSpawn}: {currentBox.name}");
    }

    void HandleBoxReachedFinal(BoxMove box)
    {
        // Unsubscribe (safe cleanup)
        if (box != null)
            box.OnReachedFinal -= HandleBoxReachedFinal;

        completedCount++;

        if (debugLogs)
            Debug.Log($"[BoxSpawner] Box completed! completed={completedCount}, spawned={spawnedCount}");

        // Clear current ref if it’s the same box
        if (currentBox == box)
            currentBox = null;

        // Spawn next one
        TrySpawnNext();
    }

    // Optional getters if you want UI display
    public int GetSpawnedCount() => spawnedCount;
    public int GetCompletedCount() => completedCount;

    public void StopSpawning()
    {
        canSpawn = false;
    }
}
