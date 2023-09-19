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

    public float jumpForce = 5f;
    private bool isJumping = false;

    private Rigidbody rb;

    //UDP Sockets
    IPEndPoint remoteEndPoint;
    UdpClient client;
    private string IP = "10.0.0.20";  // define in init
    public int port = 5000;  // define in init
    string strMessage = "";
    public GameObject Kart;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        initUDP();
    }
    void OnAudioFilterRead(float[] data, int channels)
    {
        sendString(data);
    }
    private void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
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
        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        print("UDPSend.init()");

        // define


        // ----------------------------
        // Senden
        // ----------------------------
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();

        // status
        print("Inited to " + IP + " : " + port);
       

    }
    //implementation on sending data with udp server
    private void sendString(float[] message)
    {
        try
        {
            //if (message != "")
            //{
            // print("sending data: ");

            //print all message content in one line 
            //Console.WriteLine("[{0}]", string.Join(", ", message));

            print(message[0]);
            // Daten mit der UTF8-Kodierung in das Binärformat kodieren.
            byte[] data = new byte[sizeof(float) * message.Length];
            Buffer.BlockCopy(message, 0, data, 0, data.Length);
            // Den message zum Remote-Client senden.
            client.Send(data, data.Length, remoteEndPoint);
            //}
            // print(data.Length);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

}
