using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LakituController : MonoBehaviour
{
    public float verticalSpeed;
    public float horizontalSpeed;
    public float verticalTranslation;
    public float horizontalTranslation;

    public List<Sprite> spritesStart;
    public List<Sprite> spritesRound;

    bool done;
    bool showLap;

    int state;
    int sprite;
    int lap;

    float time;
    float lapTime;
    float speed;
    float translationSpeed = .5f;
    float targetHeight = .78f;

    Vector3 position;
    Vector3 positionOrigin;

    SpriteRenderer sr;

    void Start()
    {

        position = transform.localPosition;
        positionOrigin = position;
        speed = translationSpeed;
        state = 0;

        sr = GetComponent<SpriteRenderer>();

    }

    void Update()
    {




        if (showLap)
            ShowLap(lap);
        // choose sequence
        else if (position.y < targetHeight + .2f)
        {
            if (state == 0)
            {
                if (!done)
                {
                    time = 0;
                    done = true;
                }

                speed = Mathf.Lerp(translationSpeed, 0f, time * time * 2f);

                if (speed == 0f)
                {
                    state = 1;
                    done = false;
                }
            }
            else if (state == 1)
            {
                if (!done)
                {
                    time = 0;
                    done = true;
                }

                speed = 0f;

                if (time > .3f)
                {
                    state = 2;
                    done = false;
                }
            }
            else if (state == 2)
            {
                if (!done)
                {
                    time = 0;
                    done = true;
                }

                speed = -Mathf.Lerp(translationSpeed / 15f, 0f, time * time * time * 4f);

                if (speed == 0f && time >= 1.5f)
                {
                    state = 3;
                    done = false;
                }

            }
            else if (state == 3)
            {
                if (time >= 1.2f && sprite < spritesStart.Count - 1)
                {
                    sprite++;
                    sr.sprite = spritesStart[sprite];
                    time = 0;
                }

                if (sprite == spritesStart.Count - 1)
                    state = 4;
            }
            else if (state == 4)
            {
                if (!done)
                {
                    time = 0;
                    done = true;
                }

                speed = 0f;

                if (time > 1.2f)
                {
                    state = 5;
                    done = false;
                }
            }
            else if (state == 5)
            {
                if (!done)
                {
                    time = 0;
                    done = true;
                }

                speed = -Mathf.Lerp(0f, translationSpeed, time);
            }

            position.y -= speed * Time.deltaTime;
        }
        else if (position.y > positionOrigin.y)
        {
            state = -1;
            speed = 0f;
        }
        else
            position.y -= speed * Time.deltaTime;

        transform.localPosition = position;

        time += Time.deltaTime;
    }

    public void ShowLap(int lap)
    {
        if (!showLap)
        {
            showLap = true;
            this.lap = lap;
            sr.sprite = spritesRound[lap];
            lapTime = 0f;
        }

        position.y = positionOrigin.y - (Mathf.Sin(lapTime * verticalSpeed) * verticalTranslation);
        position.x = positionOrigin.x + (Mathf.Sin(lapTime * horizontalSpeed) * horizontalTranslation) - .4f;

        if (position.y > positionOrigin.y)
        {
            lapTime = 0f;
            showLap = false;
        }

        lapTime += Time.deltaTime;
    }
}
