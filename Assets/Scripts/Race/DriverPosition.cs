using UnityEngine;
using System.Collections;

public class DriverPosition : MonoBehaviour
{
    public float height;

    public Transform driver;

    Vector3 position;

    Quaternion rotation;

    void Update()
    {
        position = driver.position;
        position.y = height;
        transform.position = position;

        rotation = driver.rotation;
        transform.rotation = rotation;
    }
}
