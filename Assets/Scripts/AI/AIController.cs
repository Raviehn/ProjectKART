using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AIController : MonoBehaviour
{
    int bufferLength = 5;
    int curbWeight = 4;

    [HideInInspector]
    public float steerAngleEmphasis;
    [HideInInspector]
    public float steerDistanceEmphasis;
    [HideInInspector]
    public float steeringValue;
    [HideInInspector]
    public float stopSteering;
    [HideInInspector]
    public float stopAcceleration;
    [HideInInspector]
    public float angleCurvePrepare;
    [HideInInspector]
    public float curvePreparingTimeFactor;
    [HideInInspector]
    public float curvePredictionRadiusClose;
    [HideInInspector]
    public float curvePredictionRadiusMid;
    [HideInInspector]
    public float curvePredictionRadiusFar;
    [HideInInspector]
    public float hysteresisValue;
    [HideInInspector]
    public float waypointSkipDist;
    [HideInInspector]
    public float obstacleRaycastDistance;
    [HideInInspector]
    public float obstacleBrakeDistance;
    [HideInInspector]
    public float raycastAngleMultiplicator;
    [HideInInspector]
    public float difficulty;

    float samplingRate = .1f;

    bool hysteresisActive;
    bool currentWaypointSet;

    int currentWaypoint;
    int bufferIndex;

    float time, time0 = .1f;
    float curvePreparingTime;
    float nextCurvePreparingTime;
    float angleDriverCheckpoint;
    float stopAccelerationHysteresis;
    float angleCurveClose;
    float angleCurveMid;
    float angleCurveFar;

    KartController kc;

    RaceManager rm;

    AIManager aim;

    Grid grid;

    float[] angleBuffer;

    Vector3[] path;

    void Start()
    {
        kc = GetComponent<KartController>();
        rm = GameObject.FindGameObjectWithTag("RaceManager").GetComponent<RaceManager>();
        aim = GameObject.FindGameObjectWithTag("AIManager").GetComponent<AIManager>();
        grid = kc.pathfinding.GetComponent<Grid>();

        angleBuffer = new float[bufferLength];
        difficulty = rm.difficulty;
    }

    void Update()
    {
        if (path != null)
        {
            // set starting waypoint for any position on the track
            if (!currentWaypointSet)
            {
                for (int i = path.Length - 1; i >= 0; i--)
                {
                    if (path[i].x > grid.GetNodeFromWorldPos(transform.position).worldPos.x - 4 && path[i].x < grid.GetNodeFromWorldPos(transform.position).worldPos.x + 4
                        && path[i].z > grid.GetNodeFromWorldPos(transform.position).worldPos.z - 4 && path[i].z < grid.GetNodeFromWorldPos(transform.position).worldPos.z + 4)
                    {
                        currentWaypoint = i;
                        currentWaypointSet = true;
                        break;
                    }
                }
            }

            if (time <= 0f)
            {
                SetNextWaypoint();
                time += time0;
            }
            time -= Time.deltaTime;

            angleDriverCheckpoint = Vector3.Angle(path[currentWaypoint] - transform.position, transform.forward);

            // deprecated
            /*if (Vector3.Distance(transform.position, path[currentWaypoint]) < distToProceed)
                currentWaypoint++;

            if (currentWaypoint >= path.Length)
                currentWaypoint = 0;*/
        }
        else if (aim.pathReady)
            path = aim._path;

        if (curvePreparingTime >= 0f)
            curvePreparingTime -= Time.deltaTime;

        if (nextCurvePreparingTime >= 0f)
            nextCurvePreparingTime -= Time.deltaTime;
    }

    public void GetInput()
    {
        if (path != null)
        {
            bool obstacleLeft = false;
            bool obstacleRight = false;
            bool obstacleCenter = false;

            float obstacleDistance = float.MaxValue;

            RaycastHit hit;

            float raycastDirection = ((transform.eulerAngles.y + 90f - kc._steering * raycastAngleMultiplicator) * 2f * Mathf.PI) / 360f;

            // check for obstacles with three parallel raycasts
            if (Physics.Raycast(transform.position + new Vector3(kc._direction.z / 4f, 0f, -kc._direction.x / 4f), new Vector3(-Mathf.Cos(raycastDirection), 0f, Mathf.Sin(raycastDirection)), out hit, obstacleRaycastDistance, 2048))
            {
                obstacleRight = true;
                obstacleDistance = (transform.position - hit.transform.position).magnitude;
            }
            else if (Physics.Raycast(transform.position + new Vector3(-kc._direction.z / 4f, 0f, kc._direction.x / 4f), new Vector3(-Mathf.Cos(raycastDirection), 0f, Mathf.Sin(raycastDirection)), out hit, obstacleRaycastDistance, 2048))
            {
                obstacleLeft = true;
                obstacleDistance = (transform.position - hit.transform.position).magnitude;
            }
            else if (Physics.Raycast(transform.position, new Vector3(-Mathf.Cos(raycastDirection), 0f, Mathf.Sin(raycastDirection)), out hit, obstacleRaycastDistance, 2048))
            {
                obstacleCenter = true;
                obstacleDistance = (transform.position - hit.transform.position).magnitude;
            }
            
            if ((obstacleLeft || obstacleRight || obstacleCenter) && obstacleDistance < obstacleBrakeDistance && kc._speed > kc._maxSpeed / 2f)
                kc.throttle = KartController.Throttle.brake;
            else if (NeedToAccelerate())
                kc.throttle = KartController.Throttle.accelerate;
            else
                kc.throttle = KartController.Throttle.none;

            GetAngleNextCurve();

            if (Mathf.Abs(angleCurveMid) > angleCurvePrepare && curvePreparingTime <= 0f && nextCurvePreparingTime <= 0f && rm.raceStart)
            {
                curvePreparingTime = (Mathf.Abs(angleCurveMid) / angleCurvePrepare) * (Mathf.Abs(angleCurveMid) / angleCurvePrepare) * (kc._speed / kc._maxSpeed) * curvePreparingTimeFactor;
                nextCurvePreparingTime = 2f;
            }

            if ((obstacleLeft || obstacleRight || obstacleCenter))
            {
                //check if the left side of the obstacle is worse than curbs
                bool leftFree = grid.GetNodeFromWorldPos(hit.transform.position + new Vector3(-kc._direction.z / 2f, 0f, kc._direction.x / 2f)).weight <= curbWeight;
                bool rightFree = grid.GetNodeFromWorldPos(hit.transform.position + new Vector3(kc._direction.z / 2f, 0f, -kc._direction.x / 2f)).weight <= curbWeight;

                //if both sides are free, follow the waypoint
                if (leftFree && rightFree)
                {
                    if (angleDriverCheckpoint > 45f)
                    {
                        float targetDirection = Vector3.Dot(path[currentWaypoint] - transform.position, new Vector3(-transform.forward.z, 0f, transform.forward.x));

                        if (targetDirection < 0f)
                            kc.steer = KartController.Steer.right;
                        else
                            kc.steer = KartController.Steer.left;
                    }
                    else
                        kc.steer = obstacleLeft ? KartController.Steer.right : KartController.Steer.left;
                }
                else if (rightFree)
                    kc.steer = KartController.Steer.right;
                else
                    kc.steer = KartController.Steer.left;
            }
            // prepare for sharp curves
            else if (curvePreparingTime >= 0f)
            {
                if (angleCurveMid > 0f)
                    kc.steer = KartController.Steer.right;
                else
                    kc.steer = KartController.Steer.left;
            }
            // evaluate steering direction if need be
            else if (NeedToSteer())
            {
                float targetDirection = Vector3.Dot(path[currentWaypoint] - transform.position, new Vector3(-transform.forward.z, 0f, transform.forward.x));

                if (targetDirection < 0f)
                    kc.steer = KartController.Steer.right;
                else
                    kc.steer = KartController.Steer.left;
            }
            else
                kc.steer = KartController.Steer.none;
        }
    }

    bool RaycastSuccessfull(Vector3 target)
    {
        bool result = true;

        float x;
        float z;

        for (float r = 0; r < 1f; r += samplingRate)
        {
            x = transform.position.x + (r * (target.x - transform.position.x));
            z = transform.position.z + (r * (target.z - transform.position.z));

            //check, if it intersects with an obstacle
            if (grid.GetNodeFromWorldPos(new Vector3(x, 0f, z)).weight > curbWeight)
            {
                result = false;
                break;
            }
        }

        return result;
    }

    void SetNextWaypoint()
    {
        for (int i = currentWaypoint; i < path.Length; i++)
        {
            if (RaycastSuccessfull(path[i]) && Vector3.Distance(transform.position, path[i]) < waypointSkipDist)
            {
                currentWaypoint = i;
                if (i == path.Length - 1)
                    i = 0;
            }
            else
                break;
        }
    }

    bool NeedToSteer()
    {
        bool result = false;
        float averageDelta = 0f;

        // if ai is off the road, steer
        if (grid.GetNodeFromWorldPos(transform.position).weight > curbWeight)
            return true;

        float value = (angleDriverCheckpoint * steerAngleEmphasis + (1f / Vector3.Distance(transform.position, path[currentWaypoint]) * steerDistanceEmphasis)) * (kc._speed / kc._maxSpeed);

        if (value > steeringValue)
            result = true;

        angleBuffer[bufferIndex] = angleDriverCheckpoint;

        for (int j = bufferIndex + 1; j < bufferIndex + bufferLength; j++)
        {
            int index = j >= bufferLength ? j - bufferLength : j;
            int prevIndex = index == 0 ? bufferLength - 1 : index - 1;
            averageDelta += angleBuffer[index] - angleBuffer[prevIndex];
        }

        bufferIndex++;
        if (bufferIndex >= bufferLength)
            bufferIndex = 0;

        averageDelta /= bufferLength - 1;

        float prediction = angleDriverCheckpoint + averageDelta * kc._steeringInertia;

        if (result)
        {
            // if next curve goes left
            if (angleCurveFar >= 45f)
            {
                // if we are steering right
                if (kc._steering < 0f)
                    if (prediction < -value)
                        result = false;
                else
                    if (prediction < value)
                        result = false;
            }
            // if next curve goes right
            else if (angleCurveFar <= -45f)
            {
                // if we are steering right
                if (kc._steering < 0f)
                    if (prediction < value)
                        result = false;
                else
                    if (prediction < -value)
                        result = false;
            }
            else if (Mathf.Abs(prediction) < stopSteering)
                result = false;
        }

        return result;
    }

    bool NeedToAccelerate()
    {
        bool result = false;

        // value to set a specific difficulty
        if (rm.raceStart && kc._speed <= kc._maxSpeed * (difficulty * .14f + .86f))
            result = true;

        if (kc._driftEnabled && NeedToSteer())
            result = false;

        float value = angleCurveClose * (kc._speed / kc._maxSpeed);

        // hysteresis
        if (value > stopAcceleration && !hysteresisActive)
        {
            result = false;
            hysteresisActive = true;
            stopAccelerationHysteresis = (hysteresisValue * (1f - angleCurveClose / 180f)) * kc._maxSpeed;
        }
        else if (kc._speed > stopAccelerationHysteresis && hysteresisActive)
            result = false;
        else
            hysteresisActive = false;

        return result;
    }

    void GetAngleNextCurve()
    {
        angleCurveClose = 0f;
        angleCurveMid = 0f;
        angleCurveFar = 0f;

        Vector3 currentVector = currentWaypoint == 0 ? path[path.Length - 1] - path[currentWaypoint] : path[currentWaypoint - 1] - path[currentWaypoint];

        for (int i = currentWaypoint; i <= currentWaypoint + curvePredictionRadiusFar; i += 2)
        {
            int index = i >= path.Length ? i - path.Length : i;
            int nextIndex = i >= path.Length - 2 ? i - path.Length + 2 : i + 2;

            float waypointDirection = Vector3.Dot(path[index] - path[nextIndex], new Vector3(-currentVector.z, 0f, currentVector.x));

            if (waypointDirection > 0f)
                angleCurveFar += Vector3.Angle(currentVector, path[index] - path[nextIndex]);
            else
                angleCurveFar -= Vector3.Angle(currentVector, path[index] - path[nextIndex]);

            if (i <= currentWaypoint + curvePredictionRadiusClose)
                angleCurveClose = angleCurveFar;
            if (i <= currentWaypoint + curvePredictionRadiusMid)
                angleCurveMid = angleCurveFar;

            currentVector = path[index] - path[nextIndex];
        }
    }

    /*void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = 0; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawWireCube(path[i], Vector3.one * .2f);
            }

            Gizmos.color = Color.red;

            if (kc.driver == 5)
                Gizmos.DrawCube(path[currentWaypoint], Vector3.one * .4f);
        }
    }*/
}
