using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DustController : MonoBehaviour {

    public float speedDust;

    public Transform driver;

    public List<Sprite> sprites;
    public List<Sprite> spritesTurning;
    public List<Sprite> spritesSpinning;

    int spriteIndex;

    float time, deltaTime = .02f;
    float time0, deltaTime0 = .075f;

    Vector3 position;

    SpriteRenderer sr;

    KartController kc;

    void Start () {

        sr = gameObject.GetComponentInChildren<SpriteRenderer>();
        sr.sprite = sprites[0];
        sr.enabled = false;

        kc = driver.GetComponent<KartController>();
    }

	void Update () {

        position = kc._spritePosition;
        position.y = -.1f;
        transform.position = position;

        // turning dust
        if(kc._turning)
        {
            sr.enabled = true;
            sr.sprite = kc._offroad ? spritesTurning[spriteIndex + 14] : spritesTurning[spriteIndex];

            if (time0 <= 0f)
                spriteIndex++;

            if (kc._steering < 0)
            {

                if (spriteIndex > 6 || spriteIndex < 0)
                    spriteIndex = 0;
            }
            else
            {
                if (spriteIndex > 13 || spriteIndex < 7)
                    spriteIndex = 7;
            }
        }
        // offroad dust
        else if (kc._offroad && kc._speed > speedDust && !kc._jump)
        {
            
            sr.enabled = true;
            sr.sprite = sprites[spriteIndex];

            if (time <= 0f)
                spriteIndex++;
           
            // max clipping
            if (kc._steering < -.35f) {

                if (spriteIndex > 8 || spriteIndex < 4)
                    spriteIndex = 4;
                    

            } else if(kc._steering > .35f)
            {
                if (spriteIndex > 13 || spriteIndex < 9)
                    spriteIndex = 9;
            } else
            {
                if (spriteIndex > 3)
                    spriteIndex = 0;
            }            
        }

        // onroad dust
        else if( !kc._offroad && kc._drifting && !kc._jump && Mathf.Abs(kc._steering) >= kc._maxSteering * .3f)
        {
            sr.enabled = true;
            sr.sprite = sprites[spriteIndex];

            if (time <= 0f)
                spriteIndex++;

            

            if (kc._steering < 0)
            {

                if (spriteIndex > 17 || spriteIndex < 14)
                    spriteIndex = 14;
            }
            else
            {
                if (spriteIndex > 21 || spriteIndex < 18)
                    spriteIndex = 18;
            }
        }

        // spinning dust
        else if(kc._spinning && !kc._jump)
        {
            sr.enabled = true;
            sr.sprite = spritesSpinning[spriteIndex];

            if (time <= 0f)
                spriteIndex++;

            if (spriteIndex >= spritesSpinning.Count - 1)
                spriteIndex = 0;
            

        }
        else
            sr.enabled = false;

        if (time <= 0f)
            time += deltaTime;
        if (time0 <= 0f)
            time0 += deltaTime0;

        time -= Time.deltaTime;
        time0 -= Time.deltaTime;

    }
}
