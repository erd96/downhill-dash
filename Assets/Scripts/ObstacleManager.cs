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

    public void GenerateObstacles(HillMeshGeneration script)
    {
        List<Vector3> vertices = script.meshDataPoints.terrainPoints;
        Vector3 maxVertex = vertices.OrderByDescending(vertex => vertex.x).ElementAt(4);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerPosition = player.transform.position;



        print("Len terrain pts: " + vertices.Count);
        ; print(obstacleDensity);
        for (int i = 0; i < vertices.Count; i++)
        {
            float randomValue = Random.Range(0, 101);

            if (randomValue < obstacleDensity)
            {
                // Check if placing an obstacle at this position maintains a path
                if (PathIsClear(vertices[i]))
                {
                    GameObject obstacle = InstantiateObstacle(vertices[i]);
                    obstacle.transform.parent = script.transform;
                    script.prefabs.Add(obstacle);
                }
            }
        }
    }

    GameObject InstantiateObstacle(Vector3 position)
    {
        Vector3 p = new Vector3(position.x, position.y + 0.33f, position.z);
        return Instantiate(treePrefabs[Random.Range(0, treePrefabs.Length)], p, Quaternion.identity);
    }

    // A-start algorithm
    bool PathIsClear(Vector3 obstaclePosition)
    {
        // Create a temporary object representing the obstacle
        GameObject tempObstacle = Instantiate(treePrefabs[0], obstaclePosition, Quaternion.identity);

        // Check if there is a clear path through the obstacles
        bool isClear = !Physics.Raycast(tempObstacle.transform.position, Vector3.forward, minObstacleDistance);

        // Destroy the temporary obstacle
        Destroy(tempObstacle);

        return isClear;
    }
}

