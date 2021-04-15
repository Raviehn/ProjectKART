using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteManager : MonoBehaviour
{
    public Camera mainCamera;
    public Camera mapCamera;

    public List<Sprite> mario;
    public List<Sprite> peach;
    public List<Sprite> bowser;
    public List<Sprite> koopaTroopa;
    public List<Sprite> luigi;
    public List<Sprite> yoshi;
    public List<Sprite> donkeyKongJr;
    public List<Sprite> toad;

    public List<Sprite> marioMap;
    public List<Sprite> peachMap;
    public List<Sprite> bowserMap;
    public List<Sprite> koopaTroopaMap;
    public List<Sprite> luigiMap;
    public List<Sprite> yoshiMap;
    public List<Sprite> donkeyKongJrMap;
    public List<Sprite> toadMap;

    [HideInInspector]
    public List<Sprite>[] driverSprites;

    List<Sprite>[] driverSpritesMap;

    [HideInInspector]
    public List<Sprite>[] driverSpritesTurning;

    int spriteIndex;

    float time, deltaTime = .05f;
    float angleOffset;

    KartController[] kc;

    Vector3 position;

    void Awake()
    {
        driverSprites = new List<Sprite>[8];
        driverSpritesMap = new List<Sprite>[8];

        driverSprites[0] = mario;
        driverSprites[1] = peach;
        driverSprites[2] = bowser;
        driverSprites[3] = koopaTroopa;
        driverSprites[4] = luigi;
        driverSprites[5] = yoshi;
        driverSprites[6] = donkeyKongJr;
        driverSprites[7] = toad;

        driverSpritesMap[0] = marioMap;
        driverSpritesMap[1] = peachMap;
        driverSpritesMap[2] = bowserMap;
        driverSpritesMap[3] = koopaTroopaMap;
        driverSpritesMap[4] = luigiMap;
        driverSpritesMap[5] = yoshiMap;
        driverSpritesMap[6] = donkeyKongJrMap;
        driverSpritesMap[7] = toadMap;

        kc = transform.parent.gameObject.GetComponentsInChildren<KartController>();
    }

    void Update()
    {
        for (int i = 0; i < kc.Length; i++)
        {
            int driver = kc[i].driver;

            var renderer = kc[i].GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer sr in renderer)
            {
                if (sr.tag == "Sprites")
                {
                    position = sr.transform.position;
                    
                    if (!kc[i]._turning)
                    {
                        angleOffset = Vector3.Angle(kc[i].transform.position - new Vector3(mainCamera.transform.position.x, 0f, mainCamera.transform.position.z), kc[i].transform.forward);

                        float direction = Vector3.Dot(kc[i].transform.position - mainCamera.transform.position, new Vector3(-kc[i].transform.forward.z, 0f, kc[i].transform.forward.x));

                        if (direction < 0)
                            angleOffset *= -1;

                        spriteIndex = Mathf.RoundToInt(kc[i]._spriteIndex + angleOffset / 360f * (driverSprites[driver].Count));

                        if (spriteIndex > driverSprites[driver].Count - 1f)
                            spriteIndex = Mathf.RoundToInt(spriteIndex % driverSprites[driver].Count);
                        else if( spriteIndex < 0 )
                            spriteIndex = Mathf.RoundToInt(driverSprites[driver].Count + spriteIndex % driverSprites[driver].Count);

                        sr.sprite = driverSprites[driver][spriteIndex];
                        kc[i]._spriteIndexSet = false;

                        // horizontal shaking
                        if ((kc[i]._spinning || kc[i]._drifting) && !kc[i]._jump)
                        {
                            float kartAngleRad = ((kc[i].transform.eulerAngles.y + 90f) * 2f * Mathf.PI) / 360f;
                            float offset = Mathf.Sin((Time.time + Mathf.PI) * 50f) * Screen.width * .0000005f;

                            position.x = Mathf.Sin(kartAngleRad) * offset + kc[i].transform.localPosition.x;
                            position.z = Mathf.Cos(kartAngleRad) * offset + kc[i].transform.localPosition.z;                                                       
                        }
                        else
                        {
                            position.x = kc[i].transform.localPosition.x;
                            position.z = kc[i].transform.localPosition.z;
                        }

                        // vertical shaking
                        if (kc[i]._speed > kc[i]._maxSpeed * .13f && !kc[i]._jump)
                            position.y = Mathf.Sin(Time.time * 150f) > 0 ? Screen.height * .000008f : 0f;

                        position += kc[i].transform.parent.transform.position;
                        sr.transform.position = position;

                        kc[i]._spritePosition = position;
                    }
                    else
                    {
                        // prepare turning values
                        if (!kc[i]._spriteIndexSet)
                        {
                            kc[i]._spriteIndex = kc[i]._steering > 0 ? Mathf.RoundToInt((driverSprites[driver].Count - 1f) / 4f) : 3 * Mathf.RoundToInt((driverSprites[driver].Count - 1f) / 4f);
                            kc[i]._spriteIndexSet = true;
                        }

                        if (kc[i]._steering > 0 && time <= 0f)
                        {
                            kc[i]._spriteIndex--;

                            if (kc[i]._spriteIndex < 0)
                                kc[i]._spriteIndex = driverSprites[driver].Count - 1;
                        }
                        else if (kc[i]._steering < 0 && time <= 0f)
                        {
                            kc[i]._spriteIndex++;

                            if (kc[i]._spriteIndex > driverSprites[driver].Count - 1)
                                kc[i]._spriteIndex = 0;
                        }
                        if (kc[i]._spriteIndex == Mathf.RoundToInt((driverSprites[driver].Count - 1f) / 2f))
                        {
                            kc[i]._turning = false;
                            kc[i]._drifting = false;
                        }

                        sr.sprite = driverSprites[driver][kc[i]._spriteIndex];
                    }
                }
                else
                {        
                    // map sprites          
                    int index = Mathf.RoundToInt(sr.transform.parent.eulerAngles.y / 360f * (driverSpritesMap[driver].Count - 1f));
                    
                    if (index < 0)
                        index = 0;
                    else if (index > driverSpritesMap[driver].Count - 1)
                        index = driverSpritesMap[driver].Count - 1;

                    sr.sprite = driverSpritesMap[driver][index];
                    sr.transform.LookAt(mapCamera.transform);
                }
            }
        }

        if (time <= 0f)
            time += deltaTime;
        time -= Time.deltaTime;

    }
}