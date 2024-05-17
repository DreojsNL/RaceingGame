using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpread : MonoBehaviour
{
    public GameObject[] treePrefabs; // Array of tree prefabs to be instantiated
    public BoxCollider spawnArea; // Reference to the box collider representing the area where trees will be spawned
    public int numberOfTrees = 10; // Number of trees to be spawned

    // Start is called before the first frame update
    void Start()
    {
        SpreadTrees();
    }

    // Function to spread trees within the box collider at the bottom
    void SpreadTrees()
    {
        Vector3 bottomCenter = spawnArea.bounds.center - new Vector3(0, spawnArea.bounds.extents.y, 0);
        Vector3 bottomExtents = new Vector3(spawnArea.bounds.extents.x, 0, spawnArea.bounds.extents.z);

        for (int i = 0; i < numberOfTrees; i++)
        {
            Vector3 randomPosition = bottomCenter + new Vector3(Random.Range(-bottomExtents.x, bottomExtents.x), 0, Random.Range(-bottomExtents.z, bottomExtents.z));
            GameObject randomTreePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)]; // Randomly select a tree prefab from the array
            Instantiate(randomTreePrefab, randomPosition, Quaternion.identity);
        }
    }
}