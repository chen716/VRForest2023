using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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
}
