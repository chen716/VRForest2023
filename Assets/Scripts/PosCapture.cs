using System;
using System.IO;
using UnityEngine;

public class PosCapture : MonoBehaviour
{
    public string fileName = "pos.csv";

    private void Start()
    {
        // Start saving position immediately and repeat every 0.2 seconds
        InvokeRepeating(nameof(LogPosition), 0f, 0.2f);
    }

    void LogPosition()
    {
        Vector3 position = transform.position;
        DateTime currentTime = DateTime.Now;
        SavePositionToCSV(currentTime, position);
    }

    void SavePositionToCSV(DateTime time, Vector3 position)
    {
        string filePath = Path.Combine(Application.dataPath, fileName);

        // Check if file exists; if not, write headers
        if (!File.Exists(filePath))
        {
            string[] header = { "Time", "X", "Y", "Z" };
            File.WriteAllText(filePath, string.Join(",", header) + "\n");
        }

        string[] content = { time.ToString("yyyy-MM-dd HH:mm:ss.fff"), position.x.ToString(), position.y.ToString(), position.z.ToString() };
        File.AppendAllText(filePath, string.Join(",", content) + "\n");
    }
}
