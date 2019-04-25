using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : Singleton<RespawnManager>
{

    public void Respawn(ref GameObject carToRespawn)
    {
        Vector3 currentPosition;
        Vector3 nextPosition;
        Vector3 posParent;
        if (carToRespawn.GetComponent<CarController>() != null)
        {
            int currentCheckPoint = carToRespawn.GetComponentInChildren<CarCheckpoint>().currentCheckpoint;
            int currentLap = carToRespawn.GetComponentInChildren<CarCheckpoint>().currentLap;
            WaypointSystem ws = GameManager.Instance.waypointSystems[currentLap];
            nextPosition = ws.points[currentCheckPoint].GetComponent<Transform>().position;
            if (currentCheckPoint == 0)
                currentPosition = ws.points[ws.points.Length - 1].GetComponent<Transform>().position;
            else
                currentPosition = ws.points[currentCheckPoint - 1].GetComponent<Transform>().position;

            Quaternion y = ws.points[ws.points.Length - 1].GetComponent<Transform>().rotation;   
            carToRespawn.GetComponent<Transform>().SetPositionAndRotation(currentPosition + new Vector3(0, 5, 0),  y);
        }
        else if (carToRespawn.GetComponent<AggresifAI>() != null)
        {
            posParent = carToRespawn.GetComponent<AggresifAI>().path.GetComponent<Transform>().position;
            CheckPoint currentCheck;
            int currentCheckPoint = carToRespawn.GetComponent<AggresifAI>().currentNode;
            int nextNode = 0;
            if (carToRespawn.GetComponent<AggresifAI>().CurrentLap < 3)
            {
                if (currentCheckPoint == 0)
                    nextNode = carToRespawn.GetComponent<AggresifAI>().nodes.Count - 1;
                else
                    nextNode = currentCheckPoint - 1;
            }
            else
            {
                if (currentCheckPoint == 0)
                    nextNode = carToRespawn.GetComponent<AggresifAI>().lastLapPoints.Count - 1;
                else
                    nextNode = currentCheckPoint - 1;
            }
            if (carToRespawn.GetComponent<AggresifAI>().CurrentLap < 3)
            {
                currentPosition = carToRespawn.GetComponent<AggresifAI>().nodes[currentCheckPoint].GetComponent<Transform>().position;
                currentCheck = carToRespawn.GetComponent<AggresifAI>().nodes[currentCheckPoint];
                nextPosition = carToRespawn.GetComponent<AggresifAI>().nodes[nextNode].GetComponent<Transform>().position;
            }
            else
            {
                currentPosition = carToRespawn.GetComponent<AggresifAI>().lastLapPoints[currentCheckPoint].GetComponent<Transform>().position;
                currentCheck = carToRespawn.GetComponent<AggresifAI>().lastLapPoints[currentCheckPoint];
                nextPosition = carToRespawn.GetComponent<AggresifAI>().lastLapPoints[nextNode].GetComponent<Transform>().position;
            }
            Vector3 CurrentUp = carToRespawn.GetComponent<Transform>().up;
            Quaternion y = currentCheck.GetComponent<Transform>().rotation;
            carToRespawn.GetComponent<Transform>().SetPositionAndRotation(currentPosition + new Vector3(0, 5, 0), y);
        }
    }
}
