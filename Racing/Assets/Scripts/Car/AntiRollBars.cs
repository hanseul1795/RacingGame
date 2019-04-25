using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBars : MonoBehaviour
{
    public WheelCollider WheelL;
    public WheelCollider WheelR;
    public float AntiRoll = 5000.0f;

    private Rigidbody rb;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.centerOfMass += new Vector3(0.0f, 0.0f, -1.0f);
    }

    private void FixedUpdate()
    {
        Debug.DrawLine(rb.centerOfMass + (transform.right * 2.0f), rb.centerOfMass + (-transform.right* 2.0f), Color.red);

        WheelHit hit;
        float travelL = 1.0f;
        float travelR = 1.0f;

        var groundedL = WheelL.GetGroundHit(out hit);

        if (groundedL)
            travelL = (-WheelL.transform.InverseTransformPoint(hit.point).y - WheelL.radius) / WheelL.suspensionDistance;

        var groundedR = WheelR.GetGroundHit(out hit);
        if (groundedR)
            travelR = (-WheelR.transform.InverseTransformPoint(hit.point).y - WheelR.radius) / WheelR.suspensionDistance;

        var antiRollForce = (travelL - travelR) * AntiRoll;

        if (groundedL)
            rb.AddForceAtPosition(WheelL.transform.up * -antiRollForce,
                   WheelL.transform.position);
        if (groundedR)
            rb.AddForceAtPosition(WheelR.transform.up * antiRollForce,
                   WheelR.transform.position);
    }
}
