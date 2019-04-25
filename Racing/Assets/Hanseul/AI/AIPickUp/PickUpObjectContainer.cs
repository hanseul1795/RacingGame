using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObjectContainer : MonoBehaviour
{
    private int currentNode = 0;
    private bool haveTarget = false;
    public List<PickupObject> powerUps = new List<PickupObject>();
    void Start()
    {
        PickupObject[] powerUpArray = GetComponentsInChildren<PickupObject>();
        for(int i =0; i < powerUpArray.Length; ++i)
        {
            powerUps.Add(powerUpArray[i]);
        }
    }

    public Transform UpdateAvailablePowerUp()
    {        
        Transform powerUpLocation = null;
        if (haveTarget)
        {
            if (powerUps[currentNode].ready)
                powerUpLocation = powerUps[currentNode].GetComponent<Transform>();
            else
            {          
                for (int i = 0; i < powerUps.Count; ++i)
                {
                    if (powerUps[i].ready)
                    {
                        powerUpLocation = powerUps[i].GetComponent<Transform>();
                        currentNode = i;
                    }
                }
                if (powerUpLocation != null)
                    haveTarget = true;
            }
        }
        else
        {
            for (int i = 0; i < powerUps.Count; ++i)
            {
                if (powerUps[i].ready)
                {
                    powerUpLocation = powerUps[i].GetComponent<Transform>();
                    currentNode = i;
                }
            }
            if (powerUpLocation != null)
                haveTarget = true;
        }
        return powerUpLocation;
    }
}