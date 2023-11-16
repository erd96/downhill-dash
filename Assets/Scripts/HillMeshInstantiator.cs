using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HillMeshInstantiator : MonoBehaviour
{
    public GameObject prefab; // Reference to the prefab to be instantiated
    private GameObject prevHMPrefab;
    private GameObject nextHMPrefab;
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
        if (state == GameState.StartGame)
        {
                Vector3 position = transform.position;
                prevHMPrefab = Instantiate(prefab, position, Quaternion.identity);
                prevHMPrefab.name = "prevHMPrefab";
                HillMeshGeneration HillMeshGenerationScript = prevHMPrefab.GetComponent<HillMeshGeneration>();
                StartCoroutine(GenerateMesh(HillMeshGenerationScript));
        }
    }


    IEnumerator GenerateMesh(HillMeshGeneration script)
    {
        // Wait for a frame to ensure that the prefab is initialized
        yield return new WaitForEndOfFrame();

        // Check if startVertices is null, if so, call the regular CreateShape function
        if (startVertices == null)
        {

            script.CreateShape();
        }
        else
        {
            // Call the CreateShapeAtLocation function and pass the startVertices
            script.CreateShapeAtLocation(startVertices);
        }

        // Update the mesh
        script.UpdateMesh();

        startVertices = script.endVertices;

        // Set the last instantiated prefab
        InstantiateEdgePrefabs(script);

    }


    void InstantiateEdgePrefabs(HillMeshGeneration script)
    {
        for (int i = 0; i < script.meshDataPoints.edgePoints.Count; i++)
        {
            int randTree = Random.Range(0, treePrefabs.Length);
            Vector3 position = new Vector3(script.meshDataPoints.edgePoints[i].x, script.meshDataPoints.edgePoints[i].y, script.meshDataPoints.edgePoints[i].z + Random.Range(-0.2f, 0.2f));
            GameObject tree = Instantiate(treePrefabs[randTree], position, Quaternion.identity);
            tree.transform.parent = script.transform;
            script.prefabs.Add(tree);
        }

        for (int j = 0; j < script.meshDataPoints.fencePointsLeft.Count; j++)
        {
            int randRightFence = Random.Range(0, fencePrefabs.Length);
            int randLeftFence = Random.Range(0, fencePrefabs.Length);

            if (j == script.meshDataPoints.fencePointsLeft.Count-1)
            {
                Vector3 rPos = new Vector3(script.meshDataPoints.fencePointsRight[j].x + 0.5f, script.meshDataPoints.fencePointsRight[j].y, script.meshDataPoints.fencePointsRight[j].z);
                Vector3 lPos = new Vector3(script.meshDataPoints.fencePointsLeft[j].x + 0.5f, script.meshDataPoints.fencePointsLeft[j].y, script.meshDataPoints.fencePointsLeft[j].z);

                GameObject rFence = Instantiate(fencePrefabs[randRightFence], rPos, Quaternion.identity);
                GameObject lFence = Instantiate(fencePrefabs[randLeftFence], lPos, Quaternion.identity);

                rFence.transform.parent = script.transform;
                lFence.transform.parent = script.transform;

                script.prefabs.Add(rFence);
                script.prefabs.Add(lFence);
            }
            else
            {
                Vector3 rPos = (script.meshDataPoints.fencePointsRight[j] + script.meshDataPoints.fencePointsRight[j + 1]) / 2f;
                Vector3 lPos = (script.meshDataPoints.fencePointsLeft[j] + script.meshDataPoints.fencePointsLeft[j + 1]) / 2f;

                float rAngle = Mathf.Atan2(script.meshDataPoints.fencePointsRight[j + 1].y - script.meshDataPoints.fencePointsRight[j].y, script.meshDataPoints.fencePointsRight[j + 1].x - script.meshDataPoints.fencePointsRight[j].x) * Mathf.Rad2Deg;
                float lAngle = Mathf.Atan2(script.meshDataPoints.fencePointsLeft[j + 1].y - script.meshDataPoints.fencePointsLeft[j].y, script.meshDataPoints.fencePointsLeft[j + 1].x - script.meshDataPoints.fencePointsLeft[j].x) * Mathf.Rad2Deg;

                GameObject rFence = Instantiate(fencePrefabs[randRightFence], rPos, Quaternion.Euler(0f, 0f, rAngle));
                GameObject lFence = Instantiate(fencePrefabs[randLeftFence], lPos, Quaternion.Euler(0f, 0f, lAngle));

                rFence.transform.parent = script.transform;
                lFence.transform.parent = script.transform;

                script.prefabs.Add(rFence);
                script.prefabs.Add(lFence);
            }
        }

    }


    public void OnMeshTriggerInstantiate()
    {
        //Debug.Log("Instantiate");
        Vector3 position = transform.position;
        nextHMPrefab = Instantiate(prefab, position, Quaternion.identity);
        nextHMPrefab.name = "nextHMPrefab";
        HillMeshGeneration HillMeshGenerationScript = nextHMPrefab.GetComponent<HillMeshGeneration>();
        StartCoroutine(GenerateMesh(HillMeshGenerationScript));
    }

    public void OnMeshTriggerDestroy()
    {
        //Debug.Log("Destroy");
        if (prevHMPrefab != null)
        {
            Destroy(prevHMPrefab);
            prevHMPrefab = null;
        }
        prevHMPrefab = nextHMPrefab;
        nextHMPrefab = null; // Assign nextHMPrefab to null after destroying it
    }


    public void OnBoundsTrigger()
    {
        Debug.Log("Player Died..");
    }




}

