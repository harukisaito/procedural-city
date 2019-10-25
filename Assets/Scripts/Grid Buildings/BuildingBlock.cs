using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlock : MonoBehaviour
{
    [SerializeField] private Building[] buildings;

    private Building[] blockElements;

    public float distance;
    public Vector3 center;

    private void Start() {
        blockElements = new Building[CityMetrics.blockLength * CityMetrics.blockWidth];
    }


    public void SpawnBuildings() {
        StartCoroutine(SetBuildings());
    }

    private IEnumerator SetBuildings() {
        for(int x = 0; x < CityMetrics.blockLength; x++) {
            for(int z = 0; z < CityMetrics.blockWidth; z++) {
                Building(x, z);
                yield return new WaitForSeconds(0.1f);
            }
        }
        SetCenterPosition();
        SpawnPavement();
    }

    private void Building(int x, int z) {
        Building current = 
        distance < 0.5f? buildings[Random.Range(0, 5)] :
        buildings[Random.Range(5, buildings.Length)];
        // for(int i = 0; i < buildings.Length; i++) {
        //     if(Mathf.Abs(distance - buildings[i].height / CityMetrics.highestBuilding)
        //      < Mathf.Abs(distance - current.height) / CityMetrics.highestBuilding) {
        //         current = buildings[i];
        //     }
        // }

        Building instance = Instantiate(current);
        instance.transform.SetParent(transform);
        instance.transform.localPosition =
            new Vector3(
                x * CityMetrics.buildingLength,
                instance.height * 0.5f, 
                z * CityMetrics.buildingWidth
            );
        instance.transform.localScale = 
            instance.transform.localScale * CityMetrics.scaleFactor;
    }
    
    private void SetCenterPosition() {
        Vector2 block = new Vector2(CityMetrics.blockLength, CityMetrics.blockWidth);
        Vector2 building = new Vector2(CityMetrics.buildingLength, CityMetrics.buildingWidth);
        center = new Vector3(
            block.x * 0.5f * building.x - building.x * 0.5f,
            -0.25f,
            block.y * 0.5f * building.y - building.y * 0.5f
        );
        center = transform.position + center;
    }

    private void SpawnPavement() {
        GameObject pavement = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pavement.transform.localScale = new Vector3(
            CityMetrics.blockLength * CityMetrics.buildingLength + CityMetrics.pavementWidth * 2,
            0.5f,
            CityMetrics.blockLength * CityMetrics.buildingLength + CityMetrics.pavementWidth * 2
        );
        pavement.transform.position = center;
    }
}
