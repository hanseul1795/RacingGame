using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarCheckpoint : ACarCheckpoint
{
    public TextMeshProUGUI LapText;

    public string fillUpText        = "Lap ";
    public bool showMaxLaps         = true;

    public UIManager man;

    public TunnelDoor tunnel;

    public void Awake()
    {
    }

    public override void Check(int p_id)
    {
        WaypointSystem ws;
        if (GameManager.Instance.waypointSystems.Length > 1)
            ws = GameManager.Instance.waypointSystems[Mathf.Min(currentLap, GameManager.Instance.waypointSystems.Length-1)];
        else
            ws = GameManager.Instance.waypointSystems[0];

        if (p_id == currentCheckpoint)
        {
            if(currentCheckpoint >= ws.points.Length-1)
            {
                currentLap++;
                currentCheckpoint = 0;
                if (man)
                {
                    if (currentLap < GameManager.Instance.maxLaps)
                        man.Activate(currentLap.ToString());
                    else
                        man.Activate("Final");
                }
            }
            else
                currentCheckpoint++;

            Debug.Log(gameObject.name);
        }
    }

    private void Update()
    {
        if (currentLap < GameManager.Instance.maxLaps)
        {
            if (showMaxLaps)
                LapText.text = fillUpText + currentLap + "/" + GameManager.Instance.maxLaps;
            else
                LapText.text = fillUpText + currentLap;
        }
        else
        {
            LapText.text = "Final";
            tunnel.closed = false;
        }
    }


}
