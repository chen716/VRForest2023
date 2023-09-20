using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public int numberOfCheckpoints = 10; // Number of checkpoints to spawn
    public GameObject checkpointPrefab; // Prefab of the checkpoint
    public Terrain terrain; // Reference to the terrain

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
            spawnPosition.y = terrainHeightAtThisPosition + 0.5f    ;

            GameObject checkpoint = Instantiate(checkpointPrefab, spawnPosition, Quaternion.identity);
            checkpoint.AddComponent<Checkpoint>();
        }
    }

    private Vector3 GetRandomPositionOnTerrain()
    {
        // Assuming the terrain starts at (0,0) and has a width and length
        float x = Random.Range(0, terrain.terrainData.size.x);
        float z = Random.Range(0, terrain.terrainData.size.z);
        float y = terrain.SampleHeight(new Vector3(x, 0, z));

        return new Vector3(x, y, z);
    }
}

