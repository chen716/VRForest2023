using System;
using System.IO;
using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class DataCapture : MonoBehaviour
{
    public string posFileName = "pos.csv";
    public string terrainFileName = "terrainData.csv";
    public float logInterval = 0.2f;

    private CheckTerrainTextures terrainTextureChecker;
    private List<string> terrainLoggedData = new List<string>();
    private List<string> positionLoggedData = new List<string>();

    private void Start()
    {
        terrainTextureChecker = GetComponent<CheckTerrainTextures>();

        if (terrainTextureChecker == null)
        {
            Debug.LogError("CheckTerrainTextures script not found on this GameObject!");
            return;
        }

        // Start the coroutine to log data every 'logInterval' seconds
        StartCoroutine(LogData());
    }

    IEnumerator LogData()
    {
        while (true)
        {
            yield return new WaitForSeconds(logInterval);

            DateTime currentTime = DateTime.Now;
            LogPosition(currentTime);
            LogTerrainData(currentTime);
        }
    }

    void LogPosition(DateTime time)
    {
        Vector3 position = transform.position;
        StringBuilder dataLine = new StringBuilder(time.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        dataLine.Append(",");
        dataLine.Append(position.x.ToString());
        dataLine.Append(",");
        dataLine.Append(position.y.ToString());
        dataLine.Append(",");
        dataLine.Append(position.z.ToString());

        positionLoggedData.Add(dataLine.ToString());
    }

    void LogTerrainData(DateTime time)
    {
        terrainTextureChecker.GetTerrainTexture();

        StringBuilder dataLine = new StringBuilder(time.ToString("yyyy-MM-dd HH:mm:ss.fff")); // Timestamp with date and time
        foreach (float value in terrainTextureChecker.textureValues)
        {
            dataLine.Append(",");
            dataLine.Append(value.ToString("F2"));
        }

        terrainLoggedData.Add(dataLine.ToString());
    }

    private void OnApplicationQuit()
    {
        // Save the data to CSV files when the application quits
        SavePositionToCSV();
        SaveTerrainDataToCSV();
    }

    void SavePositionToCSV()
    {
        string filePath = Path.Combine(Application.dataPath, posFileName);

        // If file doesn't exist, create it and add the header
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "Time,X,Y,Z\n");
        }

        using (StreamWriter writer = new StreamWriter(filePath, true)) // 'true' ensures that we append to the file
        {
            // Add all new logged data
            foreach (string line in positionLoggedData)
            {
                writer.WriteLine(line);
            }

            writer.Flush();
        }

        // Clear the logged data list to avoid writing duplicates
        positionLoggedData.Clear();
    }

    void SaveTerrainDataToCSV()
    {
        string filePath = Path.Combine(Application.dataPath, terrainFileName);

        // If file doesn't exist, create it and add the header
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "Timestamp,Texture1,Texture2,Texture3,Texture4,Texture5,Texture6,Texture7,Texture8\n");
        }

        using (StreamWriter writer = new StreamWriter(filePath, true)) // 'true' ensures that we append to the file
        {
            // Add all new logged data
            foreach (string line in terrainLoggedData)
            {
                writer.WriteLine(line);
            }

            writer.Flush();
        }

        // Clear the logged data list to avoid writing duplicates
        terrainLoggedData.Clear();
    }
}
