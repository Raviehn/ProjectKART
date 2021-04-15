using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitController : MonoBehaviour {

    public Transform driver;

    public List<Sprite> sprites;

    bool done = false;
    bool startIndexSet;

    int spriteIndex;
    int startIndex;

    float time, deltaTime = .05f;

    Vector3 position;

    SpriteRenderer sr;

    KartController kc;

    void Start()
    {
        sr = gameObject.GetComponentInChildren<SpriteRenderer>();
        sr.enabled = false;

        kc = driver.GetComponent<KartController>();
    }
    
    void Update () {

        position = driver.position;
        position.y = .02f;
        transform.position = position;

        if (kc._crashed)
        {
            if (!startIndexSet)
            {
                if (kc._crashTube)
                {
                    if (kc._tubeDirection > 0f)
                        startIndex = spriteIndex = 0;
                    else if (kc._tubeDirection < 0f)
                        startIndex = spriteIndex = 6;
                    else
                        startIndex = spriteIndex = 12;
                }
                else
                    startIndex = spriteIndex = kc._crashAngle < 90 && kc._crashAngle >= 0 ? 6 : 0;

                startIndexSet = true;
                sr.enabled = true;
            }

            sr.sprite = sprites[spriteIndex];

            if (time <= 0f)
                spriteIndex++;

            if (spriteIndex >= sprites.Count - 1)
                spriteIndex = sprites.Count - 1;

            if (spriteIndex >= startIndex + sprites.Count / 3 - 1)
            {
                done = true;
                sr.enabled = false;
            }
        }
        else if (!kc._crashed && done)
        {
            startIndexSet = false;
            done = false;
        }
        else
            sr.enabled = false;

        if (time <= 0f)
            time += deltaTime;
        time -= Time.deltaTime;

    }
}
