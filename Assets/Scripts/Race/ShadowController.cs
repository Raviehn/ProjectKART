using UnityEngine;
using System.Collections;

public class ShadowController : MonoBehaviour
{
    public Transform driver;

    Vector3 position;
    
    KartController kc;

    SpriteRenderer sr;

    void Start()
    {
        kc = driver.GetComponent<KartController>();

        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.enabled = false;
    }

    void Update()
    {
        position = driver.position;
        position.y = .03f;
        transform.position = position;

        transform.rotation = driver.rotation;
        
        if (kc._jump)
        {
            if (Time.frameCount % 2 == 0)
                sr.enabled = true;
            else
                sr.enabled = false;
        }
        else
            sr.enabled = false;
    }
}
