using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : APickable
{

    // Start is called before the first frame update
    void Start()
    {
        effectPrefab    = Instantiate(Resources.Load<GameObject>("CarLaser"));
        type            = PickupType.FRONTUSE;

        effectPrefab.transform.position = transform.Find("FrontPickup").position;
        effectPrefab.transform.rotation = transform.Find("FrontPickup").rotation;
        effectPrefab.transform.parent   = transform.Find("FrontPickup").transform;
        effectPrefab.SetActive(false);

        activeDuration = 0.5f;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void Use()
    {
        used = true;
        if (timer < activeDuration)
        {
            if (!effectPrefab.activeSelf)
            {
                effectPrefab.SetActive(true);
                GameObject laser = Instantiate(Resources.Load<GameObject>("Laser"));
                laser.transform.position = transform.Find("FrontPickup").position;
                laser.transform.rotation = transform.Find("FrontPickup").rotation;
                laser.GetComponent<LaserMove>().SetOwner(gameObject);
            }
        }
    }
}
