using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class InputRecorder : MonoBehaviour
{
    [System.Serializable]
    public class FrameInput
    {
        public float xf;
        public float yf;
    }

    private List<FrameInput> recordedInputs = new List<FrameInput>();
    public bool isReplaying = false;
    private int replayIndex = 0;

    private string savePath;
    public string inputFile = "test.dat";

    private movement movementScript;

    private void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, inputFile);

        movementScript = GetComponent<movement>();

        if (isReplaying)
        {
            LoadFromFile(savePath);
        }
    }

    void Update()
    {
        if (isReplaying)
        {
            Replay();
        }
        else
        {
            Record();
        }
    }

    void Record()
    {
        if (movementScript)
        {
            FrameInput currentInput = new FrameInput
            {
                xf = movementScript.xf,
                yf = movementScript.yf
            };

            recordedInputs.Add(currentInput);
        }
    }

    void Replay()
    {
        if (replayIndex >= recordedInputs.Count)
        {
            isReplaying = false;
            return;
        }

        FrameInput currentInput = recordedInputs[replayIndex];

        // Here, I'm assuming you want to set the xf and yf values of the movement script
        // to the recorded values and then call the Move() method to apply the movement.
        if (movementScript)
        {
            movementScript.xf = currentInput.xf;
            movementScript.yf = currentInput.yf;
            movementScript.Move();
        }

        replayIndex++;
    }

    public void SaveToFile(string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            formatter.Serialize(stream, recordedInputs);
        }
    }

    public void LoadFromFile(string path)
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                recordedInputs = formatter.Deserialize(stream) as List<FrameInput>;
            }
        }
    }

    void OnApplicationQuit()
    {
        if (!isReplaying)
        {
            SaveToFile(savePath);
        }
    }
}
