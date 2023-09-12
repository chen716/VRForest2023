using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CheckTerrainTextures : MonoBehaviour
{
    public Transform playerTransform;
    public Terrain t;
    public int posX;
    public int posZ;
    public float[] textureValues;
    void Start()
    {
       // textureValues = [0, 0, 0, 0, 0, 0];
        t = Terrain.activeTerrain;
        
        playerTransform = gameObject.transform;
    }
    private void Awake()
    {
        GetComponent<TerrainCollider>().enabled = false;
        GetComponent<TerrainCollider>().enabled = true;
    }
    void Update()
    {
        // For better performance, move this out of update 
        // and only call it when you need a footstep.
        GetTerrainTexture();
    }
    public void GetTerrainTexture()
    {
        ConvertPosition(playerTransform.position);
        CheckTexture();
    }
    void ConvertPosition(Vector3 playerPosition)
    {
        Vector3 terrainPosition = playerPosition - t.transform.position;
        Vector3 mapPosition = new Vector3
        (terrainPosition.x / t.terrainData.size.x, 0,
        terrainPosition.z / t.terrainData.size.z);
        float xCoord = mapPosition.x * t.terrainData.alphamapWidth;
        float zCoord = mapPosition.z * t.terrainData.alphamapHeight;
        posX = (int)xCoord;
        posZ = (int)zCoord;
    }
    void CheckTexture()
    {
        float[,,] aMap = t.terrainData.GetAlphamaps(posX, posZ, 1, 1);
        print(aMap[0,0,0]);
        print(aMap[0, 0, 1]);
        print(aMap[0, 0, 2]);
        print(aMap[0, 0, 3]);
        print(aMap[0, 0, 4]);
        print(aMap[0, 0, 5]);
        print(aMap[0, 0, 6]);
        //textureValues[0] = aMap[0, 0, 0];
        //textureValues[1] = aMap[0, 0, 1];
        //textureValues[2] = aMap[0, 0, 2];*/
        // textureValues[3] = aMap[0, 0, 3];
    }
}