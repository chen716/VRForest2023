using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEditor.PackageManager;
using UnityEngine;

public class sendSound : MonoBehaviour
{
    IPEndPoint remoteEndPoint;
    UdpClient client;
    private string IP = "10.0.0.20";  // define in init
    public int port = 5000;
    // Start is called before the first frame update
    void Start()
    {
        initUDP();
    }

    // Update is called once per frame
   /* void OnAudioFilterRead(float[] data, int channels)
    {
        print("data len: " + data.Length);
        string toP = "";
        for(int i = 0; i < 30; i++) { 
            toP += data[i];
        }
        print(toP);
        //print(data);
       // sendString(data);
    }*/
    public void initUDP()
    {
        remoteEndPoint = new IPEndPoint(IPAddress.Parse("192.168.86.239"), 8003);
        client = new UdpClient();
    
    }
    private void sendString(float[] message)
    {
        try
        {
            byte[] data = new byte[sizeof(float) * message.Length];
            Buffer.BlockCopy(message, 0, data, 0, data.Length);
            client.Send(data, data.Length, remoteEndPoint);

        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }
    void Update()
    {
        
    }
}
