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

    [Header("Player Trigger")]
    public CharacterMove player;   // assign the player here

    private BoxMove currentBox;
    private bool hasSpawned = false;

    void Update()
    {
        if (hasSpawned) return;
        if (player == null) return;

        // 🔑 Spawn ONLY when player reaches final waypoint
        if (player.finished)
        {
            Spawn();
            hasSpawned = true;
        }
    }

    public void Spawn()
    {
        if (boxPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("BoxSpawner missing boxPrefab or spawnPoint.");
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
            Debug.LogWarning("Spawned prefab has no BoxMove component.");
            return;
        }

        if (checkpointWaypoint == null || finalWaypoint == null)
        {
            Debug.LogWarning("BoxSpawner missing waypoint references.");
            return;
        }

        currentBox.Init(checkpointWaypoint, finalWaypoint, ui);

        Debug.Log("[SPAWNER] Box spawned after player arrival.");
    }

    public BoxMove GetCurrentBox()
    {
        return currentBox;
    }
}
