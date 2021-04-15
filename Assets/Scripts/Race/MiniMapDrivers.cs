using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniMapDrivers : MonoBehaviour
{

    public Camera cam;

    public List<Sprite> sprites;

    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {

        transform.LookAt(cam.transform);

        int index = Mathf.RoundToInt(transform.parent.eulerAngles.y / 360f * sprites.Count);

        if (index < 0)
            index = 0;
        else if (index >= sprites.Count)
            index = sprites.Count - 1;

        sr.sprite = sprites[index];
    }
}
