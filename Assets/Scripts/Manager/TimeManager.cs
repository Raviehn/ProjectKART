using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeManager : MonoBehaviour
{
    public SpriteRenderer minTens;
    public SpriteRenderer minUnits;
    public SpriteRenderer secTens;
    public SpriteRenderer secUnits;
    public SpriteRenderer mSecTens;
    public SpriteRenderer mSecUnits;

    public List<Sprite> digits;

    RaceManager rm;

    void Start()
    {
        rm = GameObject.FindGameObjectWithTag("RaceManager").GetComponent<RaceManager>();
    }

    void Update()
    {
        minTens.sprite = digits[rm.digitIndices[0]];
        minUnits.sprite = digits[rm.digitIndices[1]];
        secTens.sprite = digits[rm.digitIndices[2]];
        secUnits.sprite = digits[rm.digitIndices[3]];
        mSecTens.sprite = digits[rm.digitIndices[4]];
        mSecUnits.sprite = digits[rm.digitIndices[5]];

    }
}
