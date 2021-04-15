using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Speedometer : MonoBehaviour
{
    public int bufferLength;

    public float avgSpeedWeight;
    public float clearBufferTolerance;
    
    float dist;
    float curSpeed;
    float avgSpeed;
    float wghtSpeed;

    int index;

    Vector3 lastPos;

    float[] speedBuffer;

    void Start()
    {
        speedBuffer = new float[bufferLength];
        lastPos = transform.position;
    }

    void Update()
    {
        dist = (transform.position - lastPos).magnitude;
        curSpeed = dist / Time.deltaTime;

        speedBuffer[index] = curSpeed;
        index++;

        float tempAvgSpeed = 0;

        for (int i = 0; i < bufferLength; i++)
        {
            tempAvgSpeed += speedBuffer[i];
        }

        avgSpeed = tempAvgSpeed / bufferLength;

        wghtSpeed = avgSpeed * avgSpeedWeight + curSpeed * ( 1f - avgSpeedWeight );

        if( avgSpeed < clearBufferTolerance )
            speedBuffer = new float[bufferLength];

        if (curSpeed < clearBufferTolerance)
            curSpeed = 0f;

        if (index >= bufferLength)
            index = 0;
        
        lastPos = transform.position;
    }

    public float _speed
    {
        get
        {
            return wghtSpeed;
        }
    }
}
