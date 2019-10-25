using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float speed;
    public CarMovementDirection MovementDirection {
        get {
            return direction;
        }
        set {
            direction = value;
            if(value == CarMovementDirection.North) {
                movementDirection = new Vector3(0, 0, 1);
            } else 
            if(value == CarMovementDirection.East) {
                movementDirection = new Vector3(1, 0, 0);
            } else 
            if(value == CarMovementDirection.South) {
                movementDirection = new Vector3(0, 0, -1);
            } else {
                movementDirection = new Vector3(-1, 0, 0);
            }
        }
    }

    private CarMovementDirection direction;
    private Vector3 movementDirection = Vector3.right;

    void Update()
    {
        transform.position += movementDirection * speed * Time.deltaTime;
    }
}
