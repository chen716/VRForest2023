using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public GameObject checkpointPrefab;
    public int numberOfCheckpoints = 10;
    public Terrain terrain;

    private void Start()
    {
        SpawnCheckpoints();
    }

    private void SpawnCheckpoints()
    {
        for (int i = 0; i < numberOfCheckpoints; i++)
        {
            float randomX = Random.Range(terrain.transform.position.x, terrain.transform.position.x + terrain.terrainData.size.x);
            float randomZ = Random.Range(terrain.transform.position.z, terrain.transform.position.z + terrain.terrainData.size.z);

            Vector3 spawnPosition = new Vector3(randomX, 0, randomZ);
            float terrainHeightAtThisPosition = terrain.SampleHeight(spawnPosition);
            spawnPosition.y = terrainHeightAtThisPosition;

            GameObject checkpoint = Instantiate(checkpointPrefab, spawnPosition, Quaternion.identity);
            checkpoint.AddComponent<Checkpoint>();
        }
    }
}

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats.score += 1;
            Destroy(gameObject);
        }
    }
}

public static class PlayerStats
{
    public static int score = 0;
}
