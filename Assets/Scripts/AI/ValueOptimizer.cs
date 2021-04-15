using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ValueOptimizer : MonoBehaviour
{
    public int lapsToCheck;

    bool busy = true;

    int lap = -1;

    float lastLap;
    float fastestTime = float.MaxValue;
    float bestValue;

    List<float> times;

    float[] lapTimes;

    KartController kc;

    AIController aic;

    RaceManager rm;

    void Start()
    {
        kc = GetComponent<KartController>();
        aic = GetComponent<AIController>();
        rm = GameObject.FindGameObjectWithTag("RaceManager").GetComponent<RaceManager>();

        times = new List<float>();
        lapTimes = new float[lapsToCheck];
    }

    void Update()
    {
        if (lastLap != rm.GetLapTime(kc.driver))
        {
            lastLap = rm.GetLapTime(kc.driver);

            if (lap >= 0)
                lapTimes[lap] = rm.GetLapTime(kc.driver);
            lap++;
        }

        if (lap == lapsToCheck)
        {
            busy = false;
            lap = 0;
        }

        if (!busy)
            aic.obstacleRaycastDistance = GetBestValue(aic.obstacleRaycastDistance, 0f);
    }

    float GetBestValue(float value, float factor)
    {
        busy = true;

        float currentTime = GetTime(value);
        times.Add(currentTime);

        Debug.Log("Current time for driver " + kc.driver + ": " + currentTime + "  Fastest time: " + fastestTime + " with value: " + bestValue + ". Current value: " + value);

        if (currentTime < fastestTime)
        {
            fastestTime = currentTime;
            bestValue = value;
        }

        value += factor;

        return value;
    }

    float GetTime(float value)
    {
        float time = 0f;

        for (int i = 0; i < lapTimes.Length; i++)
            time += lapTimes[i];

        lapTimes = new float[lapsToCheck];

        return time /= lapTimes.Length;
    }

    public void ShowTimes()
    {
        for (int i = 0; i < times.Count; i++)
            Debug.Log(times[i]);
    }
}
