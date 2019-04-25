using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    Color SphereColor;
    public float Radius;
    private void Start()
    {
        GetComponent<SphereCollider>().radius = Radius;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = SphereColor;
        Gizmos.DrawSphere(transform.position, Radius);
    }
}