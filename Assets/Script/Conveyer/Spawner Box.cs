using UnityEngine;

public class SpawnerBox : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefab;
    public Transform spawnPoint;
    public int spawnCount = 1;

    void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        if (prefab == null || spawnPoint == null) return;

        for (int i = 0; i < spawnCount; i++)
        {
            Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
