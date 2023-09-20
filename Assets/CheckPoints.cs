using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        print("Collision");
        print(other.tag);
        if (other.CompareTag("Player"))
        {
            // Assuming you have a script or a method to update the player's score.
            // For demonstration purposes, we'll print to the console.
            Debug.Log("Player gained 1 point!");
            Destroy(gameObject); // Destroy this checkpoint
        }
    }
}
