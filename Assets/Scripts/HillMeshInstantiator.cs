using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HillMeshInstantiator : MonoBehaviour
{
    public GameObject prefab; // Reference to the prefab to be instantiated
    private HillMeshGeneration prevHMPrefab;
    private HillMeshGeneration nextHMPrefab;
    public GameObject[] treePrefabs;
    public GameObject[] fencePrefabs;
    private Vector3[] startVertices; // Store the start vertices of the current mesh

    private void Awake()
    {
        GameManager.OnGameStateChange += GMStateChange;  // Subscribe to the OnGameStateChange event from the GameManager
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= GMStateChange; // Unsubscribe to the OnGameStateChange when GO is destroyed to prevent memory leaks.
    }

    private void GMStateChange(GameState state)
    {

    }

    private void Start()
    {
        GameObject temp = Instantiate(prefab, transform.position, Quaternion.identity);
        prevHMPrefab = temp.GetComponent<HillMeshGeneration>();
        StartCoroutine(GenerateMesh(prevHMPrefab));

        for (int i = 0; i < 5; i++)
        {
            CreateSlope();
        }
    }

    void CreateSlope()
    {
        GameObject temp = Instantiate(prefab, transform.position, Quaternion.identity);
        nextHMPrefab = temp.GetComponent<HillMeshGeneration>();
        nextHMPrefab.prevMesh = prevHMPrefab;
        prevHMPrefab = nextHMPrefab;
        StartCoroutine(GenerateMesh(prevHMPrefab));
    }

    IEnumerator GenerateMesh(HillMeshGeneration script)
    {
        // Wait for a frame to ensure that the prefab is initialized
        yield return new WaitForEndOfFrame();

        script.CreateShape();
        script.UpdateMesh();

        //startVertices = script.endVertices;
        InstantiateEdgePrefabs(script);
        //ObstacleManager.Instance.GenerateObstacles(script);

    }


    void InstantiateEdgePrefabs(HillMeshGeneration script)
    {
        for (int i = 0; i < script.meshDataPoints.edgePoints.Count; i++)
        {
            int randTree = Random.Range(0, treePrefabs.Length);
            Vector3 position = new Vector3(script.meshDataPoints.edgePoints[i].x, script.meshDataPoints.edgePoints[i].y, script.meshDataPoints.edgePoints[i].z + Random.Range(-0.2f, 0.2f));
            GameObject tree = Instantiate(treePrefabs[randTree], position, Quaternion.identity, script.transform);
        }

        for (int j = 0; j < script.meshDataPoints.fencePointsLeft.Count; j++)
        {
            int randRightFence = Random.Range(0, fencePrefabs.Length);
            int randLeftFence = Random.Range(0, fencePrefabs.Length);

            if (j == script.meshDataPoints.fencePointsLeft.Count - 1)
            {
                Vector3 rPos = new Vector3(script.meshDataPoints.fencePointsRight[j].x + 0.5f, script.meshDataPoints.fencePointsRight[j].y, script.meshDataPoints.fencePointsRight[j].z);
                Vector3 lPos = new Vector3(script.meshDataPoints.fencePointsLeft[j].x + 0.5f, script.meshDataPoints.fencePointsLeft[j].y, script.meshDataPoints.fencePointsLeft[j].z);

                GameObject rFence = Instantiate(fencePrefabs[randRightFence], rPos, Quaternion.identity, script.transform);
                GameObject lFence = Instantiate(fencePrefabs[randLeftFence], lPos, Quaternion.identity, script.transform);

            }
            else
            {
                Vector3 rPos = (script.meshDataPoints.fencePointsRight[j] + script.meshDataPoints.fencePointsRight[j + 1]) / 2f;
                Vector3 lPos = (script.meshDataPoints.fencePointsLeft[j] + script.meshDataPoints.fencePointsLeft[j + 1]) / 2f;

                float rAngle = Mathf.Atan2(script.meshDataPoints.fencePointsRight[j + 1].y - script.meshDataPoints.fencePointsRight[j].y, script.meshDataPoints.fencePointsRight[j + 1].x - script.meshDataPoints.fencePointsRight[j].x) * Mathf.Rad2Deg;
                float lAngle = Mathf.Atan2(script.meshDataPoints.fencePointsLeft[j + 1].y - script.meshDataPoints.fencePointsLeft[j].y, script.meshDataPoints.fencePointsLeft[j + 1].x - script.meshDataPoints.fencePointsLeft[j].x) * Mathf.Rad2Deg;

                GameObject rFence = Instantiate(fencePrefabs[randRightFence], rPos, Quaternion.Euler(0f, 0f, rAngle), script.transform);
                GameObject lFence = Instantiate(fencePrefabs[randLeftFence], lPos, Quaternion.Euler(0f, 0f, lAngle), script.transform);

            }
        }

    }

    public void OnMeshTriggerInstantiate(HillMeshGeneration script)
    {
        Destroy(script.prevMesh.gameObject);
        CreateSlope();
    }

}


