using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainChunk : MonoBehaviour
{
    [SerializeField] private Material rockMaterial; 
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uv;
    private Vector3 cityPlaneScale;

    private Mesh mesh;

    private int sizeX = CityMetrics.mountainMeshSizeX;
    private int sizeZ = CityMetrics.mountainMeshSizeZ;

    public int index;

    public Dictionary<Direction, MountainChunk> neighbors = new Dictionary<Direction, MountainChunk>();
    public List<MountainChunk> neighborList = new List<MountainChunk>();
    private Dictionary<Direction, List<Vertex>> outerVertices = new Dictionary<Direction, List<Vertex>>();
    public Dictionary<Direction, Vertex[]> cornerVertices = new Dictionary<Direction, Vertex[]>();

    private void Start() {
        vertices = new Vector3[(sizeX + 1) * (sizeZ + 1)];
        triangles = new int[sizeX * sizeZ * 6];
        uv = new Vector2[vertices.Length];

        cityPlaneScale = BuildingGrid.Instance.CityPlaneScale;
        AddMountainMeshGrid();
    }

    private void AddMountainMeshGrid() {
        mesh = gameObject.AddComponent<MeshFilter>().mesh;
        mesh = GetComponent<MeshFilter>().mesh;

        AddVertices();
        AddTriangles();
        Refresh();
        StartCoroutine(EditEdgeVertices());

        GetComponent<MeshRenderer>().material = rockMaterial;
    }

    private void Refresh() {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
    }

    private void AddVertices() {
        for(int x = 0, i = 0; x <= sizeX; x++) {
            for(int z = 0; z <= sizeZ; z++, i++) {
                float xValue = cityPlaneScale.x * x / CityMetrics.mountainMeshSizeX * 10;
                float zValue = cityPlaneScale.z * z / CityMetrics.mountainMeshSizeZ * 10;

                float xFactor = UnityEngine.Random.Range(0f, 1f);
                float yFactor = UnityEngine.Random.Range(0f, 1f);

                float yValue = Mathf.PerlinNoise(xValue * 0.1f, zValue * 0.1f) * CityMetrics.mountainHeightFactor;
                vertices[i] = new Vector3(
                    xValue,
                    yValue,
                    zValue
                );
                uv[i] = new Vector2(xValue / sizeX, zValue / sizeZ);

                if(x == 0) {
                    AddOuterVertices(Direction.Left, i);
                    if(z == 0) {
                        AddCornerVertices(Direction.Left, 0, i);
                        AddCornerVertices(Direction.Down, 0, i);
                    }
                    else if(z == sizeZ) {
                        AddCornerVertices(Direction.Left, 1, i);
                        AddCornerVertices(Direction.Up, 0, i);
                    }
                }  
                if(x == sizeX) {
                    AddOuterVertices(Direction.Right, i);
                    if(z == 0) {
                        AddCornerVertices(Direction.Right, 0, i);
                        AddCornerVertices(Direction.Down, 1, i);
                    }
                    else if(z == sizeZ) {
                        AddCornerVertices(Direction.Right, 1, i);
                        AddCornerVertices(Direction.Up, 1, i);
                    }
                }
                if(z == 0) {
                    AddOuterVertices(Direction.Down, i);
                }
                if(z == sizeZ) {
                    AddOuterVertices(Direction.Up, i);
                }
            }
        }
    }

    private void AddOuterVertices(Direction direction, int vertexIndex) {
        if(!outerVertices.ContainsKey(direction)) {
            outerVertices[direction] = new List<Vertex>();
        }
        Vertex currentVertex = new Vertex(index, vertexIndex, vertices[vertexIndex]);

        outerVertices[direction].Add(currentVertex);
    }

    private void AddCornerVertices(Direction direction, int index, int vertexIndex) {
        if(!cornerVertices.ContainsKey(direction)) {
            cornerVertices[direction] = new Vertex[2];
        }
        Vertex currentVertex = new Vertex(index, vertexIndex, vertices[vertexIndex]);
        cornerVertices[direction][index] = currentVertex;
    }

    private void AddTriangles() {
        for(int triIndex = 0, verIndex = 0, x = 0; x < sizeX; x++, verIndex ++) {
            for(int z = 0; z < sizeZ; z++, triIndex += 6, verIndex++) {
                triangles[triIndex] = verIndex;
                triangles[triIndex + 1] = triangles[triIndex + 4] = verIndex + 1;
                triangles[triIndex + 2] = triangles[triIndex + 3] = verIndex + sizeX + 1;
                triangles[triIndex + 5] = verIndex + sizeX + 2;
            }
        }
    }

    private IEnumerator EditEdgeVertices() {
        yield return new WaitForEndOfFrame();
        EditEdge();
        EditCorners();
        Refresh();
    }

    private void EditEdge() {
        for(Direction direction = Direction.Up; direction <= Direction.Left; direction++) {
            if(neighbors.ContainsKey(direction)) {
                for(int i = 0; i < outerVertices[direction].Count; i++) {
                    int currentIndex = outerVertices[direction][i].index;
                    MountainChunk neighbor = neighbors[direction];
                    
                    int neighborIndex = neighbor.outerVertices[direction.Opposite()][i].index;
                    vertices[currentIndex].y = neighbor.vertices[neighborIndex].y;
                }
            }
            else {
                for(int i = 0; i < outerVertices[direction].Count; i++) {
                    int currentIndex = outerVertices[direction][i].index;
                    vertices[currentIndex].y = 0;
                } 
            }
        }
    }

    private void EditCorners() {
        if(!neighbors.ContainsKey(Direction.Up)) {
            int cIndex1 = cornerVertices[Direction.Up][0].index;
            int cIndex2 = cornerVertices[Direction.Up][1].index;

            vertices[cIndex1].y = 0;
            vertices[cIndex2].y = 0;
        }
        if(!neighbors.ContainsKey(Direction.Right)) {
            int cIndex1 = cornerVertices[Direction.Right][0].index;
            int cIndex2 = cornerVertices[Direction.Right][1].index;

            vertices[cIndex1].y = 0;
            vertices[cIndex2].y = 0;
        }
        if(neighbors.ContainsKey(Direction.Up) && neighbors.ContainsKey(Direction.Right)) {
            int index = cornerVertices[Direction.Up][1].index;
            vertices[index].y = 0;
        }
    }

    public void SetNeighbor(Direction direction, MountainChunk neighbor) {
        if(neighbor != null) {
            neighbors[direction] = neighbor;
            neighbor.neighbors[direction.Opposite()] = this;

            neighborList.Add(neighbor);
            neighbor.neighborList.Add(this);
        }
    }
}
