using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TubeSprite : MonoBehaviour {

    public float sizeFactor;
    public float shrinkDistance;

    public Transform driver;

    public List<Sprite> sprites;

    private int sprite;
    private int lastSprite;

    private float distance;
    private float scaleOrigin;

    private Vector3 scale;

    private SpriteRenderer sr;

    void Start () {

        sr = gameObject.GetComponentInChildren<SpriteRenderer>();
        sr.sprite = sprites[9];
        
        scaleOrigin = transform.localScale.x;
    }
	
	void Update () {

        distance = Vector3.Distance(driver.transform.position, transform.position);

        lastSprite = sprite;
        sprite = (int)(distance / shrinkDistance);

        if (sprite > 9)
            sprite = 9;

        sr.sprite = sprites[sprite];

       

        if (lastSprite != sprite)
        {
            if (distance / sizeFactor > scaleOrigin)
                scale = new Vector3(distance / sizeFactor, distance / sizeFactor, distance / sizeFactor);
            else
                scale = new Vector3(scaleOrigin, scaleOrigin, scaleOrigin);

            transform.localScale = scale;
        }

        
	}
}
