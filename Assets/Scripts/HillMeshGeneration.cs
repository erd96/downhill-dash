using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public struct MeshDataPoints
{
    public Vector3[] terrainPoints; // Array to store the center points of each square
    public List<Vector3> edgePoints; // Array to store the center points of each square
    public List<Vector3> fencePointsLeft;
    public List<Vector3> fencePointsRight;

    public MeshDataPoints(int xSize, int zSize)
    {
        terrainPoints = new Vector3[(xSize + 1) * (zSize - 1) + xSize * (zSize - 2)]; // Coordinates of centers of squares + vertices for the terrain middle.
        //edgePoints = new Vector3[(xSize + 1) * 2 + xSize * 2]; // Coordinates of centers of squares + vertices for the edges.
        edgePoints = new List<Vector3>();
        fencePointsLeft = new List<Vector3>();
        fencePointsRight = new List<Vector3>();
    }
}



// Ensure the presence of a MeshFilter component
[RequireComponent(typeof(MeshFilter))]
public class HillMeshGeneration : MonoBehaviour
{
    Mesh mesh; // The generated mesh
    Vector3[] vertices; // Array to store the vertices of the mesh
    Vector2[] uvs;
    public Vector3[] endVertices; // Store the end coordinates for the next mesh'
    public MeshDataPoints meshDataPoints;
    MeshCollider meshCollider;
    public GameObject meshTriggerInstantiate;
    public GameObject meshTriggerDestroy;
    public GameObject leftBoundsTrigger;
    public GameObject rightBoundsTrigger;
    public List<GameObject> prefabs = new List<GameObject>();
    int[] triangles; // Array to store the triangle indices of the mesh
    int xSize = 50; // Number of vertices along the x-axis
    int zSize = 10; // Number of vertices along the z-axis


    float noiseScale = 1f; // Increase the scale for larger, smoother features
    float maxSlope;    // Reduce the maximum slope for flatter terrain
    float noiseAmplitude = .8f; // Adjust the amplitude of the Perlin noise

    // Called at the start of the script
    void Start()
    {
        // Initialize the mesh and assign it to the MeshFilter component
        maxSlope = GameManager.Instance.maxSlope;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        noiseScale = Random.Range(0.1f, 0.15f);
        meshDataPoints = new MeshDataPoints(xSize, zSize);

    }


    // Generate the shape of the terrain mesh
    public void CreateShape()
    {

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        endVertices = new Vector3[zSize + 1];
        int index = 0;

        // Iterate through each vertex and calculate its height using Perlin noise and slope
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {

                //Calculate the height using Perlin noise and slope
                float y = CalculateSlope(x, z) + Mathf.PerlinNoise(x * noiseScale, z * noiseScale) * noiseAmplitude;

                //Set the vertex position in the array
                vertices[i] = new Vector3(x, y, z);
                if (x == xSize)
                {
                    endVertices[z] = vertices[i];
                }
                if (z == 0)
                {
                    meshDataPoints.edgePoints.Add(vertices[i]);
                }
                if (z == zSize)
                {
                    meshDataPoints.edgePoints.Add(vertices[i]);
                }

                if (z != 0 && z != zSize)
                {

                    meshDataPoints.terrainPoints[index + x] = vertices[i];
                    if (x == xSize)
                    {
                        index += (xSize + 1) + xSize;
                    }
                }

                i++;

            }
        }
        CreateTriangles();
        CreateUVs(vertices.Length);
    }



    public void CreateShapeAtLocation(Vector3[] startVertices)
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        endVertices = new Vector3[zSize + 1];
        int index = 0;

        // Iterate through each vertex and calculate its height using Perlin noise and slope
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = (int)startVertices[0].x; x <= (int)startVertices[0].x + xSize; x++)
            {


                // Calculate the height using blended Perlin noise and slope
                float oldY = CalculateSlope(x, z) + Mathf.PerlinNoise(x * noiseScale, z * noiseScale) * noiseAmplitude;
                float newY = CalculateSlope(x, z) + Mathf.PerlinNoise((x + xSize) * noiseScale, (z + zSize) * noiseScale) * noiseAmplitude;

                // Interpolate between old and new heights based on the progress of the transition
                float t = (float)(x - (int)startVertices[0].x) / (float)xSize;
                float y = Mathf.Lerp(oldY, newY, t);



                if (x == (int)startVertices[0].x && startVertices != null && z < startVertices.Length)
                {
                    vertices[i] = startVertices[z];
                }
                else
                {
                    vertices[i] = new Vector3(x, y, z);
                }

                if (x == (int)startVertices[0].x + xSize)
                {
                    endVertices[z] = vertices[i];
                }


                if (z == 0)
                {
                    meshDataPoints.edgePoints.Add(vertices[i]);
                }
                if (z == zSize)
                {
                    meshDataPoints.edgePoints.Add(vertices[i]);
                }

                if (z != 0 && z != zSize)
                {

                    meshDataPoints.terrainPoints[index + (x - (int)startVertices[0].x)] = vertices[i];
                    if (x == ((int)startVertices[0].x + xSize))
                    {
                        index += (xSize + 1) + xSize;
                    }
                }


                i++;
            }
        }
        CreateTriangles();
        CreateUVs(vertices.Length);

    }

    void CreateUVs(int length)
    {
        uvs = new Vector2[length];

        for (int i = 0, z=0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                uvs[i] = new Vector2((float)x / xSize, (float)z / zSize);
                i++;
            }
        }
    }

    void CreateTriangles()
    {
        triangles = new int[xSize * zSize * 6];
        int vert = 0, tris = 0; // Variables to track vertex and triangle indices
        // Generate triangles for the mesh to create the surface
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                // Define the triangles based on the vertex indices
                triangles[tris + 0] = vert;  // BL
                triangles[tris + 1] = vert + xSize + 1; //TL
                triangles[tris + 2] = vert + 1; // BR
                triangles[tris + 3] = vert + 1; // BR
                triangles[tris + 4] = vert + xSize + 1; // TL
                triangles[tris + 5] = vert + xSize + 2; //TR


                Vector3 temp = (vertices[triangles[tris + 0]] + vertices[triangles[tris + 1]] + vertices[triangles[tris + 5]] + vertices[triangles[tris + 3]]) / 4f;

                if (z == 0 | z == zSize - 1)
                {
                    int index = z == 0 ? x + xSize + 1 : x + xSize * 2 + 1;

                    if (z == 0)
                    {
                        meshDataPoints.fencePointsRight.Add(temp);
                    }  
                    else
                    {
                        meshDataPoints.fencePointsLeft.Add(temp);
                    }
                }
                else
                {
                    meshDataPoints.terrainPoints[(xSize + 1) * z + xSize * (z - 1) + x] = temp;
                }

                // Update the vertex and triangle indices
                vert++;
                tris += 6;
            }

            vert++;
        }
    }

    // Update the mesh with the generated vertices and triangles
    public void UpdateMesh()
    {
        // Check if the mesh has been initialized
        if (mesh == null)
        {
            // Initialize the mesh and assign it to the MeshFilter component
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        // Clear the existing mesh data
        mesh.Clear();

        // Set the vertices and triangles of the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        // Recalculate the normals for proper shading
        mesh.RecalculateNormals();

        // Add a mesh collider once the mesh is generated and updated.
        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        CreateMeshTriggers();
        gameObject.layer = LayerMask.NameToLayer("Ground");
    }



    // Calculate the slope at a given point and modify the height accordingly
    float CalculateSlope(int x, int z)
    {
        float slope = (-maxSlope / xSize) * x;
        return slope;
    }
    //void OnDrawGizmos()
    //{
    //    if (meshDataPoints.edgePoints != null)
    //    {
            
    //        for (int i = 0; i < meshDataPoints.edgePoints.Count; i++)
    //        {
    //            Gizmos.color = Color.red;
    //            Gizmos.DrawSphere(meshDataPoints.edgePoints[i], 0.1f);
    //        }

    //        for (int i = 0; i < meshDataPoints.fencePointsLeft.Count; i++)
    //        {
    //            Gizmos.color = Color.cyan;
    //            Gizmos.DrawSphere(meshDataPoints.fencePointsLeft[i], 0.1f);
    //        }

    //        for (int i = 0; i < meshDataPoints.fencePointsRight.Count; i++)
    //        {
    //            Gizmos.color = Color.magenta;
    //            Gizmos.DrawSphere(meshDataPoints.fencePointsRight[i], 0.1f);

    //        }
    //        for (int i = 0; i < meshDataPoints.terrainPoints.Length; i++)
    //        {
    //            Gizmos.color = Color.blue;
    //            Gizmos.DrawCube(meshDataPoints.terrainPoints[i], new Vector3(0.1f, 0.1f, 0.1f));
    //        }
    //    }
    //}

    void CreateMeshTriggers()
    {

        meshTriggerInstantiate = new GameObject();
        meshTriggerDestroy = new GameObject();
        leftBoundsTrigger = new GameObject();
        rightBoundsTrigger = new GameObject();

        meshTriggerInstantiate.transform.parent = this.transform;
        meshTriggerDestroy.transform.parent = this.transform;
        leftBoundsTrigger.transform.parent = this.transform;
        rightBoundsTrigger.transform.parent = this.transform;

        BoxCollider boxInstantiate = meshTriggerInstantiate.AddComponent<BoxCollider>();
        BoxCollider boxDestroy = meshTriggerDestroy.AddComponent<BoxCollider>();
        BoxCollider leftBounds = leftBoundsTrigger.AddComponent<BoxCollider>();
        BoxCollider rightBounds = rightBoundsTrigger.AddComponent<BoxCollider>();

        boxInstantiate.size = new Vector3(0.5f, 5f, zSize);
        boxDestroy.size = new Vector3(0.5f, 5f, zSize);
        leftBounds.size = new Vector3(xSize, 15f, 0.5f);
        rightBounds.size = new Vector3(xSize, 15f, 0.5f);

        boxInstantiate.center = new Vector3(0f, 2f, 0f);
        boxDestroy.center = new Vector3(0f, 2f, 0f);
        leftBounds.center = new Vector3(0f, 2f, 0f);
        rightBounds.center = new Vector3(0f, 2f, 0f);


        boxInstantiate.transform.position = mesh.bounds.center;
        boxDestroy.transform.position = new Vector3(mesh.bounds.center.x - mesh.bounds.extents.x + xSize / 20, mesh.bounds.extents.y + mesh.bounds.center.y - 1, mesh.bounds.center.z);
        leftBounds.transform.position = new Vector3(mesh.bounds.center.x, mesh.bounds.center.y, mesh.bounds.center.z + mesh.bounds.extents.z);
        rightBounds.transform.position = new Vector3(mesh.bounds.center.x, mesh.bounds.center.y, mesh.bounds.center.z - mesh.bounds.extents.z);

        boxInstantiate.isTrigger = true;
        boxDestroy.isTrigger = true;
        leftBounds.isTrigger = true;
        rightBounds.isTrigger = true;

        // Add collider triggers to the specific GameObjects
        meshTriggerInstantiate.AddComponent<TriggerEventHandler>().gameObject.name = "meshTriggerInstantiate";
        meshTriggerDestroy.AddComponent<TriggerEventHandler>().gameObject.name = "meshTriggerDestroy";
        leftBoundsTrigger.AddComponent<TriggerEventHandler>().gameObject.name = "leftBoundsTrigger";
        rightBoundsTrigger.AddComponent<TriggerEventHandler>().gameObject.name = "rightBoundsTrigger";
    }
}


