using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainGrid : MonoBehaviour
{
    public static MountainGrid Instance;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
    [SerializeField] private MountainChunk mountainChunkPrefab;

    public MountainChunk[] mountainChunks;
    private List<Vertex> cornerVertices = new List<Vertex>();

    private void AddMoutains() {
        mountainChunks = new MountainChunk[CityMetrics.terrainGridLength * CityMetrics.terrainGridWidth];
        float halfX = (float)CityMetrics.terrainGridLength / 2f;
        halfX -= 0.5f;
        float halfZ = (float)CityMetrics.terrainGridWidth / 2f;
        halfZ -= 0.5f;

        for(int x = 0, i = 0; x < CityMetrics.terrainGridLength; x++) {
            for(int z = 0; z < CityMetrics.terrainGridWidth; z++, i++) {
                if(z != halfZ || x != halfX) {
                    MountainChunk chunk = Instantiate<MountainChunk>(mountainChunkPrefab);
                    chunk.transform.SetParent(transform);
                    chunk.transform.localPosition = 
                        new Vector3(
                            BuildingGrid.Instance.CityPlaneScale.x * 10 * x,
                            0f,
                            BuildingGrid.Instance.CityPlaneScale.z * 10 * z);
                    mountainChunks[i] = chunk;
                    chunk.index = i; 
                    if(z > 0) {
                        chunk.SetNeighbor(Direction.Down, mountainChunks[i -1]);
                    }
                    if(x > 0) {
                        chunk.SetNeighbor(Direction.Left, mountainChunks[i - CityMetrics.terrainGridWidth]);
                    }
                }
                else {
                    mountainChunks[i] = null;
                }
            }
        }
        MoveToCenter();
    }

    private void MoveToCenter() {
        float x = BuildingGrid.Instance.CityPlaneScale.x * CityMetrics.terrainGridLength * 5;
        float z = BuildingGrid.Instance.CityPlaneScale.z * CityMetrics.terrainGridWidth * 5;

        transform.position = new Vector3(-x, 0f, -z);
    }

    public void OnSpawnedBlocks(object o, EventArgs e) {
        AddMoutains();
    }

}
