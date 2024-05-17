using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn; // Reference to the prefab you want to spawn
    public float distanceFromPlane = 100f; // Distance to spawn the prefab from the plane

    // Called when another collider enters the trigger zone
    void OnTriggerEnter(Collider other)
    {
        // Check if the collider entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            SpawnPrefab(); // Call the method to spawn the prefab
        }
    }

    // Method to spawn the prefab at the front of the plane
    void SpawnPrefab()
    {
        // Calculate the spawn position at the front of the plane
        Vector3 spawnPosition = transform.position + transform.forward * distanceFromPlane;

        // Instantiate the prefab at the calculated position with default rotation
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
}
