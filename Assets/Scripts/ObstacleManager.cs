using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObstacleManager : MonoBehaviour
{
    public static ObstacleManager Instance;
    public GameObject[] treePrefabs; // Reference to your obstacle prefab

    public float minObstacleDistance = 1.0f; // Minimum distance between obstacles
    public int obstacleDensity = 5;

    private void Awake()
    {
        Instance = this; // Singleton
    }

    public void InstantiateObstacles(Transform parent, Vector3[] spawnPoints)
    {
        Vector3 randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject randomObstacle = treePrefabs[Random.Range(0, treePrefabs.Length)];
        Instantiate(randomObstacle, randomSpawnPoint, Quaternion.identity, parent);

    }
}

