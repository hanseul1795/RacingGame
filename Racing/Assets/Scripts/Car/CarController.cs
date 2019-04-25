using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum DriveTrain
{
    Propulsion,     // Rear-Wheel Drive
    Traction,       // Front-Wheel Drive
    Integral        // All-Wheel Drive
}
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Serializable]
    public class AdvancedOptions
    {
        public float criticalSpeed      = 8f;   // m/s
        public int stepsBelow           = 7;    // substep above critical
        public int stepsAbove           = 2;    // substep below critical
        [Range(1.0f, 10.0f)]
        public float forwardFriction    = 3.0f;
        [Range(1.0f, 10.0f)]
        public float sidewaysFriction   = 2.5f;
    }

    [Range(1000, 5000)]                      // below 1500 = compact car, between 1500-2000 midsize large, 2000+ heavy
    public float carWeight          = 1800f;

    [Range(10, 50)]
    public float maxAngle           = 35f;
    [Range(100, 5000)]
    public float maxTorque          = 1000f;
    public float brakeTorque        = 30000f;

    public GameObject wheelShape;

    [Tooltip("Propulsion \t= Rear-Wheel Drive, \nTraction \t= Front-Wheel Drive, \nIntegral \t= All-Wheel Drive")]
    public DriveTrain driveType     = DriveTrain.Integral;

    private Rigidbody car;

    public Transform rayOrigin;
    public LayerMask rayMask;
    private RaycastHit hit;
    public RaycastHit Hit { get { return hit; } private set { } }
    public float rayDistance        = 1.0f;
    private bool grounded           = true;

    public AdvancedOptions advancedOptions;

    private WheelCollider[] m_Wheels;

    [SerializeField]
    private Camera playerCam;
    [SerializeField]
    private Camera uiCam;

    public float impactThreshold    = 1.0f;

    public bool magnetic            = false;
    public float stickiness         = 1.0f;

    PickUpController pickupController;

    //private AntiRollBars antiRoll;

    void Start()
    {
        car = GetComponent<Rigidbody>();

        car.mass = carWeight;
        m_Wheels = GetComponentsInChildren<WheelCollider>();

        pickupController = GetComponent<PickUpController>();

        for (int i = 0; i < m_Wheels.Length; ++i)
        {
            WheelCollider wheel     = m_Wheels[i];

            WheelFrictionCurve fwd  = new WheelFrictionCurve();
            fwd.extremumSlip        = wheel.forwardFriction.extremumSlip;
            fwd.extremumValue       = wheel.forwardFriction.extremumValue;
            fwd.asymptoteSlip       = wheel.forwardFriction.asymptoteSlip;
            fwd.asymptoteValue      = wheel.forwardFriction.asymptoteValue;
            fwd.stiffness           = advancedOptions.forwardFriction;

            WheelFrictionCurve sdw  = new WheelFrictionCurve();
            sdw.extremumSlip        = wheel.sidewaysFriction.extremumSlip;
            sdw.extremumValue       = wheel.sidewaysFriction.extremumValue;
            sdw.asymptoteSlip       = wheel.sidewaysFriction.asymptoteSlip;
            sdw.asymptoteValue      = wheel.sidewaysFriction.asymptoteValue;
            sdw.stiffness           = advancedOptions.sidewaysFriction;

            wheel.forwardFriction   = fwd;
            wheel.sidewaysFriction  = sdw;

            if (wheelShape)
            {
                GameObject ws       = Instantiate(wheelShape);
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

        //antiRoll = gameObject.GetComponent<AntiRollBars>();
    }

    void Update()
    {
        CheckRay();
        UpdateCar();

        if(Input.GetButton("Fire1"))
        {
            if(pickupController) pickupController.Use();
        }

        if((!grounded || new Vector3(car.velocity.x, 0, car.velocity.z).magnitude <= 1.0f) && Input.GetButtonDown("Respawn"))
        {
            GameObject car = gameObject;
            RespawnManager.Instance.Respawn(ref car);
            Debug.Log("Trying to respawn");
        }

    }

    private void UpdateCar()
    {
        if (!GameManager.Instance.raceStart)
            car.velocity = new Vector3(0.0f, car.velocity.y, 0.0f);

        m_Wheels[0].ConfigureVehicleSubsteps(advancedOptions.criticalSpeed, advancedOptions.stepsBelow, advancedOptions.stepsAbove);

        float angle = maxAngle * Input.GetAxis("Horizontal");
        float torque = maxTorque * Input.GetAxis("Vertical");

        //float handBrake = Input.GetKey(KeyCode.X) ? brakeTorque : 0;
        float handBrake = Input.GetButton("Break") ? brakeTorque : 0;

        foreach (WheelCollider wheel in m_Wheels)
        {
            if (wheel.transform.localPosition.z > 0)
                wheel.steerAngle = angle;

            if (wheel.transform.localPosition.z < 0)
            {
                wheel.brakeTorque = handBrake;
            }

            if (wheel.transform.localPosition.z < 0 && driveType != DriveTrain.Traction)
            {
                wheel.motorTorque = torque;
            }

            if (wheel.transform.localPosition.z >= 0 && driveType != DriveTrain.Propulsion)
            {
                wheel.motorTorque = torque;
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
    private void CheckRay()
    {
        if (rayOrigin)
        {
            if (Physics.Raycast(rayOrigin.position, -rayOrigin.up, out hit, rayDistance, rayMask))
            {
                grounded = true;

                if (magnetic)
                {
                    car.AddForce(-hit.normal * stickiness, ForceMode.Impulse);
                }
            }
            else
            {
                grounded = false;
            }
        }
    }
    private void FixedUpdate()
    {
        if (!grounded)
        {
            //Quaternion rot          = Quaternion.Euler((transform.localRotation.eulerAngles + new Vector3(Input.GetAxis("Vertical") * 1.5f, 0.0f, -Input.GetAxis("Horizontal") * 1.5f)));
            //transform.localRotation = rot;
            car.rotation            = Quaternion.Slerp(car.rotation, Quaternion.Euler(car.rotation.eulerAngles.x, car.rotation.eulerAngles.y, 0.0f), 2.0f * Time.deltaTime);
        }

        if(car.velocity.magnitude > 25.0f)
        {
            playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, 90.0f, 1.0f * Time.deltaTime);
            uiCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, 90.0f, 1.0f * Time.deltaTime);
            playerCam.GetComponentInParent<CameraController>().smoothing = Mathf.Lerp(playerCam.GetComponentInParent<CameraController>().smoothing, 13.0f, 1.0f * Time.deltaTime);
        }
        else
        {
            playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, 60.0f, 1.0f * Time.deltaTime);
            uiCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, 60.0f, 1.0f * Time.deltaTime);
            playerCam.GetComponentInParent<CameraController>().smoothing = Mathf.Lerp(playerCam.GetComponentInParent<CameraController>().smoothing, 6.0f, 1.0f * Time.deltaTime);

        }
        //Debug.Log(car.velocity.sqrMagnitude);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > impactThreshold)
        {
            playerCam.gameObject.GetComponent<CameraShake>().PlayShake(collision.relativeVelocity.magnitude * 0.05f, collision.relativeVelocity.magnitude * 0.1f, collision.relativeVelocity.magnitude * 0.03f);
        }
    }

    private void OnDrawGizmos()
    {
        if (magnetic)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, -transform.up * rayDistance);
        }
    }
}
