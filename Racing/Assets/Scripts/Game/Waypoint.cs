using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public int id;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Car"))
        {
            if(other.transform.root.GetComponentInChildren<ACarCheckpoint>())
            {
                ACarCheckpoint[] cc = other.transform.root.GetComponentsInChildren<ACarCheckpoint>();
                foreach(ACarCheckpoint c in cc)
                {
                    c.Check(id);
                }
            }
        }
    }
}
