using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public abstract class AIMovementBase : MonoBehaviour
{
    public int CurrentRank = 0;
    public int CurrentLap = 0;
    public int MaxLap = 3;
    public bool lapUpdated = false;
    public Transform path;
    public Transform LastPath;
    public List<CheckPoint> nodes = new List<CheckPoint>();
    public List<CheckPoint> lastLapPoints = new List<CheckPoint>();
    public Transform TargetPowerUpLocation = null;
    protected List<PickUpObjectContainer> pickUps = new List<PickUpObjectContainer>();

    protected float avoidMultiplier = 0;
    public int currentNode = 0;
    protected int currentpowerUpNode = 0;
    protected bool avoid = false;
    protected bool attack = false;
    public bool respawnAvailable = true;
    protected float timer = 0;
    private Transform TargetTransform = null;
    public bool useItem = false;
    private bool ignorePowerUp = false;
    protected bool backward = false;
    protected bool isBraking = false;
    protected bool enablePowerUpUpdate = false;
    protected float targetSteerAngle = 0;
    protected float backwardAngle = 0;
    private float respawntime = 0;
    private float respawnAvailableTime = 0;
    [Serializable]
    public class AdvancedOptions
    {
        public float criticalSpeed = 8f;   // m/s
        public int stepsBelow = 7;    // substep above critical
        public int stepsAbove = 2;    // substep below critical
        [Range(1.0f, 10.0f)]
        public float forwardFriction = 3.0f;
        [Range(1.0f, 10.0f)]
        public float sidewaysFriction = 2.5f;
    }

    [Range(1000, 5000)]                      // below 1500 = compact car, between 1500-2000 midsize large, 2000+ heavy
    public float carWeight = 1800f;

    [Range(10, 50)]
    public float maxAngle = 35f;
    [Range(100, 5000)]
    public float maxTorque = 1000f;
    public float brakeTorque = 30000f;

    public GameObject wheelShape;

    [Tooltip("Propulsion \t= Rear-Wheel Drive, \nTraction \t= Front-Wheel Drive, \nIntegral \t= All-Wheel Drive")]
    public DriveTrain driveType = DriveTrain.Integral;

    protected Rigidbody car;
    public AdvancedOptions advancedOptions;
    protected WheelCollider[] m_Wheels;

    public float impactThreshold = 1.0f;
    public bool magnetic = false;
    public float stickiness = 1.0f;

    [Header("Sensors")]
    public float sensorLength = 1000f;
    public Vector3 frontSensorPosition = new Vector3(0f, 0.2f, 0.5f);
    public float frontSideSensorPosition = 0.2f;
    public float frontSensorAngle = 35f;

    void Start()
    {
        
        car = GetComponent<Rigidbody>();
        car.mass = carWeight;
        m_Wheels = GetComponentsInChildren<WheelCollider>();
        CheckPoint[] pathTransforms = path.GetComponentsInChildren<CheckPoint>();
        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
        currentNode = nodes.Count - 1;
        for (int i = 0; i < m_Wheels.Length; ++i)
        {
            WheelCollider wheel = m_Wheels[i];

            WheelFrictionCurve fwd = new WheelFrictionCurve();
            fwd.extremumSlip = wheel.forwardFriction.extremumSlip;
            fwd.extremumValue = wheel.forwardFriction.extremumValue;
            fwd.asymptoteSlip = wheel.forwardFriction.asymptoteSlip;
            fwd.asymptoteValue = wheel.forwardFriction.asymptoteValue;
            fwd.stiffness = advancedOptions.forwardFriction;

            WheelFrictionCurve sdw = new WheelFrictionCurve();
            sdw.extremumSlip = wheel.sidewaysFriction.extremumSlip;
            sdw.extremumValue = wheel.sidewaysFriction.extremumValue;
            sdw.asymptoteSlip = wheel.sidewaysFriction.asymptoteSlip;
            sdw.asymptoteValue = wheel.sidewaysFriction.asymptoteValue;
            sdw.stiffness = advancedOptions.sidewaysFriction;

            wheel.forwardFriction = fwd;
            wheel.sidewaysFriction = sdw;

            if (wheelShape)
            {
                GameObject ws = Instantiate(wheelShape);
                ws.transform.parent = wheel.transform;

                if (wheel.transform.localPosition.z < 0)
                {
                    ws.transform.localScale = wheel.transform.localPosition.x < 0 ? new Vector3(-ws.transform.localScale.x - 0.1f, ws.transform.localScale.y + 0.1f, ws.transform.localScale.z + 0.1f) :
                                                                                    new Vector3(ws.transform.localScale.x + 0.1f, ws.transform.localScale.y + 0.1f, ws.transform.localScale.z + 0.1f);
                }
                else
                {
                    ws.transform.localScale = wheel.transform.localPosition.x < 0 ? new Vector3(-ws.transform.localScale.x, ws.transform.localScale.y, ws.transform.localScale.z) :
                                                                                   new Vector3(ws.transform.localScale.x, ws.transform.localScale.y, ws.transform.localScale.z);
                }
            }
        }
        PickUpObjectContainer[] pickUpContainers = GetComponentsInChildren<PickUpObjectContainer>();
        for (int i = 0; i < pickUpContainers.Length; i++)
        {
            pickUps.Add(pickUpContainers[i]);
        }
        Debug.Log("INIT ENDED");
    }
    protected void Init()
    {
        car = GetComponent<Rigidbody>();
        car.mass = carWeight;
        m_Wheels = GetComponentsInChildren<WheelCollider>();
        CheckPoint[] pathTransforms = path.GetComponentsInChildren<CheckPoint>();
        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
        currentNode = nodes.Count - 1;

        CheckPoint[] LastPaths = LastPath.GetComponentsInChildren<CheckPoint>();
        for (int i = 0; i < LastPaths.Length; i++)
        {
            if (LastPaths[i] != LastPath.transform)
            {
                lastLapPoints.Add(LastPaths[i]);
            }
        }
        
        for (int i = 0; i < m_Wheels.Length; ++i)
        {
            WheelCollider wheel = m_Wheels[i];

            WheelFrictionCurve fwd = new WheelFrictionCurve();
            fwd.extremumSlip = wheel.forwardFriction.extremumSlip;
            fwd.extremumValue = wheel.forwardFriction.extremumValue;
            fwd.asymptoteSlip = wheel.forwardFriction.asymptoteSlip;
            fwd.asymptoteValue = wheel.forwardFriction.asymptoteValue;
            fwd.stiffness = advancedOptions.forwardFriction;

            WheelFrictionCurve sdw = new WheelFrictionCurve();
            sdw.extremumSlip = wheel.sidewaysFriction.extremumSlip;
            sdw.extremumValue = wheel.sidewaysFriction.extremumValue;
            sdw.asymptoteSlip = wheel.sidewaysFriction.asymptoteSlip;
            sdw.asymptoteValue = wheel.sidewaysFriction.asymptoteValue;
            sdw.stiffness = advancedOptions.sidewaysFriction;

            wheel.forwardFriction = fwd;
            wheel.sidewaysFriction = sdw;

            if (wheelShape)
            {
                GameObject ws = Instantiate(wheelShape);
                ws.transform.parent = wheel.transform;

                if (wheel.transform.localPosition.z < 0)
                {
                    ws.transform.localScale = wheel.transform.localPosition.x < 0 ? new Vector3(-ws.transform.localScale.x - 0.1f, ws.transform.localScale.y + 0.1f, ws.transform.localScale.z + 0.1f) :
                                                                                    new Vector3(ws.transform.localScale.x + 0.1f, ws.transform.localScale.y + 0.1f, ws.transform.localScale.z + 0.1f);
                }
                else
                {
                    ws.transform.localScale = wheel.transform.localPosition.x < 0 ? new Vector3(-ws.transform.localScale.x, ws.transform.localScale.y, ws.transform.localScale.z) :
                                                                                   new Vector3(ws.transform.localScale.x, ws.transform.localScale.y, ws.transform.localScale.z);
                }
            }
        }
        PickUpObjectContainer[] pickUpContainers = GetComponentsInChildren<PickUpObjectContainer>();
        for (int i = 0; i < pickUpContainers.Length; i++)
        {
            pickUps.Add(pickUpContainers[i]);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
    protected void Braking()
    {
        foreach (WheelCollider wheel in m_Wheels)
        {
            if (isBraking)
            {
                if (wheel.transform.localPosition.z < 0)
                {
                    wheel.brakeTorque = brakeTorque;
                }
            }
            else
            {
                if (wheel.transform.localPosition.z < 0)
                {
                    wheel.brakeTorque = 0;
                }
            }
            if (wheel.transform.GetChild(0))
            {
                Quaternion q;
                Vector3 p;
                wheel.GetWorldPose(out p, out q);

                Transform shapeTransform = wheel.transform.GetChild(0);
                shapeTransform.position = p;
                shapeTransform.rotation = q;
            }
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        UpdatePath(other);
        EnterLoop(other);
        ExitLoop(other);
        BrakeOnCurve(other);
        EnablePowerUpUpdate(other);
        DisablePowerUpUpdate(other);
        UpdateLap(other);
        Magnetize(other);
    }
    
    IEnumerator BrakeCoroutine()
    {
        yield return new WaitForSeconds(0.15f);
        isBraking = false;
    }

    protected void UpdatePowerUp()
    {
        if (enablePowerUpUpdate)
        {
            TargetPowerUpLocation = pickUps[currentpowerUpNode].UpdateAvailablePowerUp();

        }
    }

    protected void UpdatePath(Collider other)
    {
        if (CurrentLap < MaxLap)
        {
            if (other.gameObject.GetComponent<SphereCollider>() == nodes[currentNode].GetComponent<SphereCollider>())
            {
                if (currentNode == 0)
                {
                    currentNode = nodes.Count - 1;
                }
                else
                {
                    currentNode--;
                }

            }
        }
        else if(CurrentLap == MaxLap)
        {
            if (other.gameObject.GetComponent<SphereCollider>() == lastLapPoints[currentNode].GetComponent<SphereCollider>())
            {
                if (currentNode == 0)
                {
                    currentNode = lastLapPoints.Count - 1;
                }
                else
                {
                    currentNode--;
                }
            }
        }
    }
    protected void EnterLoop(Collider other)
    {
        if (other.gameObject.tag == "LoopEnter")
        {
            if (lapUpdated)
                lapUpdated = false;
            magnetic = true;
            GetComponent<Rigidbody>().useGravity = false;
        }
    }
    protected void ExitLoop(Collider other)
    {
        if (other.gameObject.tag == "LoopExit")
        {
            magnetic = false;
            GetComponent<Rigidbody>().useGravity = true;
        }
    }
    protected void BrakeOnCurve(Collider other)
    {
        if (other.gameObject.tag == "Brake")
        {
            isBraking = true;
            StartCoroutine(BrakeCoroutine());
        }
    }
    protected void UpdateLap(Collider other)
    {
        if(other.gameObject.tag == "EndOfLap" && !lapUpdated)
        {
            lapUpdated = true;
            ++CurrentLap;
            Debug.Log(CurrentLap);
            if (CurrentLap == MaxLap)
                currentNode = lastLapPoints.Count - 1;
        }
    }
    protected void EnablePowerUpUpdate(Collider other)
    {
        if (other.gameObject.tag == "PowerUpEnter")
            enablePowerUpUpdate = true;
        
    }
    protected void DisablePowerUpUpdate(Collider other)
    {
        if (other.gameObject.tag == "PowerUpExit")
            enablePowerUpUpdate = false;
    }
    protected void Magnetize(Collider other)
    {
        if (other.gameObject.tag == "Magnatize")
        {
            magnetic = true;
        }
    }
    protected virtual void CalculateSteerAngle()
    {
        if (nodes.Count == 0)
            return;

        if (avoid)
            return;

        if(enablePowerUpUpdate && TargetPowerUpLocation != null && !ignorePowerUp)
        {
            TargetTransform = TargetPowerUpLocation;
        }
        else if(CurrentLap < MaxLap)
        {
            TargetTransform = nodes[currentNode].GetComponent<Transform>();
        }
        else if(CurrentLap == MaxLap)
        {
            TargetTransform = lastLapPoints[currentNode].GetComponent<Transform>();
        }
        Vector3 relativeVector = GetComponent<Transform>().InverseTransformPoint(TargetTransform.position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxAngle;
        targetSteerAngle = newSteer;
        backwardAngle = -targetSteerAngle;
    }

    protected void InterpolateSteering()
    {
        foreach (WheelCollider wheel in m_Wheels)
        {
            if (wheel.transform.localPosition.z > 0)
            {
                wheel.steerAngle = Mathf.Lerp(wheel.steerAngle,targetSteerAngle, Time.deltaTime * maxAngle * 10);
            }
        }
    }
    protected void UsePowerUp()
    {
        useItem = true;
    }
    protected void GoBack()
    {
        isBraking = false;
        backward = true;
    }
    public void Respawn()
    {
        respawnAvailable = true;
    }
    public void RequestRespawningByWheel()
    {
        if (respawnAvailable)
        {
            for (int i = 0; i < m_Wheels.Length; ++i)
            {
                WheelHit hit;
                if (m_Wheels[i].GetGroundHit(out hit))
                {
                    respawntime = 0;
                    return;
                }

            }
            respawntime += Time.deltaTime;
            if (respawntime >= 4)
            {
                gameObject.SetActive(false);
                GameObject car = gameObject;
                RespawnManager.Instance.Respawn(ref car);
                gameObject.SetActive(true);
                respawntime = 0;
                respawnAvailable = false;
            }
        }
    }
 
    public IEnumerator RespawnCoroutine()
    {
        Vector3 CurrentPosition = GetComponent<Transform>().position;
        yield return new WaitForSeconds(2);
        Vector3 NextPosition = DidTheCarMove();
        if (Vector3.Distance(CurrentPosition, NextPosition) < 10f)
            Respawn();
    }

    public Vector3 DidTheCarMove()
    {
        return GetComponent<Transform>().position;
    }
    protected void ReUpdateRespawnAvailability()
    {
        if (!respawnAvailable)
            respawnAvailableTime += Time.deltaTime;
        if(respawnAvailableTime >= 10)
        {
            respawnAvailable = true;
        }
    }
    protected abstract void DetectFromFrontLeftSide();
    protected abstract void DetectFromFrontRightSide();
    protected abstract void DetectFromBackLeftSide();
    protected abstract void DetectFromBackRightSide();
    protected abstract void Detect();
    protected abstract void Decide();
    protected abstract void Drive();
}