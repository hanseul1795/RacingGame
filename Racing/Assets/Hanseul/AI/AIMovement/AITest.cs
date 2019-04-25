using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class AITest : MonoBehaviour
{
    public Transform path;
    private List<CheckPoint> nodes;
    private int currectNode = 0;
    private bool avoiding = false;
    private bool backward = false;
    private bool isBraking = false;
    private float targetSteerAngle = 0;
    private float timer = 0;
    private bool isWheelSensorEnabled = false;
    private float backwardAngle = 0;
    public float MaxSpeed;
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

    private Rigidbody car;

    public Transform rayOrigin;
    public LayerMask rayMask;
    private RaycastHit hit;
    public RaycastHit Hit { get { return hit; } private set { } }
    public float rayDistance = 1.0f;
    private bool grounded = true;

    public AdvancedOptions advancedOptions;

    private WheelCollider[] m_Wheels;

    public float impactThreshold = 1.0f;

    public bool magnetic = false;
    public float stickiness = 1.0f;

    [Header("Sensors")]
    public float sensorLength = 1000f;
    public Vector3 frontSensorPosition = new Vector3(0f, 0.2f, 0.5f);
    public float frontSideSensorPosition = 0.2f;
    public float frontSensorAngle = 35f;

    [Header("LOOPSensor")]
    public float loopsensorLegnth = 1000f;
    public Vector3 LoopSensorPosition = new Vector3(0f, 1f, 0.5f);
    public float LoopSideSensorPos = 1f;
    public float LoopSteerAngle = 20f;

    void Start()
    {
        car = GetComponent<Rigidbody>();

        car.mass = carWeight;
        m_Wheels = GetComponentsInChildren<WheelCollider>();
        CheckPoint[] pathTransforms = path.GetComponentsInChildren<CheckPoint>();
        nodes = new List<CheckPoint>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
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
    }
    void Update()
    {
      
    }


    private void FixedUpdate()
    {
        NormalDrive();
        LoopDrive(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<SphereCollider>() == nodes[currectNode].GetComponent<SphereCollider>())
        {
            if (currectNode == nodes.Count - 1)
            {
                currectNode = 0;
            }
            else
            {
                currectNode++;
            }
            if (other.gameObject.tag == "LoopEnter")
            {
                magnetic = true;
                isWheelSensorEnabled = true;
                GetComponent<Rigidbody>().useGravity = false;
            }
            if (other.gameObject.tag == "LoopExit")
            {
                magnetic = false;
                isWheelSensorEnabled = false;
                GetComponent<Rigidbody>().useGravity = true;
            }
            if(other.gameObject.tag == "Brake")
            {
                isBraking = true;
                StartCoroutine(BrakeCoroutine());
            }     
        }
    }
    IEnumerator BrakeCoroutine()
    {
        yield return new WaitForSeconds(0.15f);
        ReleaseBrake();
    }
    private void OnDrawGizmos()
    {
        if (magnetic)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, -transform.up * rayDistance);
        }
    }
    private void Drive()
    {      
        m_Wheels[0].ConfigureVehicleSubsteps(advancedOptions.criticalSpeed, advancedOptions.stepsBelow, advancedOptions.stepsAbove);
        foreach (WheelCollider wheel in m_Wheels)
        {
            if (wheel.transform.localPosition.z > 0)
            {
                if (!isBraking && Math.Abs(wheel.motorTorque) < maxTorque)
                {
                    if(backward)
                    {
                        wheel.motorTorque += -maxTorque * Time.deltaTime * 30;
                        targetSteerAngle = backwardAngle;
                    }
                    else
                        wheel.motorTorque += maxTorque * Time.deltaTime;
                }
                else if(!isBraking && Math.Abs(wheel.motorTorque) >= maxTorque)
                {
                    if(backward)
                    {
                        wheel.motorTorque = -maxTorque;
                        targetSteerAngle = backwardAngle;
                    }
                    else
                    wheel.motorTorque = maxTorque;
                }
                else
                {
                    wheel.motorTorque = 0;
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

    private void Braking()
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
    private void LerpToSteerAngle()
    {
        foreach (WheelCollider wheel in m_Wheels)
        {
            if (wheel.transform.localPosition.z > 0)
            {
                wheel.steerAngle = Mathf.Lerp(wheel.steerAngle, targetSteerAngle, Time.deltaTime * maxAngle);
            }
        }
    }
    private void SteerLoop()
    {
        foreach (WheelCollider wheel in m_Wheels)
        {
            if (wheel.transform.localPosition.z > 0)
            {
                wheel.steerAngle = Mathf.Lerp(wheel.steerAngle, targetSteerAngle, Time.deltaTime *  200);
            }

        }
    }
    private void ApplySteer()
    {
        if (avoiding)
        {
            return;
        }
        
        Vector3 relativeVector = GetComponent<Transform>().InverseTransformPoint(nodes[currectNode].GetComponent<Transform>().position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxAngle;
        targetSteerAngle = newSteer;
        backwardAngle = -targetSteerAngle;
    }
    

    private void Sensors()
    {
        if (isWheelSensorEnabled)
            return;

        RaycastHit hit;
        Vector3 sensorStartPos = GetComponent<Transform>().position;
        sensorStartPos += GetComponent<Transform>().forward * frontSensorPosition.z;
        sensorStartPos += GetComponent<Transform>().up * frontSensorPosition.y;
        float avoidMultiplier = 0;
        avoiding = false;

        //front right sensor
        sensorStartPos += GetComponent<Transform>().right * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, GetComponent<Transform>().forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier = 1f;
                if (hit.distance <= 2f && !isBraking && !backward)
                {
                    isBraking = true;
                }
                if(isBraking)
                {
                    timer += Time.deltaTime;
                    if (timer >= 1.5f)
                    {
                        GoBack();
                        timer = 0;
                    }
                }
            }
        }
        //front right angle sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, GetComponent<Transform>().up) * GetComponent<Transform>().forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier += 0.5f;
                if (hit.distance <= 2f && !isBraking && !backward)
                {
                    isBraking = true;
                }

                if (isBraking)
                {
                    timer += Time.deltaTime;
                    if (timer >= 1.5f)
                    {
                        GoBack();
                        timer = 0;
                    }
                }
            }
        }
        //front left sensor
        sensorStartPos -= GetComponent<Transform>().right * frontSideSensorPosition * 2;
        if (Physics.Raycast(sensorStartPos, GetComponent<Transform>().forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier -= 1f;
                if (hit.distance <= 2f && !isBraking && !backward)
                {
                    isBraking = true;
                }
                if (isBraking)
                {
                    timer += Time.deltaTime;
                    if (timer >= 1.5f)
                    {
                        GoBack();
                        timer = 0;
                    }
                }
            }
        }

        //front left angle sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, GetComponent<Transform>().up) * GetComponent<Transform>().forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier -= 0.5f;
                if (hit.distance <= 2f && !isBraking && !backward)
                {
                    isBraking = true;
                }
                if (isBraking)
                {
                    timer += Time.deltaTime;
                    if (timer >= 1.5f)
                    {
                        GoBack();
                        timer = 0;
                    }
                }
            }
        }
        else
        {
            avoiding = false;
        }
        //front center sensor
        if (avoidMultiplier == 0)
        {
            if (Physics.Raycast(sensorStartPos, GetComponent<Transform>().forward, out hit, sensorLength))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    avoiding = true;
                    if (hit.normal.x < 0)
                    {
                        avoidMultiplier = 1;
                    }
                    else
                    {
                        avoidMultiplier = -1;
                    }
                    if (hit.distance <= 2f && !isBraking && !backward)
                    {
                        isBraking = true;
                    }
                    if (isBraking)
                    {
                        timer += Time.deltaTime;
                        if (timer >= 1.5f)
                        {
                            GoBack();
                            timer = 0;
                        }
                    }
                }
            }
        }
        if (avoiding)
        {
            targetSteerAngle = maxAngle * avoidMultiplier;
            backwardAngle = -targetSteerAngle;
        }

        else if(!avoiding && backward)
        {
            backward = false;
        }
    }

    void GoBack()
    {
        isBraking = false;
        backward = true;
    }

    void ReleaseBrake()
    {
        isBraking = false;
    }


    void LoopDrive()
    {
        if (!isWheelSensorEnabled)
            return;

        //LoopSensor();
        //LoopRay();
        LoopDriving();
        SteerLoop();
    }
    
    void NormalDrive()
    {
        if (isWheelSensorEnabled)
            return;

        Sensors();
        ApplySteer();
        Drive();
        Braking();
        LerpToSteerAngle();
    }

    void LoopDriving()
    {
        car.AddForce(-GetComponent<Transform>().up * stickiness, ForceMode.Impulse);
        m_Wheels[0].ConfigureVehicleSubsteps(advancedOptions.criticalSpeed, advancedOptions.stepsBelow, advancedOptions.stepsAbove);
        foreach (WheelCollider wheel in m_Wheels)
        {
            if (wheel.transform.localPosition.z > 0)
            {
                 if (!isBraking && Math.Abs(wheel.motorTorque) < maxTorque)
                 {
                    wheel.motorTorque += maxTorque * Time.deltaTime;
                }
                 else if (!isBraking && Math.Abs(wheel.motorTorque) >= maxTorque)
                 {

                    wheel.motorTorque = maxTorque;
                }
                 else
                 {
                     wheel.motorTorque = 0;
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
}