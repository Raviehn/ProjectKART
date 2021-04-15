using UnityEngine;
using System.Collections;

public class RaceBackground : MonoBehaviour
{
    public bool isChild;

    public float offset;
    
    public Transform driver;

    private Vector3 position;

    private SpriteRenderer sr;

    void Start()
    {

        sr = gameObject.GetComponentInChildren<SpriteRenderer>();

    }

    void Update()
    {
        position = transform.position;

        if( !isChild )
            position.x = -driver.eulerAngles.y / 360f * sr.bounds.size.x + offset;
        else
            position.x = -driver.eulerAngles.y / 360f * sr.bounds.size.x + sr.bounds.size.x + offset;

        transform.position = position;
    }
}

/*using UnityEngine;
using System.Collections;

public class RaceBackground : MonoBehaviour
{
    public bool isChild;

    public float distance;
    public float height;
    public float angle;
    public float offset;

    public Transform driver;

    private float spriteOffset;

    private Vector3 position;

    private Quaternion rotation;

    private SpriteRenderer sr;

    void Start()
    {

        sr = gameObject.GetComponentInChildren<SpriteRenderer>();
        spriteOffset = sr.bounds.size.x;

    }

    void Update()
    {
        position = driver.position;

        position.y = height;

        if (isChild)
        {
            position.x += distance * -Mathf.Sin(driver.eulerAngles.y / 360f * 2f * Mathf.PI) + ((driver.eulerAngles.y / 360f) - 1f + offset) * spriteOffset * -Mathf.Cos(driver.eulerAngles.y / 360f * 2f * Mathf.PI);
            position.z += distance * -Mathf.Cos(driver.eulerAngles.y / 360f * 2f * Mathf.PI) + ((driver.eulerAngles.y / 360f) - 1f + offset) * spriteOffset * Mathf.Sin(driver.eulerAngles.y / 360f * 2f * Mathf.PI);
        }
        else
        {
            position.x += distance * -Mathf.Sin(driver.eulerAngles.y / 360f * 2f * Mathf.PI) + (driver.eulerAngles.y / 360f + offset) * spriteOffset * -Mathf.Cos(driver.eulerAngles.y / 360f * 2f * Mathf.PI);
            position.z += distance * -Mathf.Cos(driver.eulerAngles.y / 360f * 2f * Mathf.PI) + (driver.eulerAngles.y / 360f + offset) * spriteOffset * Mathf.Sin(driver.eulerAngles.y / 360f * 2f * Mathf.PI);
        }


        rotation = driver.rotation;
        transform.rotation = rotation;

        transform.position = position;
    }
}
*/