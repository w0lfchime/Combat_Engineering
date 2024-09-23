using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingLogo : MonoBehaviour
{
    public Transform transform;
    // Start is called before the first frame update
    void Start()
    {
        transform = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    public float rotationSpeed = 30f;  // Speed of rotation in degrees per second

    // Update is called once per frame
    void Update()
    {
        // Rotate around the Y-axis based on rotationSpeed
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}