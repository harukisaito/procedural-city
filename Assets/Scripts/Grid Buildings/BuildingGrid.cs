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

    public int cityLength;
    public int cityWidth;

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

    public List<Street> streetsXRight;
    public List<Street> streetsXLeft;
    public List<Street> streetsZRight;
    public List<Street> streetsZLeft;

    private void Start() {
        cityLength = gridLength * blockLength * buildingLength + (streetWidth * (gridLength - 1));
        cityWidth = gridWidth * blockWidth * buildingWidth + (streetWidth * (gridWidth - 1));

        blocks = new BuildingBlock[gridLength * gridWidth];

        StartCoroutine(SpawnBlock());
    }

    private IEnumerator SpawnBlock() {
        for(int x = 0, i = 0; x < gridLength; x++) {
            for(int z = 0; z < gridWidth; z++, i++) {
                CreateBlock(x, z, i);
                CreateStreets(x, z, i);

                yield return new WaitForSeconds(0.01f);
            }
        }
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cityPlaneScale = new Vector3(cityLength + 50, 1, cityWidth + 50) * scaleFactor;
        MoveCityToCenter();
        SetDistance();
        plane.transform.localPosition = new Vector3(
            0,
            -1f,
            0
        );
        plane.transform.localScale = cityPlaneScale;
        OnSpawnedBlocks();
    }

    private void CreateBlock(int x, int z, int i) {
        currentBlock = Instantiate<BuildingBlock>(blockSpawner);
        currentBlock.transform.SetParent(this.transform);
        currentBlock.transform.localPosition = 
            new Vector3(
                (((x * blockLength * buildingLength) + streetWidth * x) 
                    + buildingLength * 0.5f) * scaleFactor,
                0,
                (((z * blockWidth * buildingWidth) + streetWidth * z) 
                    + buildingWidth * 0.5f) * scaleFactor
            );
        currentBlock.transform.localScale = 
            currentBlock.transform.localScale * scaleFactor;
        blocks[i] = currentBlock;
    }

    private void CreateStreets(int x, int z, int i) {
        CreateStreetsX(x, z, i);
        CreateStreetsZ(x, z, i);
    }

    private void CreateStreetsX(int x, int z, int i) {
        if(x == 0 && z < gridWidth - 1) {
            GameObject streetRightGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            streetRightGameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            Street streetRight = streetRightGameObject.AddComponent<Street>();
            GameObject streetLeftGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            streetLeftGameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            Street streetLeft = streetLeftGameObject.AddComponent<Street>();
            streetRight.transform.SetParent(this.transform);
            streetRight.transform.localPosition = 
                new Vector3(
                    cityLength * 0.5f,
                    0,
                    (((z * blockWidth * buildingWidth) + streetWidth * z) 
                        + blockWidth * buildingWidth + streetWidth * 0.25f + CityMetrics.pavementWidth * 0.5f) 
                        * scaleFactor 
                );
            streetRight.transform.localScale = 
            new Vector3(
                cityLength, 
                1f, 
                (streetWidth * 0.5f) - CityMetrics.pavementWidth);
            streetLeft.transform.SetParent(this.transform);
            streetLeft.transform.localPosition = 
                new Vector3(
                    cityLength * 0.5f,
                    0,
                    (((z * blockWidth * buildingWidth) + streetWidth * z) 
                        + blockWidth * buildingWidth + streetWidth * 0.75f
                         - CityMetrics.pavementWidth * 0.5f) 
                        * scaleFactor 
                );
            streetLeft.transform.localScale = 
            new Vector3(
                cityLength, 
                1f, 
                (streetWidth * 0.5f) - CityMetrics.pavementWidth);
            streetsXRight.Add(streetRight);
            streetsXLeft.Add(streetLeft);
        }
    }

    private void CreateStreetsZ(int x, int z, int i) {
        if(z == 0 && x < gridLength - 1) {
            GameObject streetRightGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            streetRightGameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            Street streetRight = streetRightGameObject.AddComponent<Street>();
            GameObject streetLeftGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            streetLeftGameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            Street streetLeft = streetLeftGameObject.AddComponent<Street>();
            streetRight.transform.SetParent(this.transform);
            streetRight.transform.localPosition = 
                new Vector3(
                    (((x * blockLength * buildingLength) + streetWidth * x) 
                        + blockLength * buildingLength + streetWidth * 0.75f - CityMetrics.pavementWidth * 0.5f)
                        * scaleFactor,
                    0,
                    cityWidth * 0.5f
                );
            streetRight.transform.localScale = 
                new Vector3(
                    (streetWidth * 0.5f) - CityMetrics.pavementWidth, 
                    1, 
                    cityWidth
                    );
            streetLeft.transform.SetParent(this.transform);
            streetLeft.transform.localPosition = 
                new Vector3(
                    (((x * blockLength * buildingLength) + streetWidth * x) 
                        + blockLength * buildingLength + streetWidth * 0.25f + CityMetrics.pavementWidth * 0.5f)
                        * scaleFactor,
                    0,
                    cityWidth * 0.5f
                );
            streetLeft.transform.localScale = 
            new Vector3(
                (streetWidth * 0.5f) - CityMetrics.pavementWidth, 
                1, 
                cityWidth
            );
            streetsZRight.Add(streetRight);
            streetsZLeft.Add(streetLeft);
        }
    }

    private void MoveCityToCenter() {
        float x = cityLength * scaleFactor;
        float z = cityWidth * scaleFactor;
        x = x * 0.5f;
        z = z * 0.5f;

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
