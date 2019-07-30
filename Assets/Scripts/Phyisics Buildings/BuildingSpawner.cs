using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    public static BuildingSpawner Instance;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    [SerializeField] private GameObject[] buildingPrefabs;
    [SerializeField] private int spawnWaveCount;
    [SerializeField] private float dragAmount;

    private GameObject currentBuilding;

    private bool constrainRotation;

    private Vector2 originalDrag;

    private List<GameObject> buildings = new List<GameObject>();
    private Dictionary<GameObject ,Vector3> originalPositions = new Dictionary<GameObject, Vector3>();

    public bool ConstrainRotation {
        get {
            return constrainRotation;
        }
        set {
            constrainRotation = value;
        }
    }

    private void Start() {
        for(int i = 0; i < spawnWaveCount; i++) {
            foreach(var b in buildingPrefabs) {
                currentBuilding = Instantiate(b);
                Vector2 position = Random.insideUnitCircle * 10;
                currentBuilding.transform.localPosition = new Vector3(position.x, 0f , position.y);
                currentBuilding.transform.SetParent(this.transform);
                buildings.Add(currentBuilding);
            }
        }
    }

    public void StartPhysics() {
        foreach(var b in buildings) {
            List<GameObject> children = new List<GameObject>();
            children = GetChildren(b);
            foreach(var c in children) {
                if(c.GetComponent<Rigidbody>() != null) {
                    return;
                }
                originalPositions.Add(c, c.transform.position);
                Rigidbody body;
                c.GetComponent<Collider>().enabled = false;
                c.AddComponent<BoxCollider>();
                c.AddComponent<Rigidbody>();
                body = c.GetComponent<Rigidbody>();
                if(constrainRotation) {
                    body.constraints = RigidbodyConstraints.FreezeRotation;
                }
                StartCoroutine(IncreaseBodyDrag(body));
            }
        }
    }

    public void ResetDrag() {
        StopAllCoroutines();
        foreach(var b in buildings) {
            List<GameObject> children = new List<GameObject>();
            children = GetChildren(b);
            foreach(var c in children) {
                Rigidbody body;
                body = c.GetComponent<Rigidbody>();

                body.drag = originalDrag.x;
                body.angularDrag = originalDrag.y;
                body.mass = 2;
            }
        }
    }

    private List<GameObject> GetChildren(GameObject building) {
        List<GameObject> children = new List<GameObject>();
        for(int i = 0; i < building.transform.childCount; i++) {
            children.Add(building.transform.GetChild(i).gameObject);
        }   
        return children;
    }

    private IEnumerator IncreaseBodyDrag(Rigidbody body) {
        originalDrag.x = body.drag;
        originalDrag.y = body.angularDrag;

        while(true) {
            body.drag += Time.deltaTime * dragAmount;
            if(!constrainRotation) {
                body.angularDrag += Time.deltaTime * dragAmount;
            }
            yield return null;
        }
    }

    public void LerpToPosition() {
        StopAllCoroutines();
        foreach(var b in buildings) {
            List<GameObject> children = new List<GameObject>();
            children = GetChildren(b);
            foreach(var c in children) {
                Destroy(c.GetComponent<BoxCollider>());
                Destroy(c.GetComponent<Rigidbody>());
                StartCoroutine(LerpBackToPosition(c));
            }
        }
    }

    private IEnumerator LerpBackToPosition(GameObject body) {
        float value = 0;
        Vector3 inAirPosition = body.transform.localPosition;
        Quaternion inAirRotation = body.transform.localRotation;
        while(true) {
            body.transform.localPosition = 
                Vector3.Lerp(inAirPosition, originalPositions[body], value);
            body.transform.localRotation =
                Quaternion.Lerp(inAirRotation, Quaternion.identity, value);
            value += Time.deltaTime;
            yield return null;
        }
    }
}
