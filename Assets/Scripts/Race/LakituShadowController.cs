using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LakituShadowController : MonoBehaviour {
    
    public float yPos;

    public List<Sprite> sprites;
    
    Vector3 position;
    
    SpriteRenderer sr;

	void Start () {

        sr = GetComponent<SpriteRenderer>();
	}
	
	void Update () {

        position = new Vector3(transform.parent.transform.localPosition.x, yPos, 1.7f);
        transform.position = position;

        if (Time.frameCount % 2 == 0)
            sr.enabled = true;
        else
            sr.enabled = false;

        float distance = transform.parent.transform.position.y - position.y;

        if (distance < .6f)
            sr.sprite = sprites[0];
        else if (distance < .8f)
            sr.sprite = sprites[1];
        else if (distance < 1f)
            sr.sprite = sprites[2];
        else
            sr.enabled = false;

        
    }
}
