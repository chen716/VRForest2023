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

    private Vector3 lastFootstepPosition; // New variable to track last footstep position

    // Footstep related fields
    public AudioSource source;
    public AudioClip[] sandClips;
    public AudioClip[] soilClips;
    public AudioClip[] soilWetClips;
    public AudioClip[] grassClips;
    public AudioClip[] leavesClips;
    public AudioClip[] mossClips;
    public AudioClip[] rootsClips;
    public AudioClip[] stoneClips;
    private AudioClip previousClip;
    //private AudioClip previousClip;

    void Start()
    {
        
        //t = Terrain.activeTerrain;
        textureValues = new float[8];

        playerTransform = gameObject.transform;
        lastFootstepPosition = playerTransform.position; // Initialize with current position

        // Load footstep sounds based on material name
        sandClips = LoadFootstepClips("sand");
        soilClips = LoadFootstepClips("soil");
        soilWetClips = LoadFootstepClips("soilWet");
        grassClips = LoadFootstepClips("grass");
        leavesClips = LoadFootstepClips("leaves");
        mossClips = LoadFootstepClips("moss");
        rootsClips = LoadFootstepClips("roots");
        stoneClips = LoadFootstepClips("stone");
    }
    AudioClip[] LoadFootstepClips(string materialName)
    {
        string directoryPath = "Sounds/" + materialName + "/";
        print("Loading " + directoryPath);
        // Assuming there are up to 10 clips for each foot type (L and R)
        List<AudioClip> clips = new List<AudioClip>();
        for (int i = 1; i <= 10; i++)
        {
            AudioClip clipL = Resources.Load<AudioClip>(directoryPath + materialName + "L" + i);
            AudioClip clipR = Resources.Load<AudioClip>(directoryPath + materialName + "R" + i);
            if (clipL) clips.Add(clipL);
            if (clipR) clips.Add(clipR);
        }
        return clips.ToArray();
    }

    private void Awake()
    {
     
    }

    void Update()
    {
        // Calculate distance from the last footstep
        float distanceTravelled = Vector3.Distance(playerTransform.position, lastFootstepPosition);

        // Check if distance is greater than or equal to 2 units
        if (distanceTravelled >= 2.0f)
        {
            GetTerrainTexture();
            PlayFootstep(); // Play the footstep sound
            lastFootstepPosition = playerTransform.position; // Update the last footstep position
        }
    }

    public void GetTerrainTexture()
    {
        ConvertPosition(playerTransform.position);
        CheckTexture();
    }

    void ConvertPosition(Vector3 playerPosition)
    {
        Vector3 terrainPosition = playerPosition - t.transform.position;
        Vector3 mapPosition = new Vector3(terrainPosition.x / t.terrainData.size.x, 0, terrainPosition.z / t.terrainData.size.z);
        float xCoord = mapPosition.x * t.terrainData.alphamapWidth;
        float zCoord = mapPosition.z * t.terrainData.alphamapHeight;
        posX = (int)xCoord;
        posZ = (int)zCoord;
    }

    void CheckTexture()
    {
        float[,,] aMap = t.terrainData.GetAlphamaps(posX, posZ, 1, 1);
        //print("gah" + textureValues[7]);
        for (int i = 0; i < 8; i++)
        {
            textureValues[i] = aMap[0, 0, i];
        }
    }


    public void PlayFootstep()
    {
        //print("playing footstep");
        if (textureValues[0] > 0)
        {
            source.PlayOneShot(GetClip(sandClips), textureValues[0]);
        }
        if (textureValues[1] > 0)
        {
            source.PlayOneShot(GetClip(soilClips), textureValues[1]);
        }
        if (textureValues[2] > 0)
        {
            source.PlayOneShot(GetClip(soilWetClips), textureValues[2]);
        }
        if (textureValues[3] > 0)
        {
            source.PlayOneShot(GetClip(grassClips), textureValues[3]);
        }
        if (textureValues[4] > 0)
        {
            source.PlayOneShot(GetClip(leavesClips), textureValues[4]);
        }
        if (textureValues[5] > 0)
        {
            source.PlayOneShot(GetClip(mossClips), textureValues[5]);
        }
        /* if (textureValues[6] > 0)
         {
             source.PlayOneShot(GetClip(rootsClips), textureValues[6]);
         }*/
        if (textureValues[7] > 0)
        {
            source.PlayOneShot(GetClip(stoneClips), textureValues[7]);
        }
    }

    AudioClip GetClip(AudioClip[] clipArray)
    {
        int attempts = 3;
        AudioClip selectedClip = clipArray[Random.Range(0, clipArray.Length - 1)];
        while (selectedClip == previousClip && attempts > 0)
        {
            selectedClip = clipArray[Random.Range(0, clipArray.Length - 1)];
            attempts--;
        }
        previousClip = selectedClip;
        return selectedClip;
    }
}
