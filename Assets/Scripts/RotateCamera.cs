using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float rotationSpeed = 0.02f;
    [Range(-1, 1)]
    public int direction;

    private void Update() { transform.Rotate(direction * transform.InverseTransformDirection(Vector3.up) * rotationSpeed); }
}
