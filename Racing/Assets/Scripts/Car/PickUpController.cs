using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public APickable currentPickup;
    public GameObject pickupSlot;
    private GameObject[] pickupUIs;

    // Start is called before the first frame update
    void Start()
    {
        if(pickupSlot)
        {
            pickupUIs = new GameObject[pickupSlot.transform.childCount];

            for (int i = 0; i < pickupSlot.transform.childCount; ++i)
            {
                pickupUIs[i] = pickupSlot.transform.GetChild(i).gameObject;
            }
        }

        HidePickupUI();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePickupSlot();
        //Use();
        CheckIfReadyToDestroy();

    }

    private void HidePickupUI()
    {
        if (!pickupSlot) return;

        for(int i = 0; i < pickupSlot.transform.childCount; ++i)
        {
            GameObject obj = pickupSlot.transform.GetChild(i).gameObject;
            
            if(obj)
            {
                if (obj.activeSelf)
                    obj.SetActive(false);
            }
        }
    }

    private void UpdatePickupSlot()
    {
        if(currentPickup)
        {
            if(currentPickup.GetType() == typeof(TurboBoost))
            {
                GameObject speed = Array.Find<GameObject>(pickupUIs, x =>  x.name == "Speed");

                if (speed)
                {
                    if(!speed.activeSelf)
                        speed.SetActive(true);
                }
                Debug.Log("Turbo Acquired");
            }
            else if(currentPickup.GetType() == typeof(Laser))
            {
                //Debug.Log("Laser Acquired");
            }
        }
        else
        {
            HidePickupUI();
        }
    }

    public void Use()
    {
        //if (Input.GetButton("Fire1"))
        //{
            if (currentPickup)
            {
                if(!currentPickup.used)
                    if(pickupSlot)
                        pickupSlot.GetComponent<Animator>().SetTrigger("Use");

                currentPickup.Use();
                Debug.Log("Using Pickup");
            }
            else
                Debug.Log("No Pickup Available");
        //}
    }

    private void CheckIfReadyToDestroy()
    {
        if (currentPickup)
        {
            if (currentPickup.readyToDestroy)
            {
                Destroy(currentPickup.effectPrefab);
                Destroy(currentPickup);
                currentPickup = null;
                HidePickupUI();
            }
        }

    }
}
