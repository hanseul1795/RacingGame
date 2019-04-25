using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointSystem : MonoBehaviour
{
    public Waypoint[] points;
    void Start()
    {
        points = gameObject.GetComponentsInChildren<Waypoint>();
        for(int i = 0; i < points.Length; ++i)
        {
            points[i].id = i;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        for (int i = 0; i < points.Length; ++i)
        {
            int next = (i + 1) % points.Length;
            Gizmos.DrawLine(points[i].transform.position, points[next].transform.position);
        }
    }
}
