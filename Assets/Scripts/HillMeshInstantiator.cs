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

            int index1 = j == (script.meshDataPoints.fencePointsLeft.Count - 1) ? (j - 1) : j;
            int index2 = j == (script.meshDataPoints.fencePointsLeft.Count - 1) ? j : (j + 1);
            Vector3 rPos = (script.meshDataPoints.fencePointsRight[index1] + script.meshDataPoints.fencePointsRight[index2]) / 2f;
            Vector3 lPos = (script.meshDataPoints.fencePointsLeft[index1] + script.meshDataPoints.fencePointsLeft[index2]) / 2f;

            float rAngle = Mathf.Atan2(script.meshDataPoints.fencePointsRight[index2].y - script.meshDataPoints.fencePointsRight[index1].y, script.meshDataPoints.fencePointsRight[index2].x - script.meshDataPoints.fencePointsRight[index1].x) * Mathf.Rad2Deg;
            float lAngle = Mathf.Atan2(script.meshDataPoints.fencePointsLeft[index2].y - script.meshDataPoints.fencePointsLeft[index1].y, script.meshDataPoints.fencePointsLeft[index2].x - script.meshDataPoints.fencePointsLeft[index1].x) * Mathf.Rad2Deg;

            Instantiate(fencePrefabs[randRightFence], new Vector3(rPos.x, rPos.y, rPos.z - 0.2f), Quaternion.Euler(0f, 0f, rAngle), script.transform);
            Instantiate(fencePrefabs[randLeftFence], new Vector3(lPos.x, lPos.y, lPos.z + 0.2f), Quaternion.Euler(0f, 0f, lAngle), script.transform);



        }

    }

    public void OnMeshTriggerInstantiate(HillMeshGeneration script)
    {
        Destroy(script.prevMesh.gameObject);
        CreateSlope();
    }

}


