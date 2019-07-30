using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlock : MonoBehaviour
{
    [SerializeField] private Building[] buildings;

    private Building[] blockElements;

    public float distance;

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
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private void Building(int x, int z) {
        Building current = buildings[0];
        for(int i = 0; i < buildings.Length; i++) {
            if(Mathf.Abs(distance - buildings[i].height / CityMetrics.highestBuilding + Random.Range(0f, 2f))
             < Mathf.Abs(distance - current.height) / CityMetrics.highestBuilding) {
                current = buildings[i];
            }
        }
        Building instance = Instantiate(current);
        instance.transform.SetParent(transform);
        instance.transform.localPosition =
            new Vector3(x * CityMetrics.buildingLength, 0, z * CityMetrics.buildingWidth);
        instance.transform.localScale = 
                    instance.transform.localScale * CityMetrics.scaleFactor;
    }
}
