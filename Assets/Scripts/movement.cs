using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;

public class movement : MonoBehaviour
{
    public float baseSpeed = 5f;
    public float rotationSpeed = 5f;
    public float sprintMultiplier = 5f;
    public float maxSlopeAngle = 45f;
    public float maxSpeed = 10f; // Define the maximum speed.
    public AudioSource audioSource;
    public float jumpForce = 5f;
    private bool isJumping = false;

    private Rigidbody rb;

    //UDP Sockets sending
    IPEndPoint remoteEndPoint;
    UdpClient client;
    private string IP = "10.0.0.20";  // define in init
    public int port = 5000;  // define in init
    public float horizontalInput ;
    public float verticalInput;
    //UDP socket receiving
    Thread receiveThread;
    Thread receiveThread2;
    IPEndPoint receiveRemoteEndPoint;
    IPEndPoint receiveRemoteEndPoint2;
    UdpClient receiveClient;
    UdpClient receiveClient2;
    private string ReceiveIP;  // define in init
    public int receivePort;
    public int receivePort2 = 8001;  // define in init
    string strMessage = "";
    public string lastReceivedUDPPacket = "";
    public string allReceivedUDPPackets = ""; // clean up this from time to time!
    public string lastReceivedUDPPacket2 = "";
    public string allReceivedUDPPackets2 = ""; // clean up this from time to time!
    public string x1 = "";
    public string y1 = "";
    public float t1 = 0;
    public string x2 = "";
    public string y2 = "";
    public float t2 = 0;
    public float xf = 0;
    public float yf = 0;
    public float xf1 = 0;
    public float yf1 = 0;
    public float xf2 = 0;
    public float yf2 = 0;
    public bool WiiBoard = true;
    public bool singleBoard = true;
    public bool playback = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        initUDP();
    }
    void OnAudioFilterRead(float[] data, int channels)
    {
        print("data len: " + data.Length);
        string toP = "";
        for (int i = 200; i < data.Length; i++)
        {
            toP += data[i];
        }
        print(toP);
        //print(data);
        sendString(data);
    }
    private void Update()
    {
        Move();
        Jump();
    }

    public void Move()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (WiiBoard)
        {
            yf = 0;
            if (singleBoard)
            {
                string temp = getLatestUDPPacket();
                if (temp.Length > 0)
                {
                    string[] words = temp.Split(',');
                    x1 = words[0];
                    y1 = words[1];
                    t1 = float.Parse(words[2]);
                    xf = -(float.Parse(x1) - 127) / 127;
                    yf = (float.Parse(y1) - 127) / 127 ;
                    //print("x1: " + xf + " y1:" + yf);

                    verticalInput = yf;
                    horizontalInput = xf;

                }
            }
            else
            {
                string tempL = getLatestUDPPacket();
                string tempR = getLatestUDPPacket2();
                if (tempL.Length > 0)
                {
                    string[] words = tempL.Split(',');
                    x1 = words[0];
                    y1 = words[1];
                    t1 = float.Parse(words[2]);
                    xf1 = -(float.Parse(x1) - 127) / 127;
                    yf1 = (float.Parse(y1) - 127) / 127;
                    //print("x1: " + xf + " y1:" + yf);

                }
                if (tempR.Length > 0)
                {
                    string[] words = tempR.Split(',');
                    x2 = words[0];
                    y2 = words[1];
                    t2 = float.Parse(words[2]);
                    xf2 = -(float.Parse(x2) - 127) / 127;
                    yf2 = (float.Parse(y2) - 127) / 127;
                    //print("x1: " + xf + " y1:" + yf);
                }
                if(t1 +t2 > 0) {
                    verticalInput = - (xf1 + xf2) / 2.0f; // -(xf1 * t1 + xf2 * t2) / (t1+t2) * 2.0f / 1.8f ;
                    horizontalInput =-( (t1) / (t1+t2)*2 -1);
                    //print("x1: "+ xf1 + ", xf2:" + xf2 +", t1: "+t1+", t2:"+t2+" v:"+verticalInput +", h:"+ horizontalInput);
                }

            }
        }
        if (playback)
        {
            // time parser 
            // desc: track how much time it has been since start using Unity time

            //set vertical input to a value
            //set horizontal input to a value 

        }
        Vector3 moveDirection = transform.forward * verticalInput;

        // Rotate based on horizontal input.
        transform.Rotate(0f, horizontalInput * rotationSpeed * Time.deltaTime, 0f);

        // Check for sprinting
        float currentSpeed = baseSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed *= sprintMultiplier;
        }

        // Calculate the slope factor based on the terrain normal.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            float slopeFactor = Mathf.Clamp01(slopeAngle / maxSlopeAngle);
            currentSpeed *= (1 - slopeFactor);
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

            Vector3 move = moveDirection * currentSpeed * Time.deltaTime;
            transform.Translate(move, Space.World);
        }
    }


    void Jump()
    {
        bool isGrounded = Physics.Raycast(transform.position, -Vector3.up, 1.5f);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
        }
        else if (isGrounded)
        {
            isJumping = false;
        }
    }
    public void initUDP()
    {

        remoteEndPoint = new IPEndPoint(IPAddress.Parse("192.168.86.239"), 8003);
        client = new UdpClient();
        ReceiveIP = "127.0.0.1";
        receivePort = 5000;
        UDPReceiveInit();




    }
    public void UDPReceiveInit()
    {
        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        print("UDPReceive.init()");

        // define
        ReceiveIP = "192.168.86.25";
        receivePort = 8000;
        print("listening from " + ReceiveIP + ":" + receivePort);
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
       // receiveClient2 = new UdpClient(receivePort2);
        //receiveRemoteEndPoint2 = new IPEndPoint(IPAddress.Any, receivePort2);
        receiveThread2 = new Thread(new ThreadStart(ReceiveData2));
        receiveThread2.IsBackground = true;
        receiveThread2.Start();


    }
    private void ReceiveData()
    {

        receiveClient = new UdpClient(receivePort);
        while (true)
        {

            try
            {
                // Bytes empfangen.，
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 8000);
                byte[] data = receiveClient.Receive(ref anyIP);
                // Bytes mit der UTF8-Kodierung in das Textformat kodieren.
                string text = Encoding.UTF8.GetString(data);
                //print("rec1: " + text);
                lastReceivedUDPPacket = text;
                allReceivedUDPPackets = allReceivedUDPPackets + text;

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }

    }
    void ReceiveData2()
    {
        print("starting receive 2");
        receiveClient2 = new UdpClient(receivePort2);
        while (true)
        {

            try
            {
                // Bytes empfangen.，
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 8001);
                byte[] data = receiveClient2.Receive(ref anyIP);
                // Bytes mit der UTF8-Kodierung in das Textformat kodieren.
                string text = Encoding.UTF8.GetString(data);
                /*print("rec2: " + text);*/
                lastReceivedUDPPacket2 = text;
                allReceivedUDPPackets2 = allReceivedUDPPackets2 + text;

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }
    public string getLatestUDPPacket()
    {
        allReceivedUDPPackets = "";
        return lastReceivedUDPPacket;
    }
    public string getLatestUDPPacket2()
    {
        allReceivedUDPPackets2 = "";
        return lastReceivedUDPPacket2;
    }

    //implementation on sending data with udp server
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

}

