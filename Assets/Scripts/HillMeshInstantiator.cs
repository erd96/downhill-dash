using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HillMeshInstantiator : MonoBehaviour
{
    public GameObject prefab; // Reference to the prefab to be instantiated
    private GameObject prevHMPrefab;
    private GameObject nextHMPrefab;
    public GameObject[] treePrefabs;
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
        for (int i = 0; i < script.meshDataPoints.edgePoints.Length; i++)
        {
            int randTree = Random.Range(0, treePrefabs.Length);
            GameObject tree = Instantiate(treePrefabs[randTree], script.meshDataPoints.edgePoints[i], Quaternion.identity);
            tree.transform.parent = script.transform;
            script.prefabs.Add(tree);
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



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class HillMeshInstantiator : MonoBehaviour
//{
//    public GameObject prefab; // Reference to the prefab to be instantiated
//    private GameObject prevHMPrefab;
//    private GameObject nextHMPrefab;
//    public GameObject[] treePrefabs;
//    private Vector3[] startVertices; // Store the start vertices of the current mesh

//    private void Start()
//    {
//        Vector3 position = transform.position;
//        prevHMPrefab = Instantiate(prefab, position, Quaternion.identity);
//        prevHMPrefab.name = "prevHMPrefab";
//        HillMeshGeneration HillMeshGenerationScript = prevHMPrefab.GetComponent<HillMeshGeneration>();
//        StartCoroutine(GenerateMesh(HillMeshGenerationScript));
//    }


//    IEnumerator GenerateMesh(HillMeshGeneration script)
//    {
//        // Wait for a frame to ensure that the prefab is initialized
//        yield return new WaitForEndOfFrame();

//        // Check if startVertices is null, if so, call the regular CreateShape function
//        if (startVertices == null)
//        {

//            script.CreateShape();
//        }
//        else
//        {
//            // Call the CreateShapeAtLocation function and pass the startVertices
//            script.CreateShapeAtLocation(startVertices);
//        }

//        // Update the mesh
//        script.UpdateMesh();

//        startVertices = script.endVertices;

//        // Set the last instantiated prefab

//        InstantiateEdgePrefabs(script);

//    }


//    void InstantiateEdgePrefabs(HillMeshGeneration script)
//    {
//        for (int i = 0; i < script.meshDataPoints.edgePoints.Length; i++)
//        {
//            int randTree = Random.Range(0, treePrefabs.Length);
//            GameObject tree = Instantiate(treePrefabs[randTree], script.meshDataPoints.edgePoints[i], Quaternion.identity);
//            tree.transform.parent = script.transform;
//            script.prefabs.Add(tree);
//        }

//    }


//    public void OnMeshTriggerInstantiate()
//    {
//        //Debug.Log("Instantiate");
//        Vector3 position = transform.position;
//        nextHMPrefab = Instantiate(prefab, position, Quaternion.identity);
//        nextHMPrefab.name = "nextHMPrefab";
//        HillMeshGeneration HillMeshGenerationScript = nextHMPrefab.GetComponent<HillMeshGeneration>();
//        StartCoroutine(GenerateMesh(HillMeshGenerationScript));
//    }

//    public void OnMeshTriggerDestroy()
//    {
//        //Debug.Log("Destroy");
//        if (prevHMPrefab != null)
//        {
//            Destroy(prevHMPrefab);
//            prevHMPrefab = null;
//        }
//        prevHMPrefab = nextHMPrefab;
//        nextHMPrefab = null; // Assign nextHMPrefab to null after destroying it
//    }


//    public void OnBoundsTrigger()
//    {
//        Debug.Log("Player Died..");
//    }




//}
