using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public bool ConstrainRotation {
        set {
            BuildingSpawner.Instance.ConstrainRotation = value;
            print(value);
        }   
    }

    public void StartPhysics() {
        BuildingSpawner.Instance.StartPhysics();
    }

    public void ResetDrag() {
        BuildingSpawner.Instance.ResetDrag();
    }

    public void LerpBackToPosition() {
        BuildingSpawner.Instance.LerpToPosition();
    }
}
