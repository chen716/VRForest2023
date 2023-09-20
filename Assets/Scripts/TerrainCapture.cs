using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;

public class TerrainDataLogger : MonoBehaviour
{
    public float logInterval = 0.2f; // Time interval for logging data
    private List<string> loggedData = new List<string>(); // List to store logged data
    string fileName = "TerrainData.csv";

    private CheckTerrainTextures terrainTextureChecker; // Reference to the CheckTerrainTextures script attached to the same GameObject

    private void Start()
    {
        terrainTextureChecker = GetComponent<CheckTerrainTextures>();

        if (terrainTextureChecker == null)
        {
            Debug.LogError("CheckTerrainTextures script not found on this GameObject!");
            return;
        }

        // Start the coroutine to log data every 'logInterval' seconds
        StartCoroutine(LogTerrainData());
    }

    IEnumerator LogTerrainData()
    {
        while (true)
        {
            yield return new WaitForSeconds(logInterval);

            // Get the current terrain data from the CheckTerrainTextures script
            terrainTextureChecker.GetTerrainTexture();

            // Get the current date and time
            DateTime currentTime = DateTime.Now;

            // Create a string representation of the data with the current time
            StringBuilder dataLine = new StringBuilder(currentTime.ToString("yyyy-MM-dd HH:mm:ss.fff")); // Timestamp with date, time, and milliseconds
            foreach (float value in terrainTextureChecker.textureValues)
            {
                dataLine.Append(",");
                dataLine.Append(value.ToString("F2"));
            }

            // Add the data line to the logged data list
            loggedData.Add(dataLine.ToString());
        }
    }

    // Call this function to save the logged data to a CSV file
    public void SaveDataToCSV()
    {
        string filePath = Path.Combine(Application.dataPath, fileName);
        StringBuilder csvContent = new StringBuilder();

        // Add header
        csvContent.AppendLine("Timestamp,Texture1,Texture2,Texture3,Texture4,Texture5,Texture6,Texture7,Texture8");

        // Add logged data
        foreach (string line in loggedData)
        {
            csvContent.AppendLine(line);
        }

        // Write to file
        File.WriteAllText(filePath, csvContent.ToString());
        Debug.Log("Data saved to: " + filePath);
    }

    private void OnApplicationQuit()
    {
        // Save the data to a CSV file when the application quits
        SaveDataToCSV();
    }
}
