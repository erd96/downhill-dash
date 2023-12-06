using UnityEngine;


public class ObstacleManager : MonoBehaviour
{
    public static ObstacleManager Instance;
    [SerializeField] GameObject[] obstaclePrefabs; // Reference to your obstacle prefab
    [SerializeField] GameObject pillarPrefab;
    private GameObject prevPrefab = null;

    private void Awake()
    {

        Instance = this; // Singleton
    }

    public void InstantiateObstacles(Transform parent, Vector3[] spawnPoints)
    {
        Vector3 spawnPoint;
        GameObject randomObstacle = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

        if (randomObstacle.CompareTag("Obstacle1Z") || randomObstacle.CompareTag("Rail")) 
        {
            if (GameManager.Instance.terrainCount % 2 == 0) spawnPoints = new Vector3[] { spawnPoints[0], spawnPoints[2] };
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        } 
        else spawnPoint = spawnPoints[1];  // Obstacle3Z

        prevPrefab =  Instantiate(randomObstacle, spawnPoint, randomObstacle.transform.rotation, parent);
    }


    public void InstantiatePillar(Transform parent, Vector3 position)
    {
        Instantiate(pillarPrefab, position, pillarPrefab.transform.rotation, parent);
    }
}

