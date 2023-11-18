using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public struct MeshDataPoints
{
    public List<Vector3> terrainPoints; 
    public List<Vector3> edgePoints; 
    public List<Vector3> playerTrackLeft;
    public List<Vector3> playerTrackMiddle;
    public List<Vector3> playerTrackRight;
    public List<Vector3> fencePointsLeft;
    public List<Vector3> fencePointsRight;

    public MeshDataPoints(int xSize, int zSize)
    {
        terrainPoints = new List<Vector3>();
        playerTrackLeft = new List<Vector3>();
        playerTrackMiddle = new List<Vector3>();
        playerTrackRight = new List<Vector3>();
        edgePoints = new List<Vector3>();
        fencePointsLeft = new List<Vector3>();
        fencePointsRight = new List<Vector3>();
    }
}


[RequireComponent(typeof(MeshFilter))]
public class HillMeshGeneration : MonoBehaviour
{
    public HillMeshGeneration prevMesh;
    Mesh mesh; // The generated mesh
    Vector3[] vertices; // Array to store the vertices of the mesh
    Vector3[] endVertices; // Store the end coordinates for the next mesh'
    Vector3[] obstacleSpawnPoints = new Vector3[3];
    Vector2[] uvs;
    public MeshDataPoints meshDataPoints;
    MeshCollider meshCollider;
    public GameObject meshTriggerInstantiate;
    public GameObject meshTriggerDestroy;
    int[] triangles; // Array to store the triangle indices of the mesh
    int xSize = 7; // Number of vertices along the x-axis
    int zSize = 7; // Number of vertices along the z-axis


    float noiseScale = 1f; // Increase the scale for larger, smoother features
    float maxSlope = 5f;    // Reduce the maximum slope for flatter terrain
    float noiseAmplitude = .8f; // Adjust the amplitude of the Perlin noise


    void Start()
    {
        this.name = "Slope";
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        noiseScale = Random.Range(0.1f, 0.15f);
        meshDataPoints = new MeshDataPoints(xSize, zSize);
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        endVertices = new Vector3[zSize + 1];

    }


    // Generate the shape of the terrain mesh
    public void CreateShape()
    {
        if (prevMesh == null)
        {
            for (int i = 0, z = 0; z <= zSize; z++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    float y = CalculateSlope(x, z) + Mathf.PerlinNoise(x * noiseScale, z * noiseScale) * noiseAmplitude;
                    vertices[i] = new Vector3(x, y, z);

                    if (z == 0 || z == zSize)
                    {
                        meshDataPoints.edgePoints.Add(vertices[i]);
                    }

                    if (x == xSize)
                    {
                        endVertices[z] = vertices[i];
                    }

                    i++;

                }
            }
        }
        else
        {
           CreateShapeAtLocation();
        }

        CreateTriangles();
        CreateUVs(vertices.Length);
    }



    void CreateShapeAtLocation()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        endVertices = new Vector3[zSize + 1];

        for (int i = 0, z = 0; z <= zSize; z++)
        {

            for (int x = (int)prevMesh.endVertices[0].x; x <= (int)prevMesh.endVertices[0].x + xSize; x++)
            {

                float oldY = CalculateSlope(x, z) + Mathf.PerlinNoise(x * noiseScale, z * noiseScale) * noiseAmplitude;
                float newY = CalculateSlope(x, z) + Mathf.PerlinNoise((x + xSize) * noiseScale, (z + zSize) * noiseScale) * noiseAmplitude;

                float t = (float)(x - (int)prevMesh.endVertices[0].x) / (float)xSize;
                float y = Mathf.Lerp(oldY, newY, t);
                if (x == (int)prevMesh.endVertices[0].x && prevMesh.endVertices != null && z < prevMesh.endVertices.Length)
                {
                    vertices[i] = prevMesh.endVertices[z];
                }
                else
                {
                    vertices[i] = new Vector3(x, y, z);
                }

                if (x == (int)prevMesh.endVertices[0].x + xSize)
                {
                    endVertices[z] = vertices[i];
                }

                if (z == 0 || z == zSize)
                {
                    meshDataPoints.edgePoints.Add(vertices[i]);
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
        int vert = 0, tris = 0; 
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
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
                    meshDataPoints.terrainPoints.Add(temp);
                    if (z == 1)
                    {
                        if (x == 0)
                        {
                            obstacleSpawnPoints[0] = temp;
                        }
                        meshDataPoints.playerTrackRight.Add(temp);
                        if (x == xSize - 1 && GameManager.Instance.playerTrackRightZ == 0.0)
                        {
                            Vector3 woorldCoordinate = transform.TransformDirection(temp);
                            GameManager.Instance.playerTrackRightZ = temp.z;
                        }
                        
                    }
                    if (z == 3)
                    {
                        if (x == 0)
                        {
                            obstacleSpawnPoints[1] = temp;
                        }

                        meshDataPoints.playerTrackMiddle.Add(temp);
                        if (x==xSize-1 && GameManager.Instance.playerTrackMiddleZ == 0.0)
                        {
                            Vector3 woorldCoordinate = transform.TransformDirection(temp);
                            GameManager.Instance.playerTrackMiddleZ = temp.z;
                        }
                        
                    }
                    if (z == 5)
                    {
                        if (x == 0)
                        {
                            obstacleSpawnPoints[2] = temp;
                        }

                        meshDataPoints.playerTrackLeft.Add(temp);
                        if (x == xSize - 1 && GameManager.Instance.playerTrackLeftZ == 0.0)
                        {
                            Vector3 woorldCoordinate = transform.TransformDirection(temp);
                            GameManager.Instance.playerTrackLeftZ = temp.z;
                        }
                    }
                }

                vert++;
                tris += 6;
            }

            vert++;
        }
    }

    // Update the mesh with the generated vertices and triangles
    public void UpdateMesh()
    {
        if (mesh == null)
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        CreateMeshTriggers();
        gameObject.layer = LayerMask.NameToLayer("Ground");
        if (prevMesh != null)
            ObstacleManager.Instance.InstantiateObstacles(gameObject.transform, obstacleSpawnPoints);
        
        
    }



    // Calculate the slope at a given point and modify the height accordingly
    float CalculateSlope(int x, int z)
    {
        float slope = (-maxSlope / xSize) * x;
        return slope;
    }

    // For debugging purposes. 
    void OnDrawGizmos()
    {
        if (meshDataPoints.playerTrackLeft != null)
        {
            for (int i = 0; i < meshDataPoints.playerTrackLeft.Count; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(meshDataPoints.playerTrackLeft[i], 0.1f);
            }

            for (int i = 0; i < meshDataPoints.playerTrackMiddle.Count; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(meshDataPoints.playerTrackMiddle[i], 0.1f);
            }

            for (int i = 0; i < meshDataPoints.playerTrackRight.Count; i++)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(meshDataPoints.playerTrackRight[i], 0.1f);
            }
        }
    }

    void CreateMeshTriggers()
    {
        if (prevMesh!=null)
        {
            meshTriggerInstantiate = new GameObject();
            meshTriggerInstantiate.transform.parent = this.transform;
            BoxCollider boxInstantiate = meshTriggerInstantiate.AddComponent<BoxCollider>();
            boxInstantiate.size = new Vector3(0.5f, 5f, zSize);
            boxInstantiate.center = new Vector3(0f, 2f, 0f);
            boxInstantiate.transform.position = mesh.bounds.center;
            boxInstantiate.isTrigger = true;
            meshTriggerInstantiate.AddComponent<TriggerEventHandler>().gameObject.name = "meshTriggerInstantiate";
        }

    }
}


