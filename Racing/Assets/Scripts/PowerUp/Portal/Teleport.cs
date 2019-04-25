using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform endpoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        //other.transform.parent.position = endpoint.position;
        //other.transform.parent.rotation = endpoint.rotation;

        Rigidbody rb = other.GetComponentInParent<Rigidbody>();
        if(rb)
        {
            rb.position = endpoint.position;
            //rb.velocity = endpoint.rotation * rb.velocity;
            rb.rotation = endpoint.rotation;
            Vector3 vel = rb.velocity;
            rb.velocity = Vector3.zero;
            rb.AddForce(endpoint.forward * vel.magnitude, ForceMode.VelocityChange);
        }
    }
}
