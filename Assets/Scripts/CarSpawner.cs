using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public static CarSpawner Instance;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public Car carPrefab;

    public void OnSpawnedBlocks(object o, EventArgs e) {
        StartCoroutine(SpawnCars());        
    }   

    private IEnumerator SpawnCars() {
        List<Street> streetsXRight = BuildingGrid.Instance.streetsXRight;
        List<Street> streetsXLeft = BuildingGrid.Instance.streetsXLeft;
        List<Street> streetsZRight = BuildingGrid.Instance.streetsZRight;
        List<Street> streetsZLeft = BuildingGrid.Instance.streetsZLeft;

        while(true) {
            for(int i = 0; i < streetsXRight.Count; i++) {
                Car car = Instantiate<Car>(carPrefab);
                car.MovementDirection = CarMovementDirection.East;
                car.transform.localPosition = streetsXRight[i].transform.position - new Vector3(BuildingGrid.Instance.cityLength * 0.5f, 0, 0);
            }
            for(int i = 0; i < streetsXLeft.Count; i++) {
                Car car = Instantiate<Car>(carPrefab);
                car.MovementDirection = CarMovementDirection.West;
                car.transform.localPosition = streetsXLeft[i].transform.position + new Vector3(BuildingGrid.Instance.cityLength * 0.5f, 0, 0);
            }
            for(int i = 0; i < streetsZRight.Count; i++) {
                Car car = Instantiate<Car>(carPrefab);
                car.MovementDirection = CarMovementDirection.North;
                car.transform.localPosition = streetsZRight[i].transform.position - new Vector3(0, 0, BuildingGrid.Instance.cityWidth * 0.5f);
                car.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
            for(int i = 0; i < streetsZLeft.Count; i++) {
                Car car = Instantiate<Car>(carPrefab);
                car.MovementDirection = CarMovementDirection.South;
                car.transform.localPosition = streetsZLeft[i].transform.position + new Vector3(0, 0, BuildingGrid.Instance.cityWidth * 0.5f);
                car.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
            yield return new WaitForSeconds(UnityEngine.Random.Range(1, 3));
        }
    }
}
