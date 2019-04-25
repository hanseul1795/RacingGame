using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    [SerializeField]
    private Transform cameraLookAtTarget;
    [SerializeField]
    private Transform cameraPosition;

    private Transform defaultPosition;
    public Transform[] cameraAltPosition;

    private Vector3 newCamPos;

    private void Start()
    {

    }

    private void Update()
    {
    }
}
