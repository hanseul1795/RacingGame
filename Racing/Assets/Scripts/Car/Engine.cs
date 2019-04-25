using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public GameObject transmissionIndicator;
    public GameObject thruster;


    private Rigidbody rb;

    private Vector3 acceleration;
    private float nextLimit = 10.0f;
    private float previousLimit;


    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        previousLimit = nextLimit;
        //lastVelocity = rb.velocity.magnitude;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        acceleration = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        SimulateTransmission();

        thruster.SetActive(acceleration.magnitude >= 60.0f);
    }

    private void SimulateTransmission()
    {
        if (!transmissionIndicator) return;

        //Debug.Log("A: " + acceleration.magnitude + " V: " + nextLimit + " P: " + previousLimit);

        if (acceleration.magnitude > nextLimit)
        {
            previousLimit = nextLimit;
            nextLimit += nextLimit * 1.5f;
            transmissionIndicator.SetActive(true);
        }

        if (acceleration.magnitude <= previousLimit)
        {
            nextLimit = previousLimit;
            previousLimit -= previousLimit * 1.5f;
        }

        if (acceleration.magnitude <= 10.0f)
        {
            nextLimit = 10.0f;
            previousLimit = nextLimit;
        }
    }
}
