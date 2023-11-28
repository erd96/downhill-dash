using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObstacleManager : MonoBehaviour
{
    public static ObstacleManager Instance;
    public GameObject[] obstaclePrefabs; // Reference to your obstacle prefab

    public float minObstacleDistance = 1.0f; // Minimum distance between obstacles
    public int obstacleDensity = 5;

    private void Awake()
    {

        Instance = this; // Singleton
    }

    public void InstantiateObstacles(Transform parent, Vector3[] spawnPoints)
    {
        Vector3 spawnPoint;
        GameObject randomObstacle = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

        if (randomObstacle.CompareTag("Obstacle1Z")) spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        else spawnPoint = spawnPoints[1];
        Instantiate(randomObstacle, spawnPoint, Quaternion.identity, parent);
    }
}

