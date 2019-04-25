using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACarCheckpoint : MonoBehaviour
{
    public int      currentCheckpoint     = 0;
    public int      currentLap            = 0;

    public virtual void Check(int p_id)
    {
        WaypointSystem ws;
        if (GameManager.Instance.waypointSystems.Length > 1)
            ws = GameManager.Instance.waypointSystems[currentLap];
        else
            ws = GameManager.Instance.waypointSystems[0];

        if (p_id == currentCheckpoint)
        {
            if (currentCheckpoint >= ws.points.Length - 1)
            {
                currentLap++;
                currentCheckpoint = 0;
            }
            else
                currentCheckpoint++;
        }
    }
}
