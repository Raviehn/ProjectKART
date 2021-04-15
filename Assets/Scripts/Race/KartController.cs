using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KartController : MonoBehaviour
{
    public bool isPlayer;
    public bool observe;

    [HideInInspector]
    public bool raceFinished;

    [HideInInspector]
    public int driver;

    [HideInInspector]
    public float acceleration;
    [HideInInspector]
    public float startAcceleration;
    [HideInInspector]
    public float deceleration;
    [HideInInspector]
    public float maxSpeed;
    [HideInInspector]
    public float maxDriftValue;
    [HideInInspector]
    public float fullspeedTimeUntilDrift;
    [HideInInspector]
    public float fullspeedHysteresis;
    [HideInInspector]
    public float maxSteering;
    [HideInInspector]
    public float steeringInertia;
    [HideInInspector]
    public float startSteering;
    [HideInInspector]
    public float friction;
    [HideInInspector]
    public float startSpinningValue;
    [HideInInspector]
    public float turboStartMin;
    [HideInInspector]
    public float turboStartMax;

    [HideInInspector]
    public Transform pathfinding;

    [HideInInspector]
    public AIController aic;

    public enum Throttle { none, accelerate, brake }

    public Throttle throttle;

    public enum Steer { none, left, right }

    public Steer steer;

    public enum Action { none, jump, fire }

    public Action action;

    public AudioSource driftSound;
    public AudioSource jumpSound;
    public AudioSource boostSound;
    public AudioSource spinSound;

    public AudioSource[] engineSound;

    bool offroad;
    bool canDrift;
    bool driftEnabled;
    bool drifting;
    bool driftTimePassed;
    bool turning;
    bool jump;
    bool jumped;
    bool up;
    bool spinning;
    bool fullspeed;
    bool crashFrontal;
    bool crashLateral;
    bool crashTubeFrontal;
    bool crashTubeLateral;
    bool startDone;
    bool spriteIndexSet;

    int spriteIndex;
    int lapAntiCheatIndex;
    int kartType;
    int lap;

    float time;
    float throttleTime;
    float crashTimeWall;
    float crashTime;
    float driftTime;
    float fullspeedTime;
    float spriteTime, spriteChangeTime = .07f;
    float steering;
    float driftAngle;
    float force;
    float maxForce;
    float jumpHeight;
    float crashAngle;
    float spinningTime;
    float maxSteeringValue;
    float maxSteeringValueTemp;
    float hitDirection;
    float finishLineTime;

    Vector3 direction;
    Vector3 spritePosition;

    Speedometer speedometer;

    SpriteRenderer sr;

    RaceManager rm;

    SpriteManager sm;

    HumanController hc;

    LakituController lc;

    Grid grid;

    void Awake()
    {
        rm = GameObject.FindGameObjectWithTag("RaceManager").GetComponent<RaceManager>();
        
        lc = GameObject.FindGameObjectWithTag("LakituController").GetComponent<LakituController>();

        if (isPlayer)
        {
            driver = rm.player1Driver;
            hc = GetComponent<HumanController>();
        }
        else
        {
            pathfinding = GameObject.FindGameObjectWithTag("Pathfinding").transform;
            driver = rm.GetPlayer();
        }

        kartType = driver >= 4 ? driver - 4 : driver;
    }

    void Start()
    {

        aic = GetComponent<AIController>();

        rm.SetUpKartValues(this);

        grid = pathfinding.GetComponent<Grid>();

        engineSound[kartType].Play();

        if (!isPlayer)
            engineSound[kartType].volume = 0f;

        speedometer = gameObject.GetComponentInChildren<Speedometer>();
        sr = GetComponent<SpriteRenderer>();

        sm = GameObject.FindGameObjectWithTag("SpriteManager").GetComponent<SpriteManager>();

        spriteIndex = sm.driverSprites[driver].Count / 2;

        // koopa and toad cannot drift
        if (driver == 3 || driver == 7)
            canDrift = false;
        else
            canDrift = true;

        lapAntiCheatIndex = -1;
        if (!rm.isQualification)
            lap = 1;
    }

    void Update()
    {
        // check, if we are on the road
        offroad = grid.GetNodeFromWorldPos(transform.position).weight > 4 ? true : false;

        if (canDrift && throttle == Throttle.accelerate && ((fullspeed && (fullspeedTime > fullspeedTimeUntilDrift || driftTimePassed)) || (jump && _speed > maxSpeed / 2f)))
        {
            driftEnabled = true;
            driftTimePassed = true;
        }
        else
            driftEnabled = false;

        maxForce = offroad ? maxSpeed / 2.5f : maxSpeed;

        // input
        if (isPlayer && !raceFinished && !observe)
            hc.GetInput();
        else
            aic.GetInput();

        if (!turning)
            ApplyThrottle();
        ApplyFriction();
        ApplySteeringValue();
        EvaluateSprite();

        //Engine sound
        float pitch = offroad ? (force / maxForce / 2f) + 1f : (force / maxForce) + 1f;
        if (pitch > 2f)
            pitch = 2f;

        engineSound[kartType].pitch = pitch;

        // waiting for race to start
        if (!rm.raceStart)
        {
            if (throttle == Throttle.accelerate)
                throttleTime += Time.deltaTime;
            return;
        }
        else if (!startDone)
        {
            if (throttleTime > turboStartMin && throttleTime < turboStartMax)
            {
                force = maxForce;
                boostSound.Play();
            }
            else if (throttleTime > startSpinningValue)
            {
                spinning = true;
                spinningTime = Time.time;
                force = 0f;

                driftSound.Play();
            }
            else
                force = 0f;
            
            startDone = true;
        }

        // reset spinning
        if (Time.time - spinningTime > .6f && spinning)
        {
            spinning = false;
            driftSound.Stop();
        }

        transform.position += direction * force * Time.deltaTime;

        if (jump)
        {
            DoJump();
            transform.position = new Vector3(transform.position.x, jumpHeight, transform.position.z);
        }

        // time management
        time += Time.deltaTime;
        if (fullspeed)
            fullspeedTime += Time.deltaTime;
        else
            fullspeedTime = 0f;

        if (spriteTime <= 0f)
            spriteTime += spriteChangeTime;
        spriteTime -= Time.deltaTime;

        if (finishLineTime > 0f)
            finishLineTime -= Time.deltaTime;

        if (Time.time - crashTimeWall > .2f && crashLateral)
            crashLateral = false;
        else if (Time.time - crashTime > .12f && (crashTubeFrontal || crashTubeLateral))
        {
            if (crashTubeFrontal)
            {
                crashTubeFrontal = false;
                force = throttle == Throttle.accelerate ? maxForce * .2f : 0f;
            }
            else
                crashTubeLateral = false;
        }

        engineSound[kartType].volume = Mathf.Lerp(0f, 1f, time * 2f);
    }

    void ApplyThrottle()
    {
        switch (throttle)
        {
            case Throttle.accelerate:

                float addToForce = acceleration * force + startAcceleration;

                if (force + addToForce <= maxForce)
                    force += addToForce * Time.deltaTime * 166f;
                else if (!offroad)
                {
                    force = maxForce;
                    fullspeed = true;
                }

                break;
            case Throttle.brake:
                force -= deceleration * Time.deltaTime;
                break;
            case Throttle.none:
                break;
        }
    }

    void ApplyFriction()
    {
        if (force > 0f)
        {
            if (spinning)
                force -= friction * 1.2f * Time.deltaTime;
            else if (turning)
                force -= friction * 3f * Time.deltaTime;
            else if (crashFrontal)
            {
                force -= friction * 7f * Time.deltaTime;
                if (force < maxForce * .4f)
                {
                    crashFrontal = false;
                    force = throttle == Throttle.accelerate ? maxForce * .2f : 0f;
                }
            }
            else
                force -= friction * Time.deltaTime;

            // reduce speed slowly when entering offroad
            if (force > maxForce)
                force -= friction * 2f * Time.deltaTime;
        }

        if (force < 0f)
        {
            force = 0f;
            driftAngle = 0f;
        }

        // reset fullspeed
        if (force < maxSpeed - fullspeedHysteresis)
        {
            fullspeed = false;
            driftTimePassed = false;
        }
    }

    void ApplySteeringValue()
    {
        maxSteeringValueTemp = maxSteering / ((_speed / maxSpeed * 2f) + .25f);

        if (maxSteeringValueTemp < maxSteering && _speed > maxSpeed * .2f)
            maxSteeringValueTemp = maxSteering;

        if (maxSteeringValue < maxSteeringValueTemp)
            maxSteeringValue += .2f;
        else if (maxSteeringValue > maxSteeringValueTemp)
            maxSteeringValue -= .2f;

        // drifting
        if (Mathf.Abs(steering) >= maxSteering * .95f && driftEnabled)
        {
            drifting = true;
            driftAngle += 83f * Time.deltaTime;

            if (driftAngle > maxDriftValue)
                driftAngle = maxDriftValue;

            if (driftTime > 1.8f)
            {
                turning = true;
                if (!spinSound.isPlaying)
                    spinSound.Play();
            }

            if (!driftSound.isPlaying)
                driftSound.Play();

            driftTime += Time.deltaTime;
        }

        else
        {
            drifting = false;

            if (driftAngle > 0f)
                driftAngle = jumped ? driftAngle -= 96f * Time.deltaTime : driftAngle -= 24f * Time.deltaTime;
            else
            {
                driftAngle = 0f;
                driftTime = 0f;
                jumped = false;
            }

            if (!spinning)
                driftSound.Stop();
        }

        switch (steer)
        {
            case Steer.left:
                steering += steeringInertia * Time.deltaTime;
                break;
            case Steer.right:
                steering -= steeringInertia * Time.deltaTime;
                break;
            case Steer.none:
                if (steering > .01f)
                    steering -= steeringInertia * .7f * Time.deltaTime;
                else if (steering < -.01f)
                    steering += steeringInertia * .7f * Time.deltaTime;
                else
                    steering = 0f;

                if (_speed < maxSpeed * .01f)
                    steering = 0f;
                break;
        }

        if (steering > maxSteeringValue)
            steering = maxSteeringValue;
        else if (steering < -maxSteeringValue)
            steering = -maxSteeringValue;

        // compute direction vector
        if (steering < 0f && drifting)
            driftAngle *= -1;

        float x = -Mathf.Cos(((transform.eulerAngles.y + 90f + driftAngle) * 2f * Mathf.PI) / 360f);
        float y = Mathf.Sin(((transform.eulerAngles.y + 90f + driftAngle) * 2f * Mathf.PI) / 360f);

        if (!(crashFrontal || crashLateral || crashTubeFrontal || crashTubeLateral))
            direction = new Vector3(x, 0f, y);

        transform.Rotate(Vector3.down * steering * Mathf.Sqrt((_speed / maxForce)) * 166f * Time.deltaTime);
    }

    void DoJump()
    {
        if (!jumpSound.isPlaying)
            jumpSound.Play();

        jumpHeight = transform.position.y;

        if (transform.position.y < .12f && up)
            jumpHeight += .83f * Time.deltaTime;
        else
        {
            jumpHeight -= .83f * Time.deltaTime;
            up = false;
        }

        if (jumpHeight < 0)
        {
            jumpHeight = 0;
            jump = false;
            jumped = true;
        }
    }

    void EvaluateSprite()
    {
        // smooth the animation
        if (spriteTime <= 0f && !turning)
        {
            int oldIndex = spriteIndex;
            int spriteListCenter = Mathf.RoundToInt((sm.driverSprites[driver].Count - 1f) / 2f);

            float driftFactor = drifting ? 1.4f : .4f;

            // choose sprite when steering
            spriteIndex = Mathf.RoundToInt(((-steering / maxSteering) * spriteListCenter) * ((_speed / maxForce * .5f) + .1f) * driftFactor + spriteListCenter);

            // handle sprites at start
            if (_speed <= 0f)
            {
                if (spriteIndex > spriteListCenter + 1)
                    spriteIndex = spriteListCenter + 1;
                else if (spriteIndex < spriteListCenter - 1)
                    spriteIndex = spriteListCenter - 1;
            }
            else
            {
                if (spriteIndex > 3 * spriteListCenter / 2)
                    spriteIndex = 3 * spriteListCenter / 2;
                else if (spriteIndex < spriteListCenter / 2)
                    spriteIndex = spriteListCenter / 2;
            }


            if (spriteIndex > oldIndex + 1)
                spriteIndex = oldIndex + 1;
            else if (spriteIndex < oldIndex - 1)
                spriteIndex = oldIndex - 1;
        }
    }

    // collision
    void OnTriggerEnter(Collider info)
    {
        // compute rebound vector
        float x = 0f;
        float z = 0f;

        if (info.tag == "Wall")
        {
            float directionAngle = Vector3.Angle(direction, Vector3.forward);
            if (direction.x < 0)
                directionAngle = 360f - directionAngle;

            crashAngle = directionAngle - info.transform.eulerAngles.y;

            // handle 270 degrees wall
            if (crashAngle < -180f)
            {
                if (crashAngle < -235f)
                {
                    x = Mathf.Sin(AngleToRad(info.transform.eulerAngles.y - 135f));
                    z = Mathf.Cos(AngleToRad(info.transform.eulerAngles.y - 135f));
                    crashFrontal = true;
                }
                else
                {
                    x = Mathf.Sin(AngleToRad(info.transform.eulerAngles.y - 170f));
                    z = Mathf.Cos(AngleToRad(info.transform.eulerAngles.y - 170f));
                    crashLateral = true;
                    crashTimeWall = Time.time;
                }
            }

            else if (crashAngle > 45f && crashAngle < 135f)
            {
                x = crashAngle < 90f ? Mathf.Sin(AngleToRad(info.transform.eulerAngles.y - 45f)) : Mathf.Sin(AngleToRad(info.transform.eulerAngles.y - 135f));
                z = crashAngle < 90f ? Mathf.Cos(AngleToRad(info.transform.eulerAngles.y - 45f)) : Mathf.Cos(AngleToRad(info.transform.eulerAngles.y - 135f));
                crashFrontal = true;
            }
            else
            {

                x = crashAngle < 90f ? Mathf.Sin(AngleToRad(info.transform.eulerAngles.y - 10f)) : Mathf.Sin(AngleToRad(info.transform.eulerAngles.y - 170f));
                z = crashAngle < 90f ? Mathf.Cos(AngleToRad(info.transform.eulerAngles.y - 10f)) : Mathf.Cos(AngleToRad(info.transform.eulerAngles.y - 170f));
                crashLateral = true;
                crashTimeWall = Time.time;
            }

            direction = new Vector3(x, 0f, z);
            force = maxForce;
        }
        else if (info.tag == "Tube")
        {
            crashAngle = Vector3.Angle(direction, info.transform.position - transform.position);

            if (crashAngle < 8f)
            {
                hitDirection = 0f;

                x = -direction.x;
                z = -direction.z;

                force = maxForce / 4f;
                crashTubeFrontal = true;
            }
            else
            {
                hitDirection = Vector3.Dot(info.transform.position - transform.position, new Vector3(-direction.z, 0f, direction.x));

                float angle;

                // if tube lies left
                if (hitDirection > 0f)
                    angle = 70f;
                else
                    angle = -70f;

                x = Mathf.Sin(transform.eulerAngles.y / 360f * 2f * Mathf.PI + angle);
                z = Mathf.Cos(transform.eulerAngles.y / 360f * 2f * Mathf.PI + angle);

                if (force > maxForce / 2f)
                    force = maxForce / 2f;

                crashTubeLateral = true;
            }

            direction = new Vector3(x, 0f, z);
            crashTime = Time.time;
        }
        else if (info.name == "Speeder")
        {
            force = maxForce * 2f;
        }
        /*else if (info.name == "KartCollider")
        {
            float power = mass - info.transform.parent.GetComponent<KartController>()._mass + 4;
            float tempo = Mathf.Abs(_speed - info.transform.parent.GetComponent<KartController>()._speed);

            crashAngle = Vector3.Angle(direction, info.transform.position - transform.position);
            crashAngle = crashAngle / 360f * 2f * Mathf.PI;

            hitDirection = Vector3.Dot(info.transform.position - transform.position, new Vector3(-direction.z, 0f, direction.x));

            // if kart lies left
            if (hitDirection > 0f)
            {
                x = Mathf.Sin(transform.eulerAngles.y / 360f * 2f * Mathf.PI + crashAngle);
                z = Mathf.Cos(transform.eulerAngles.y / 360f * 2f * Mathf.PI + crashAngle);
            }
            else
            {
                x = Mathf.Sin(transform.eulerAngles.y / 360f * 2f * Mathf.PI - crashAngle);
                z = Mathf.Cos(transform.eulerAngles.y / 360f * 2f * Mathf.PI - crashAngle);
            }

            direction = -(info.transform.position - transform.position);

            force += maxForce * .02f / power * (tempo / maxSpeed);
            //TODO force muss runtergehen
            crashTubeLateral = true;

            //direction = new Vector3(x, 0f, z);
            crashTime = Time.time;

        }*/
        else if (info.tag == "FinishLine" && finishLineTime <= 0f)
        {
            if (Mathf.Abs(info.transform.rotation.y - transform.rotation.y) < .7071068f)
            {
                if (rm.isQualification || lapAntiCheatIndex >= 0)
                {
                    rm.CrossFinishLine(driver);

                    if ((isPlayer && lap < rm._laps) || (!rm.isQualification && lap == 0))
                        lc.ShowLap(lap);

                    lap++;
                }
                else
                    lapAntiCheatIndex++;
            }
            else
                lapAntiCheatIndex--;

            finishLineTime = .1f;
        }

    }

    float AngleToRad(float angle)
    {
        return angle / 360f * 2 * Mathf.PI;
    }

    // Getter and setter

    public bool _up
    {
        set
        {
            up = value;
        }
    }

    public bool _offroad
    {
        get
        {
            return offroad;
        }
    }

    public bool _driftEnabled
    {
        get
        {
            return driftEnabled;
        }
    }

    public bool _drifting
    {
        get
        {
            return drifting;
        }
        set
        {
            drifting = value;
        }
    }

    public bool _jump
    {
        get
        {
            return jump;
        }
        set
        {
            jump = value;
        }
    }

    public bool _spinning
    {
        get
        {
            return spinning;
        }
    }

    public bool _turning
    {
        get
        {
            return turning;
        }
        set
        {
            turning = value;
        }
    }

    public bool _crashed
    {
        get
        {
            return crashFrontal || crashLateral || crashTubeFrontal || crashTubeLateral;
        }
    }

    public bool _crashTube
    {
        get
        {
            return crashTubeFrontal || crashTubeLateral;
        }
    }

    public bool _spriteIndexSet
    {
        get
        {
            return spriteIndexSet;
        }
        set
        {
            spriteIndexSet = value;
        }
    }

    public int _spriteIndex
    {
        get
        {
            return spriteIndex;
        }
        set
        {
            spriteIndex = value;
        }
    }

    public float _steering
    {
        get
        {
            return steering;
        }
        set
        {
            steering = value;
        }
    }

    public float _steeringInertia
    {
        get
        {
            return steeringInertia;
        }
    }

    public float _maxSteering
    {
        get
        {
            return maxSteering;
        }
    }

    public float _speed
    {
        get
        {
            return speedometer._speed;
        }
    }

    public float _maxSpeed
    {
        get
        {
            return maxSpeed;
        }
    }

    public float _crashAngle
    {
        get
        {
            return crashAngle;
        }
    }

    public float _tubeDirection
    {
        get
        {
            return hitDirection;
        }
    }

    public Vector3 _direction
    {
        get
        {
            return direction;
        }
    }

    public Vector3 _spritePosition
    {
        get
        {
            return spritePosition;
        }
        set
        {
            spritePosition = value;
        }
    }
}