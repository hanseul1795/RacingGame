using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //public KeyCode switchViewKey    = KeyCode.Space;

    public float smoothing          = 6f;
    public Transform lookAtTarget;
    public Transform positionTarget;
    public Transform sideView;

    public float     rayDistance    = 1.0f;
    public LayerMask rayMask;
    public Transform rayOrigin;
    private RaycastHit hit;

    private Vector3 worldUp;
    private Vector3 nextUp;

    bool m_ShowingSideView;

    private void Start()
    {
    }
    private void FixedUpdate()
    {
        UpdateCamera();
    }

    private void Update()
    {
        if (Input.GetButtonDown("SwitchView"))
            m_ShowingSideView = !m_ShowingSideView;
    }

    private void UpdateCamera()
    {
        if (rayOrigin)
        {
            if (Physics.Raycast(rayOrigin.position, -rayOrigin.up, out hit, rayDistance, rayMask))
            {
                nextUp = hit.normal;
            }
            else
            {
                nextUp = Vector3.up;
            }
        }
        else
        {
            nextUp = Vector3.up;
        }

        worldUp = Vector3.Lerp(worldUp, nextUp, 2.0f * Time.deltaTime);

        if (m_ShowingSideView)
        {
            transform.position = sideView.position;
            transform.rotation = sideView.rotation;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, positionTarget.position, Time.deltaTime * smoothing);
            transform.LookAt(lookAtTarget, worldUp);
        }
    }
}
