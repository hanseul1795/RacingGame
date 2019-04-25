using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AggresifAI : AIMovementBase
{
    public LayerMask rayMask;
    private RaycastHit hit;

    void Start()
    {
        Init();
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.raceStart)
            car.velocity = new Vector3(0.0f, car.velocity.y, 0.0f);

        else
        {
            Detect();
            //Decide();
            CalculateSteerAngle();
            Drive();
            Braking();
            InterpolateSteering();
            UseItem();
            ReUpdateRespawnAvailability();
            RequestRespawningByWheel();
        }
    }
    protected override void Detect()
    {
        avoidMultiplier = 0;
        avoid = false;
        DetectFromFrontLeftSide();
        DetectFromFrontRightSide();
        if (avoidMultiplier == 0)
        {
            RaycastHit hit;
            Vector3 sensorStartPos = GetComponent<Transform>().position; 
            sensorStartPos = GetComponent<Transform>().forward * frontSensorPosition.z;
            sensorStartPos += GetComponent<Transform>().up * frontSensorPosition.y;
            if (Physics.Raycast(sensorStartPos, GetComponent<Transform>().forward, out hit, sensorLength))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    avoid = true;
                    if (hit.normal.x < 0)
                    {
                        avoidMultiplier = 1;
                    }
                    else
                    {
                        avoidMultiplier = -1;
                    }
                    if (hit.distance <= 1f && !isBraking && !backward)
                    {
                        Vector3 CurrentPos = GetComponent<Transform>().position;
                        int nextnode;
                        if (currentNode == 0)
                            nextnode = nodes.Count - 1;
                        else
                            nextnode = currentNode - 1;
                        if (Vector2.Distance(CurrentPos, nodes[currentNode].GetComponent<Transform>().position) > Vector2.Distance(CurrentPos, nodes[currentNode].GetComponent<Transform>().position))
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
        if (avoid)
        {
            targetSteerAngle = maxAngle * -avoidMultiplier;
            backwardAngle = -targetSteerAngle;
        }

        else if (!avoid && backward)
        {
            backward = false;
        }
    }
    protected override void Decide()
    {
        
    }
    protected override void Drive()
    {
        //if(magnetic)
        //    car.AddForce(-GetComponent<Transform>().up * stickiness, ForceMode.Impulse);
        if (Physics.Raycast(transform.position, -transform.up, out hit, 5, rayMask))
        {
            if (magnetic)
            {
                car.AddForce(-hit.normal * stickiness, ForceMode.Impulse);
            }
        }

        m_Wheels[0].ConfigureVehicleSubsteps(advancedOptions.criticalSpeed, advancedOptions.stepsBelow, advancedOptions.stepsAbove);
        foreach (WheelCollider wheel in m_Wheels)
        {
            if (wheel.transform.localPosition.z > 0)
            {
                if (!isBraking && Math.Abs(wheel.motorTorque) < maxTorque)
                {
                    if (backward)
                    {
                        wheel.motorTorque += -maxTorque * Time.deltaTime * 30;
                        targetSteerAngle = backwardAngle;
                    }
                    else
                        wheel.motorTorque += maxTorque * Time.deltaTime;
                }
                else if (!isBraking && Math.Abs(wheel.motorTorque) >= maxTorque)
                {
                    if (backward)
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
    protected override void DetectFromFrontLeftSide()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = m_Wheels[1].GetComponent<Transform>().position;
        sensorStartPos += GetComponent<Transform>().forward * frontSensorPosition.z;
        sensorStartPos += GetComponent<Transform>().up * frontSensorPosition.y;
        sensorStartPos -= GetComponent<Transform>().right * frontSensorPosition.x;
        if (Physics.Raycast(sensorStartPos, (GetComponent<Transform>().right * -1f), out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoid = true;
                avoidMultiplier -= 1f;
                if (hit.distance <= 0.5f && !isBraking && !backward)
                {
                    Vector3 CurrentPos = GetComponent<Transform>().position;
                    int nextnode;
                    if (currentNode == 0)
                        nextnode = nodes.Count - 1;
                    else
                        nextnode = currentNode - 1;
                    if (Vector2.Distance(CurrentPos, nodes[currentNode].GetComponent<Transform>().position) < Vector2.Distance(CurrentPos, nodes[currentNode].GetComponent<Transform>().position))
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
        //front right angle sensor
        if (Physics.Raycast(sensorStartPos, Quaternion.Euler(0, -frontSensorAngle, 0) * GetComponent<Transform>().forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoid = true;
                avoidMultiplier -= 0.5f;
                if (hit.distance <= 0.5f && !isBraking && !backward)
                {
                    Vector3 CurrentPos = GetComponent<Transform>().position;
                    int nextnode;
                    if (currentNode == 0)
                        nextnode = nodes.Count - 1;
                    else
                        nextnode = currentNode - 1;
                    if (Vector2.Distance(CurrentPos, nodes[currentNode].GetComponent<Transform>().position) < Vector2.Distance(CurrentPos, nodes[currentNode].GetComponent<Transform>().position))
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
    protected override void DetectFromBackRightSide()
    {
        return;
    }
    protected override void DetectFromBackLeftSide()
    {
        return;
    }
    protected override void DetectFromFrontRightSide()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = m_Wheels[0].GetComponent<Transform>().position;
        sensorStartPos += GetComponent<Transform>().forward * frontSensorPosition.z;
        sensorStartPos += GetComponent<Transform>().up * frontSensorPosition.y;
        sensorStartPos += GetComponent<Transform>().right * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, GetComponent<Transform>().right, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoid = true;
                avoidMultiplier += 1f;
                if (hit.distance <= 0.5f && !isBraking && !backward)
                {
                    Vector3 CurrentPos = GetComponent<Transform>().position;
                    int nextnode;
                    if (currentNode == 0)
                        nextnode = nodes.Count - 1;
                    else
                        nextnode = currentNode - 1;
                    if (Vector2.Distance(CurrentPos, nodes[currentNode].GetComponent<Transform>().position) < Vector2.Distance(CurrentPos, nodes[currentNode].GetComponent<Transform>().position))
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
        //front right angle sensor
        if (Physics.Raycast(sensorStartPos, Quaternion.Euler(0, frontSensorAngle, 0) * GetComponent<Transform>().forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoid = true;
                avoidMultiplier += 0.5f;
                if (hit.distance <= 0.5f && !isBraking && !backward)
                {
                    Vector3 CurrentPos = GetComponent<Transform>().position;
                    int nextnode;
                    if (currentNode == 0)
                        nextnode = nodes.Count - 1;
                    else
                        nextnode = currentNode - 1;
                    if (Vector2.Distance(CurrentPos, nodes[currentNode].GetComponent<Transform>().position) < Vector2.Distance(CurrentPos, nodes[currentNode].GetComponent<Transform>().position))
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
    void UseItem()
    {
        if (GetComponent<PickUpController>().currentPickup != null)
            GetComponent<PickUpController>().Use();
    }
}