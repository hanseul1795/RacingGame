using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSuspension : MonoBehaviour
{
    [Range(0.1f, 20f)]
    public float naturalFrequency = 10;

    [Range(0f, 3f)]
    public float dampingRatio = 0.8f;

    [Range(-1f, 1f)]
    public float forceShift = 0.03f;

    public bool setSuspensionDistance = true;

    private Rigidbody m_Rigidbody;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        foreach (WheelCollider wc in GetComponentsInChildren<WheelCollider>())
        {
            JointSpring spring = wc.suspensionSpring;

            float sqrtWcSprungMass = Mathf.Sqrt(wc.sprungMass);
            spring.spring = sqrtWcSprungMass * naturalFrequency * sqrtWcSprungMass * naturalFrequency;
            spring.damper = 2f * dampingRatio * Mathf.Sqrt(spring.spring * wc.sprungMass);

            wc.suspensionSpring = spring;

            Vector3 wheelRelativeBody = transform.InverseTransformPoint(wc.transform.position);
            float distance = m_Rigidbody.centerOfMass.y - wheelRelativeBody.y + wc.radius;

            wc.forceAppPointDistance = distance - forceShift;

            if (spring.targetPosition > 0 && setSuspensionDistance)
                wc.suspensionDistance = wc.sprungMass * Physics.gravity.magnitude / (spring.targetPosition * spring.spring);
        }
    }
}
