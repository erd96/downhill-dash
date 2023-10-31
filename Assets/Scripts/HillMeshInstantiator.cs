using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HillMeshInstantiator : MonoBehaviour
{
    public GameObject prefab; // Reference to the prefab to be instantiated
    private GameObject lastInstantiatedPrefab;
    private Vector3[] startVertices; // Store the start vertices of the current mesh

    void Update()
    {
        // Check if the spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Calculate the position for the prefab based on the current position of this GameObject
            Vector3 position = transform.position;

            // Check if a prefab has already been instantiated
            if (lastInstantiatedPrefab != null)
            {
               
                GameObject NextHillPrefab = Instantiate(prefab, position, Quaternion.identity);
                // Access the script attached to the instantiated prefab
                HillMeshGeneration NextHillMeshGenerationScript = NextHillPrefab.GetComponent<HillMeshGeneration>();
                // Call the CreateShapeAtLocation function

                StartCoroutine(GenerateMesh(NextHillMeshGenerationScript));
            } 
            else
            {
                // Instantiate the prefab at the calculated position
                GameObject HillPrefab = Instantiate(prefab, position, Quaternion.identity);

                // Access the script attached to the instantiated prefab
                HillMeshGeneration HillMeshGenerationScript = HillPrefab.GetComponent<HillMeshGeneration>();

                // Use a coroutine to ensure the proper execution timing
                StartCoroutine(GenerateMesh(HillMeshGenerationScript));

            }
            
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

        // Set the last instantiated prefab
        lastInstantiatedPrefab = script.gameObject;
        startVertices = script.endVertices;

    }
}
