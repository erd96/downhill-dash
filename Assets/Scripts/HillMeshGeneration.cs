using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public struct MeshDataPoints
{
    public Vector3[] terrainPoints; // Array to store the center points of each square
    public Vector3[] edgePoints; // Array to store the center points of each square

    // Constructor to initialize the centerPoints array with the appropriate size
    public MeshDataPoints(int xSize, int zSize)
    {
        //terrainPoints = new Vector3[(xSize) * (zSize) - xSize*2];
        terrainPoints = new Vector3[(xSize+1)*(zSize-1) + xSize*(zSize-2)]; // Coordinates of centers of squares + vertices for the terrain middle.
        //edgePoints = new Vector3[(xSize) * 2];
        edgePoints = new Vector3[(xSize+1) * 2 + xSize*2]; // Coordinates of centers of squares + vertices for the edges.
    }
}



// Ensure the presence of a MeshFilter component
[RequireComponent(typeof(MeshFilter))]
public class HillMeshGeneration : MonoBehaviour
{
    Mesh mesh; // The generated mesh
    Vector3[] vertices; // Array to store the vertices of the mesh
    public Vector3[] endVertices; // Store the end coordinates for the next mesh'
    public MeshDataPoints meshDataPoints;
    MeshCollider meshCollider;
    GameObject meshTriggerInstantiate;
    GameObject meshTriggerDestroy;
    GameObject leftBoundsTrigger;
    GameObject rightBoundsTrigger;
    public List<GameObject> prefabs = new List<GameObject>(); 
    int[] triangles; // Array to store the triangle indices of the mesh
    int xSize = 50; // Number of vertices along the x-axis
    int zSize = 10; // Number of vertices along the z-axis
    float maxSlope = 10.0f; // Maximum slope of the terrain
    float noiseScale = 0.15f; // Scale of Perlin noise

    // Called at the start of the script
    void Start()
    {
        // Initialize the mesh and assign it to the MeshFilter component
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
                float y = CalculateSlope(x, z) + Mathf.PerlinNoise(x * noiseScale, z * noiseScale) * 2f;

                //Set the vertex position in the array
                vertices[i] = new Vector3(x, y, z);
                if (x == xSize)
                {
                    endVertices[z] = vertices[i];
                }
                if (z==0)
                {
                    meshDataPoints.edgePoints[x] = vertices[i];
                }
                if (z==zSize)
                {
                    meshDataPoints.edgePoints[meshDataPoints.edgePoints.Length - xSize + x -1] = vertices[i];
                }
                
                if (z!=0 && z!=zSize)
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
    }


    // Generate the shape of the terrain mesh

    public void CreateShapeAtLocation(Vector3[] startVertices)
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        endVertices = new Vector3[zSize + 1];
        int index = 0;

        // Iterate through each vertex and calculate its height using Perlin noise and slope
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = (int)startVertices[0].x; x <= (int)startVertices[0].x+xSize; x++)
            {
                
               
                float y = CalculateSlope(x, z) + Mathf.PerlinNoise(x * noiseScale, z * noiseScale) * 2f;

               

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
                    meshDataPoints.edgePoints[x - (int)startVertices[0].x] = vertices[i];
                }
                if (z == zSize)
                {
                    meshDataPoints.edgePoints[meshDataPoints.edgePoints.Length - ((int)startVertices[0].x + xSize) + x - 1] = vertices[i];
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
                
                if (z==0 | z== zSize-1 )
                {
                    int index = z == 0 ? x+xSize+1 : x+xSize*2+1;
                    meshDataPoints.edgePoints[index] = temp;
                }
                else
                {
                    meshDataPoints.terrainPoints[(xSize+1)*z +xSize*(z-1) + x] = temp; 
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

        // Recalculate the normals for proper shading
        mesh.RecalculateNormals();

        // Add a mesh collider once the mesh is generated and updated.
        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        CreateMeshTriggers();
    }



    // Calculate the slope at a given point and modify the height accordingly
    float CalculateSlope(int x, int z)
    {
        float slope = (-maxSlope / xSize) * x;
        return slope;
    }
    void OnDrawGizmos()
    {
        if (meshDataPoints.edgePoints != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < meshDataPoints.edgePoints.Length; i++)
            {
                Gizmos.DrawSphere(meshDataPoints.edgePoints[i], 0.1f);
            }

            for (int i = 0; i < meshDataPoints.terrainPoints.Length; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(meshDataPoints.terrainPoints[i], new Vector3(0.1f, 0.1f, 0.1f));
            }
        }
    }

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

        BoxCollider boxInstantiate =  meshTriggerInstantiate.AddComponent<BoxCollider>();
        BoxCollider boxDestroy = meshTriggerDestroy.AddComponent<BoxCollider>();
        BoxCollider leftBounds = leftBoundsTrigger.AddComponent<BoxCollider>();
        BoxCollider rightBounds = rightBoundsTrigger.AddComponent<BoxCollider>();

        boxInstantiate.size = new Vector3(0.5f, 5f, zSize);
        boxDestroy.size = new Vector3(0.5f, 5f, zSize);
        leftBounds.size = new Vector3(xSize, 10f, 0.5f);
        rightBounds.size = new Vector3(xSize, 10f, 0.5f);

        boxInstantiate.center = new Vector3(0f, 2f, 0f);
        boxDestroy.center = new Vector3(0f, 2f, 0f);
        leftBounds.center = new Vector3(0f, 2f, 0f);
        rightBounds.center = new Vector3(0f, 2f, 0f);


        boxInstantiate.transform.position = mesh.bounds.center;
        boxDestroy.transform.position = new Vector3(mesh.bounds.center.x - mesh.bounds.extents.x +xSize/20, mesh.bounds.extents.y + mesh.bounds.center.y -1, mesh.bounds.center.z);
        leftBounds.transform.position = new Vector3(mesh.bounds.center.x, -mesh.bounds.extents.y, mesh.bounds.center.z + mesh.bounds.extents.z);
        rightBounds.transform.position = new Vector3(mesh.bounds.center.x, -mesh.bounds.extents.y, mesh.bounds.center.z - mesh.bounds.extents.z);

        boxInstantiate.isTrigger = true;
        boxDestroy.isTrigger = true;
        leftBounds.isTrigger = true;
        rightBounds.isTrigger = true;
    }
}
