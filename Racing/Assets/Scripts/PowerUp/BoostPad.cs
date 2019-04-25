using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad : MonoBehaviour
{
    public float bustForce = 500.0f;
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
        Rigidbody rb = other.transform.root.GetComponent<Rigidbody>();

        if (rb)
        {
           // rb.AddForce(transform.forward * bustForce, ForceMode.Impulse);
            Debug.Log("Boost Applied");
        }
    }
}
