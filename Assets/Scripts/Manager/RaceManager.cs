using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    [HideInInspector]
    public bool raceStart = false;
    [HideInInspector]
    public bool isQualification;

    public bool testDrive;
    public int testDriver;

    public float startTime;
    [HideInInspector]
    public float displayTime;
    public float difficulty;

    [HideInInspector]
    public int[] digitIndices;

    bool bigRankDigit;
    bool eventEnd;

    int lastRank;
    int maxPlayers = 8;
    int qualiLaps = 3;
    int laps;


    [HideInInspector]
    public int player1Driver;
    [HideInInspector]
    public int player2Driver;

    float time;
    float lapTimesTime;
    float rankTimeBigDigit;

    int[] lapsPassed;
    float[] distance;
    Vector3[] lastPosition;
    int[] rank;
    float[] finishLineTime;
    float[,] lapTimes;
    float[] fastestLap;
    int[,,] lapTimesResults;
    KartController[] driver;
    List<Transform> checkpoints;
    bool[] availableDrivers;
    bool[] qualiStart;

    public List<Sprite> rankSprites;

    List<int> qualiRank;

    GameObject lapTimesTransform;

    Text lapTimesText;

    SpriteRenderer srRank;

    SoundManager sm;

    List<float> values0 = new List<float>();
    List<float> values1 = new List<float>();
    List<float> values2 = new List<float>();
    List<float> values3 = new List<float>();

    List<float> sortedQualiTimes = new List<float>();

    List<List<float>> values;

    void Awake()
    {
        isQualification = PlayerPrefs.GetInt("Quali") == 1 ? true : false;
        //laps = isQualification ? qualiLaps : PlayerPrefs.GetInt("Laps");
        laps = isQualification ? qualiLaps : 999;

        difficulty = PlayerPrefs.GetFloat("Difficulty");

        availableDrivers = new bool[maxPlayers];

        for (int i = 0; i < availableDrivers.Length; i++)
            availableDrivers[i] = true;

        availableDrivers[PlayerPrefs.GetInt("Player1")] = false;
        player1Driver = testDrive ? testDriver : PlayerPrefs.GetInt("Player1");

        if (PlayerPrefs.GetInt("Player2") != -1)
        {
            availableDrivers[PlayerPrefs.GetInt("Player2")] = false;
            player2Driver = PlayerPrefs.GetInt("Player2");
        }

        Application.targetFrameRate = -1;

        raceStart = testDrive ? true : false;

        digitIndices = new int[6];

        for (int i = 0; i < digitIndices.Length; i++)
            digitIndices[i] = 0;

        lapsPassed = new int[maxPlayers];
        distance = new float[maxPlayers];
        lastPosition = new Vector3[maxPlayers];
        rank = new int[maxPlayers];
        finishLineTime = new float[maxPlayers];
        driver = new KartController[maxPlayers];
        checkpoints = new List<Transform>();
        lapTimes = new float[maxPlayers, laps];
        fastestLap = new float[maxPlayers];
        lapTimesResults = new int[maxPlayers, laps, 5];
        qualiStart = new bool[maxPlayers];

        qualiRank = new List<int>();

        for (int i = 0; i < fastestLap.Length; i++)
            fastestLap[i] = float.MaxValue;

        // get checkpoints
        foreach (Transform t in GameObject.FindGameObjectWithTag("Checkpoints").transform)
            checkpoints.Add(t);

        SetUpDictionary();

        values = new List<List<float>>();
        values.Add(values0);
        values.Add(values1);
        values.Add(values2);
        values.Add(values3);
        
        srRank = GameObject.FindGameObjectWithTag("Position").GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (!testDrive)
        {
            // get all drivers
            foreach (Transform t in GameObject.FindGameObjectWithTag("Drivers").transform)
                driver[t.GetChild(0).GetComponent<KartController>().driver] = t.GetChild(0).GetComponent<KartController>();

            // set starting positions for qualification mode
            if (isQualification)
            {
                srRank.enabled = false;
                Transform qualiPositions = GameObject.FindGameObjectWithTag("QualiPositions").transform;

                for (int i = 0; i < PlayerPrefs.GetInt("Players"); i++)
                {
                    driver[i].transform.parent.transform.position = qualiPositions.GetChild(i).transform.position;
                    driver[i].transform.rotation = qualiPositions.GetChild(i).transform.rotation;
                }
            }
            else
            {
                Transform racePositions = GameObject.FindGameObjectWithTag("RacePositions").transform;

                driver[PlayerPrefs.GetInt("StartPos0")].transform.parent.transform.position = racePositions.GetChild(0).transform.position;
                driver[PlayerPrefs.GetInt("StartPos0")].transform.rotation = racePositions.GetChild(0).transform.rotation;
                driver[PlayerPrefs.GetInt("StartPos1")].transform.parent.transform.position = racePositions.GetChild(1).transform.position;
                driver[PlayerPrefs.GetInt("StartPos1")].transform.rotation = racePositions.GetChild(1).transform.rotation;
                driver[PlayerPrefs.GetInt("StartPos2")].transform.parent.transform.position = racePositions.GetChild(2).transform.position;
                driver[PlayerPrefs.GetInt("StartPos2")].transform.rotation = racePositions.GetChild(2).transform.rotation;
                driver[PlayerPrefs.GetInt("StartPos3")].transform.parent.transform.position = racePositions.GetChild(3).transform.position;
                driver[PlayerPrefs.GetInt("StartPos3")].transform.rotation = racePositions.GetChild(3).transform.rotation;
                driver[PlayerPrefs.GetInt("StartPos4")].transform.parent.transform.position = racePositions.GetChild(4).transform.position;
                driver[PlayerPrefs.GetInt("StartPos4")].transform.rotation = racePositions.GetChild(4).transform.rotation;
                driver[PlayerPrefs.GetInt("StartPos5")].transform.parent.transform.position = racePositions.GetChild(5).transform.position;
                driver[PlayerPrefs.GetInt("StartPos5")].transform.rotation = racePositions.GetChild(5).transform.rotation;
                driver[PlayerPrefs.GetInt("StartPos6")].transform.parent.transform.position = racePositions.GetChild(6).transform.position;
                driver[PlayerPrefs.GetInt("StartPos6")].transform.rotation = racePositions.GetChild(6).transform.rotation;
                driver[PlayerPrefs.GetInt("StartPos7")].transform.parent.transform.position = racePositions.GetChild(7).transform.position;
                driver[PlayerPrefs.GetInt("StartPos7")].transform.rotation = racePositions.GetChild(7).transform.rotation;
            }

            // set start distances
            for (int i = 0; i < distance.Length; i++)
            {
                if (driver[i] != null)
                {
                    distance[i] -= (driver[i].transform.position - checkpoints[0].transform.position).magnitude;
                    lastPosition[i] = driver[i].transform.position;
                }
            }
        }

        lapTimesTransform = GameObject.FindGameObjectWithTag("LapTimes");
        lapTimesText = lapTimesTransform.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>();
        lapTimesTransform.SetActive(false);

        sm = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }

    void SetUpDictionary()
    {
        // values for Mario and Luigi

        //acceleration
        values0.Add(.0022f);
        //startAcceleration
        values0.Add(.022f);
        //deceleration
        values0.Add(1.66f);
        //maxSpeed
        values0.Add(8.3f);
        //maxDriftValue
        values0.Add(20f);
        //fullspeedTimeUntilDrift
        values0.Add(4f);
        //fullspeedHysteresis
        values0.Add(.5f);
        //maxSteering
        values0.Add(.62f);
        //steeringInertia
        values0.Add(2.4f);
        //startSteering
        values0.Add(.04f);
        //friction
        values0.Add(2.5f);
        //startSpinningValue
        values0.Add(1.5f);
        //turboStartMax
        values0.Add(3f);
        //turboStartMin
        values0.Add(2.8f);

        //steerAngleEmphasis
        values0.Add(.06f);
        //steeringValue
        values0.Add(.6f);
        //stopSteering
        values0.Add(9f);
        //stopAcceleration
        values0.Add(65f);
        //curvePredictionRadiusClose
        values0.Add(10f);
        //curvePredictionRadiusMid
        values0.Add(20f);
        //curvePredictionRadiusFar
        values0.Add(30f);
        //angleCurvePrepare
        values0.Add(90f);
        //curvePreparingTimeFactor
        values0.Add(.06f);
        //hysteresisValue
        values0.Add(1.7f);
        //waypointSkipDist
        values0.Add(6f);
        //obstacleRaycastDistance
        values0.Add(6f);
        //obstacleBrakeDistance
        values0.Add(2f);
        //raycastAngleMultiplicator
        values0.Add(50f);

        // values for Princess and Yoshi

        //acceleration
        values1.Add(.003f);
        //startAcceleration
        values1.Add(.03f);
        //deceleration
        values1.Add(1.66f);
        //maxSpeed
        values1.Add(8f);
        //maxDriftValue
        values1.Add(40f);
        //fullspeedTimeUntilDrift
        values1.Add(4f);
        //fullspeedHysteresis
        values1.Add(.5f);
        //maxSteering
        values1.Add(.61f);
        //steeringInertia
        values1.Add(2.4f);
        //startSteering
        values1.Add(.04f);
        //friction
        values1.Add(2.5f);
        //startSpinningValue
        values1.Add(1.5f);
        //turboStartMax
        values1.Add(3f);
        //turboStartMin
        values1.Add(2.8f);

        //steerAngleEmphasis
        values1.Add(.07f);
        //steeringValue
        values1.Add(.6f);
        //stopSteering
        values1.Add(17.5f);
        //stopAcceleration
        values1.Add(100f);
        //curveredictionRadiusClose
        values1.Add(10f);
        //curveredictionRadiusMid
        values1.Add(20f);
        //curveredictionRadiusFar
        values1.Add(30f);
        //angleCurvePrepare
        values1.Add(75f);
        //curvePreparingTimeFactor
        values1.Add(.08f);
        //hysteresisValue
        values1.Add(1.5f);
        //waypointSkipDist
        values1.Add(5f);
        //obstacleRaycastDistance
        values1.Add(7f);
        //obstacleBrakeDistance
        values1.Add(0f);
        //raycastAngleMultiplicator
        values1.Add(20f);

        // balues for Bowser and Donkey Kong Jr.

        //acceleration
        values2.Add(.002f);
        //startAcceleration
        values2.Add(.02f);
        //deceleration
        values2.Add(1.66f);
        //maxSpeed
        values2.Add(8.4f);
        //maxDriftValue
        values2.Add(40f);
        //fullspeedTimeUntilDrift
        values2.Add(4f);
        //fullspeedHysteresis
        values2.Add(.5f);
        //maxSteering
        values2.Add(.6f);
        //steeringInertia
        values2.Add(2.4f);
        //startSteering
        values2.Add(.04f);
        //friction
        values2.Add(2.5f);
        //startSpinningValue
        values2.Add(1.5f);
        //turboStartMax
        values2.Add(3f);
        //turboStartMin
        values2.Add(2.8f);

        //steerAngleEmphasis
        values2.Add(.02f);
        //steeringValue
        values2.Add(.4f);
        //stopSteering
        values2.Add(21f);
        //stopAcceleration
        values2.Add(85f);
        //curvePredictionRadiusClose
        values2.Add(10f);
        //curvePredictionRadiusMid
        values2.Add(20f);
        //curvePredictionRadiusFar
        values2.Add(30f);
        //angleCurvePrepare
        values2.Add(95f);
        //curvePreparingTimeFactor
        values2.Add(.05f);
        //hysteresisValue
        values2.Add(1.8f);
        //waypointSkipDist
        values2.Add(6.5f);
        //obstacleRaycastDistance
        values2.Add(5f);
        //obstacleBrakeDistance
        values2.Add(2f);
        //raycastAngleMultiplicator
        values2.Add(50f);

        // values for Koopa Troopa and Toad

        //acceleration
        values3.Add(.0023f);
        //startAcceleration
        values3.Add(.023f);
        //deceleration
        values3.Add(1.66f);
        //maxSpeed
        values3.Add(8f);
        //maxDriftValue
        values3.Add(40f);
        //fullspeedTimeUntilDrift
        values3.Add(4f);
        //fullspeedHysteresis
        values3.Add(.5f);
        //maxSteering
        values3.Add(.61f);
        //steeringInertia
        values3.Add(2.4f);
        //startSteering
        values3.Add(.04f);
        //friction
        values3.Add(2.5f);
        //startSpinningValue
        values3.Add(1.5f);
        //turboStartMax
        values3.Add(3f);
        //turboStartMin
        values3.Add(2.8f);

        //steerAngleEmphasis
        values3.Add(.02f);
        //steeringValue
        values3.Add(.3f);
        //stopSteering
        values3.Add(17f);
        //stopAcceleration
        values3.Add(120f);
        //curvePredictionRadiusClose
        values3.Add(10f);
        //curvePredictionRadiusMid
        values3.Add(20f);
        //curvePredictionRadiusFar
        values3.Add(30f);
        //angleCurvePrepare
        values3.Add(75f);
        //curvePreparingTimeFactor
        values3.Add(.06f);
        //hysteresisValue
        values3.Add(1.2f);
        //waypointSkipDist
        values3.Add(5f);
        //obstacleRaycastDistance
        values3.Add(3f);
        //obstacleBrakeDistance
        values3.Add(1f);
        //raycastAngleMultiplicator
        values3.Add(50f);
    }

    public void SetUpKartValues(KartController kc)
    {
        int driver = kc.driver < 4 ? kc.driver : kc.driver - 4;

        kc.acceleration = values[driver][0];
        kc.startAcceleration = values[driver][1];
        kc.deceleration = values[driver][2];
        kc.maxSpeed = values[driver][3];
        kc.maxDriftValue = values[driver][4];
        kc.fullspeedTimeUntilDrift = values[driver][5];
        kc.fullspeedHysteresis = values[driver][6];
        kc.maxSteering = values[driver][7];
        kc.steeringInertia = values[driver][8];
        kc.startSteering = values[driver][9];
        kc.friction = values[driver][10];
        kc.startSpinningValue = values[driver][11];
        kc.turboStartMax = values[driver][12];
        kc.turboStartMin = values[driver][13];
        
        kc.aic.steerAngleEmphasis = values[driver][14];
        kc.aic.steeringValue = values[driver][15];
        kc.aic.stopSteering = values[driver][16];
        kc.aic.stopAcceleration = values[driver][17];
        kc.aic.curvePredictionRadiusClose = values[driver][18];
        kc.aic.curvePredictionRadiusMid = values[driver][19];
        kc.aic.curvePredictionRadiusFar = values[driver][20];
        kc.aic.angleCurvePrepare = values[driver][21];
        kc.aic.curvePreparingTimeFactor = values[driver][22];
        kc.aic.hysteresisValue = values[driver][23];
        kc.aic.waypointSkipDist = values[driver][24];
        kc.aic.obstacleRaycastDistance = values[driver][25];
        kc.aic.obstacleBrakeDistance = values[driver][26];
        kc.aic.raycastAngleMultiplicator = values[driver][27];
    }

    void Update()
    {
        if (!testDrive)
        {
            if (time > startTime && !raceStart)
            {
                raceStart = true;

                if (!isQualification)
                    for (int i = 0; i < finishLineTime.Length; i++)
                        finishLineTime[i] = time;
            }

            if ((isQualification && qualiStart[player1Driver] || !isQualification && raceStart) && !driver[player1Driver].raceFinished)
            {
                displayTime += Time.deltaTime;
                ManageTime();
            }

            for (int i = 0; i < lapsPassed.Length && lapsPassed[i] >= laps; i++)
                if (i == lapsPassed.Length - 1)
                    eventEnd = true;

            if (eventEnd)
            {
                lapTimesTransform.SetActive(false);

                qualiRank = qualiRank.Distinct().ToList();

                PlayerPrefs.SetInt("StartPos0", qualiRank[0]);
                PlayerPrefs.SetInt("StartPos1", qualiRank[1]);
                PlayerPrefs.SetInt("StartPos2", qualiRank[2]);
                PlayerPrefs.SetInt("StartPos3", qualiRank[3]);
                PlayerPrefs.SetInt("StartPos4", qualiRank[4]);
                PlayerPrefs.SetInt("StartPos5", qualiRank[5]);
                PlayerPrefs.SetInt("StartPos6", qualiRank[6]);
                PlayerPrefs.SetInt("StartPos7", qualiRank[7]);

                sortedQualiTimes.Sort();
                float fastestQualiLap = sortedQualiTimes[0];
                float slowestTimeOverall = sortedQualiTimes[sortedQualiTimes.Count - 4];

                if (fastestLap[player1Driver] <= fastestQualiLap)
                    difficulty = 1f;
                else if (fastestLap[player1Driver] > slowestTimeOverall)
                    difficulty = 0f;
                else
                    difficulty = 1f - ((fastestLap[player1Driver] - fastestQualiLap) / (slowestTimeOverall - fastestQualiLap));

                PlayerPrefs.SetFloat("Difficulty", difficulty);
                Debug.Log("Difficulty for this race: " + difficulty);

                if (isQualification)
                    PlayerPrefs.SetInt("Quali", 0);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            // display lap times
            if (driver[player1Driver].raceFinished)
            {
                lapTimesText.text = "L1  " + lapTimesResults[player1Driver, 0, 0] + "' " + lapTimesResults[player1Driver, 0, 1] + "" + lapTimesResults[player1Driver, 0, 2] + "\"" + lapTimesResults[player1Driver, 0, 3] + "" + lapTimesResults[player1Driver, 0, 4] +
                    "\nL2  " + lapTimesResults[player1Driver, 1, 0] + "' " + lapTimesResults[player1Driver, 1, 1] + "" + lapTimesResults[player1Driver, 1, 2] + "\"" + lapTimesResults[player1Driver, 1, 3] + "" + lapTimesResults[player1Driver, 1, 4] +
                    "\nL3  " + lapTimesResults[player1Driver, 2, 0] + "' " + lapTimesResults[player1Driver, 2, 1] + "" + lapTimesResults[player1Driver, 2, 2] + "\"" + lapTimesResults[player1Driver, 2, 3] + "" + lapTimesResults[player1Driver, 2, 4];

                if (lapTimesTime >= 2f)
                    lapTimesTransform.SetActive(true);
                else
                    lapTimesTime += Time.deltaTime;
            }

            time += Time.deltaTime;
        }
    }

    void EvaluateResults(int finishedDriver)
    {
        for (int lap = 0; lap < laps; lap++)
        {
            // if lap time extends 10min, put 9 in every position
            if (lapTimes[finishedDriver, lap] >= 600f)
                for (int i = 0; i < 5; i++)
                    lapTimesResults[finishedDriver, lap, i] = 9;
            else
            {
                int seconds = Mathf.FloorToInt(lapTimes[finishedDriver, lap]);
                int milliseconds = Mathf.RoundToInt((lapTimes[finishedDriver, lap] - seconds) * 100f);

                lapTimesResults[finishedDriver, lap, 0] = Mathf.FloorToInt(seconds / 60f);
                lapTimesResults[finishedDriver, lap, 1] = Mathf.FloorToInt((seconds / 10f) % 6);
                lapTimesResults[finishedDriver, lap, 2] = Mathf.FloorToInt(seconds % 10);
                lapTimesResults[finishedDriver, lap, 3] = Mathf.FloorToInt((milliseconds / 10f) % 10);
                lapTimesResults[finishedDriver, lap, 4] = Mathf.FloorToInt(milliseconds % 10);
            }

        }
    }

    void EvaluateCurrentRank(int driver)
    {
        for (int i = 0; i < maxPlayers; i++)
        {
            if (i >= qualiRank.Count - 1 || fastestLap[qualiRank[i]] > fastestLap[driver])
            {
                qualiRank.Insert(i, driver);
                break;
            }
        }
    }

    void ManageTime()
    {
        int seconds = Mathf.FloorToInt(displayTime);
        int milliseconds = Mathf.RoundToInt((displayTime - seconds) * 100f);

        digitIndices[0] = Mathf.FloorToInt(seconds / 600f);
        digitIndices[1] = Mathf.FloorToInt((seconds % 600f ) / 60f);
        digitIndices[2] = Mathf.FloorToInt((seconds / 10f) % 6);
        digitIndices[3] = Mathf.FloorToInt(seconds % 10);
        digitIndices[4] = Mathf.FloorToInt((milliseconds / 10f) % 10);
        digitIndices[5] = Mathf.FloorToInt(milliseconds % 10);
    }

    void FixedUpdate()
    {
        if (!testDrive)
        {
            // evaluate distance
            for (int i = 0; i < driver.Length; i++)
            {
                if (driver[i] != null)
                {
                    distance[i] += (lastPosition[i] - driver[i].transform.position).magnitude;
                    lastPosition[i] = driver[i].transform.position;
                }
            }

            GetPositions();

            int currentRank = rank[PlayerPrefs.GetInt("Player1")];

            if (raceStart && currentRank != lastRank)
            {
                bigRankDigit = true;
                rankTimeBigDigit = .2f;
            }

            srRank.sprite = bigRankDigit ? srRank.sprite = rankSprites[currentRank + 7] : rankSprites[currentRank - 1];

            if (rankTimeBigDigit <= 0f)
                bigRankDigit = false;

            if (bigRankDigit)
                rankTimeBigDigit -= Time.deltaTime;

            lastRank = currentRank;
        }
    }

    void GetPositions()
    {
        for (int i = 0; i < maxPlayers; i++)
        {
            if (driver[i] != null)
            {
                int currentRank = 1;
                float driversDistance = distance[i] + lapsPassed[i] * 200f;

                for (int j = 0; j < distance.Length; j++)
                    if (i != j && driver[j] != null && driversDistance < distance[j] + lapsPassed[j] * 200f)
                        currentRank++;

                rank[i] = currentRank;
            }
        }
    }

    public void CrossFinishLine(int _driver)
    {
        if (!testDrive && !driver[_driver].raceFinished)
        {
            if (qualiStart[_driver] || !isQualification)
            {
                // reset time when round finished in quali mode
                if (isQualification && _driver == player1Driver)
                    displayTime = 0f;
                float lapTime = time - finishLineTime[_driver];
                lapTimes[_driver, lapsPassed[_driver]] = lapTime;

                lapsPassed[_driver]++;

                distance[_driver] = 0f;

                if (isQualification)
                {
                    EvaluateCurrentRank(_driver);
                    driver[_driver].aic.difficulty += .5f;

                    // set fastest lap
                    if (lapTime < fastestLap[_driver])
                        fastestLap[_driver] = lapTime;

                    // set overall slowest lap
                    if (_driver != player1Driver)
                        sortedQualiTimes.Add(lapTime);
                }

                if (lapsPassed[_driver] >= laps)
                {
                    if (_driver == player1Driver)
                        sm.PlayGoalSound();

                    driver[_driver].raceFinished = true;
                    driver[_driver].aic.difficulty = 0f;
                    EvaluateResults(_driver);
                }
            }

            finishLineTime[_driver] = time;
            qualiStart[_driver] = true;
        }
    }

    float GetFastestLapTime(int _driver)
    {
        float lapTime = float.MaxValue;

        if (driver[_driver] == null || lapsPassed[_driver] < 1)
            return 0f;

        for (int i = 0; i < lapsPassed[_driver]; i++)
        {
            if (lapTimes[_driver, i] < lapTime)
                lapTime = lapTimes[_driver, i];
        }

        return lapTime;
    }

    public int GetPlayer()
    {
        //int result = Random.Range(0, maxPlayers);

        //while (!availableDrivers[result])
        //    result = Random.Range(0, maxPlayers);

        int result = 0;

        for (int i = 0; i < maxPlayers; i++)
            if (availableDrivers[i] && i != player1Driver)
                result = i;

        availableDrivers[result] = false;

        return result;
    }

    public float GetLapTime(int driver)
    {
        if (lapsPassed[driver] > 0)
            return lapTimes[driver, lapsPassed[driver] - 1];
        else
            return 0f;
    }

    public bool _playerFinished
    {
        get
        {
            return driver[player1Driver].raceFinished;
        }
    }

    public int _laps
    {
        get
        {
            return laps;
        }
    }

    void OnGUI()
    {
        if (!isQualification)
            GUI.Label(new Rect(10, 0, 1000, 20), "Difficulty: " + difficulty.ToString());
        /*GUI.Label(new Rect(20, 20, 100, 20), "Donkey Kong Jr.");
        GUI.Label(new Rect(20, 40, 100, 20), "Pos: " + position[6].ToString());
        GUI.Label(new Rect(20, 60, 100, 20), "Lap: " + (lapsPassed[6] + 1).ToString());
        GUI.Label(new Rect(20, 80, 1000, 20), "FLT: " + GetFastestLapTime(6) + "s");

        GUI.Label(new Rect(20, 120, 100, 20), "Yoshi");
        GUI.Label(new Rect(20, 140, 100, 20), "Pos: " + position[5].ToString());
        GUI.Label(new Rect(20, 160, 100, 20), "Lap: " + (lapsPassed[5] + 1).ToString());
        GUI.Label(new Rect(20, 180, 1000, 20), "FLT: " + GetFastestLapTime(5) + "s");

        GUI.Label(new Rect(20, 220, 100, 20), "Mario");
        GUI.Label(new Rect(20, 240, 100, 20), "Pos: " + position[0].ToString());
        GUI.Label(new Rect(20, 260, 100, 20), "Lap: " + (lapsPassed[0] + 1).ToString());
        GUI.Label(new Rect(20, 280, 1000, 20), "FLT: " + GetFastestLapTime(0) + "s");

        GUI.Label(new Rect(20, 320, 100, 20), "Koopa Troopa");
        GUI.Label(new Rect(20, 340, 100, 20), "Pos: " + position[3].ToString());
        GUI.Label(new Rect(20, 360, 100, 20), "Lap: " + (lapsPassed[3] + 1).ToString());
        GUI.Label(new Rect(20, 380, 1000, 20), "FLT: " + GetFastestLapTime(3) + "s");

        GUI.Label(new Rect(20, 420, 100, 20), "Bowser (Player)");
        GUI.Label(new Rect(20, 440, 100, 20), "Pos: " + position[2].ToString());
        GUI.Label(new Rect(20, 460, 100, 20), "Lap: " + (lapsPassed[2] + 1).ToString());
        GUI.Label(new Rect(20, 480, 1000, 20), "FLT: " + GetFastestLapTime(2) + "s");


        // Draw FPS

        int fps = Mathf.RoundToInt(1f / Time.deltaTime / 5f);

        if (fps >= 100)
            fps = 99;

        for( int y = 10; y <= fps; y++)
            texture.SetPixel(frame, y, Color.red);
        texture.Apply();

        if (timeTex <= 0f)
            frame++;

        if(frame >= texture.width)
        {
            frame = 0;
            texture = new Texture2D(Screen.width - 20, 120, TextureFormat.ARGB32, false);
        }

        GUI.DrawTexture(new Rect(10, Screen.height - 130, texture.width, texture.height), texture);*/
    }
}
