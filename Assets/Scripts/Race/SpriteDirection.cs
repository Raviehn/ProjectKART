using UnityEngine;
using System.Collections;

public class SpriteDirection : MonoBehaviour {
    
    Quaternion rotation;

    SpriteRenderer sr;

    void Start ()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

	void Update () {

        rotation = Camera.main.transform.rotation;
        sr.transform.rotation = rotation;
	}
}
