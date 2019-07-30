using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingGrid : MonoBehaviour
{
    public static BuildingGrid Instance;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    [SerializeField] private BuildingBlock blockSpawner;
    [SerializeField] private GameObject cityPlane;
    [SerializeField] private AnimationCurve curve;
    private BuildingBlock currentBlock;

    private int gridLength = CityMetrics.cityGridLength;
    private int gridWidth = CityMetrics.cityGridWidth;

    private int blockLength = CityMetrics.blockLength;
    private int blockWidth = CityMetrics.blockWidth;

    private int buildingLength = CityMetrics.buildingLength;
    private int buildingWidth = CityMetrics.buildingWidth;

    private int streetWidth = CityMetrics.streetWidth;
    private float scaleFactor = CityMetrics.scaleFactor;

    private int cityLength;
    private int cityWidth;

    private float firstDistance;

    private Vector3 cityPlaneScale;
    public Vector3 CityPlaneScale {
        get {
            return cityPlaneScale;
        }
    }

    private Vector3 centeredPosition;
    private BuildingBlock[] blocks;
    public EventHandler SpawnedBlocks;

    private void Start() {
        cityLength = gridLength * blockLength * buildingLength;
        cityWidth = gridWidth * blockWidth * buildingWidth;

        blocks = new BuildingBlock[gridLength * gridWidth];

        StartCoroutine(SpawnBlock());
    }

    private IEnumerator SpawnBlock() {
        for(int x = 0, i = 0; x < gridLength; x++) {
            for(int z = 0; z < gridWidth; z++, i++) {
                currentBlock = Instantiate<BuildingBlock>(blockSpawner);
                currentBlock.transform.SetParent(this.transform);
                currentBlock.transform.localPosition = 
                    new Vector3(
                        ((x * blockLength * buildingLength) + streetWidth * x) * scaleFactor,
                        0,
                        ((z * blockWidth * buildingWidth) + streetWidth * z) * scaleFactor
                    );
                currentBlock.transform.localScale = 
                    currentBlock.transform.localScale * scaleFactor;
                blocks[i] = currentBlock;
                yield return new WaitForSeconds(0.05f);
            }
        }
        GameObject plane = Instantiate(cityPlane);
        cityPlaneScale = new Vector3(cityLength, 1, cityWidth) * scaleFactor * 0.25f;
        OnSpawnedBlocks();
        MoveCityToCenter();
        SetDistance();
        plane.transform.localScale = cityPlaneScale;
    }

    private void MoveCityToCenter() {
        float x = (cityLength + (streetWidth * (gridLength - 1))) * scaleFactor;
        float z = (cityWidth + (streetWidth * (gridWidth - 1))) * scaleFactor;
        x = x * 0.5f;
        z = z * 0.5f - 10 * scaleFactor;

        centeredPosition = new Vector3(-x, 0, -z);
        
        transform.position = centeredPosition;

    }

    private void SetDistance() {
        Vector3 cityCenter = Vector3.zero;
        for(int i = 0; i < blocks.Length; i++) {
            Vector3 blockPosition = blocks[i].transform.position + new Vector3(2, 0, 1);
            float distance = Vector3.Distance(cityCenter, blockPosition);
            if(i == 0) {
                firstDistance = distance;
            }
            distance /= firstDistance;
            
            blocks[i].distance = curve.Evaluate(1 - distance);
            blocks[i].SpawnBuildings();
        }
    }


    protected virtual void OnSpawnedBlocks() {
        if(SpawnedBlocks == null) {
            return;
        } 

        SpawnedBlocks(this, EventArgs.Empty);
    }

    
}
