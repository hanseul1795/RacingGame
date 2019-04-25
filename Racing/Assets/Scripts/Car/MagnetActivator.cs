using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetActivator : MonoBehaviour
{
    public bool activateMagnet = false;
    public bool overrideMagneticForce = false;
    public float magneticForce = 500.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Car"))
        {
            if (other.transform.root.GetComponent<CarController>())
            {
                CarController cc = other.transform.root.GetComponent<CarController>();
                cc.magnetic = activateMagnet;

                if (overrideMagneticForce)
                    cc.stickiness = magneticForce;

            }
        }
    }
}
