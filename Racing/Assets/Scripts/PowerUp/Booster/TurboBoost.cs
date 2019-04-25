using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurboBoost : APickable
{
    public float        turboPower     = 500.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        effectPrefab = Instantiate(Resources.Load<GameObject>("TurboBoost"));
        type = PickupType.BACKUSE;

        effectPrefab.transform.position = transform.Find("BackPickup").position;
        effectPrefab.transform.rotation = transform.Find("BackPickup").rotation;
        effectPrefab.transform.parent   = transform.Find("BackPickup").transform;

        activeDuration = 3.0f;

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
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * turboPower, ForceMode.Impulse);
            if (effectPrefab)
                effectPrefab.SetActive(true);
        }
        else
        {
            if (effectPrefab)
                effectPrefab.SetActive(false);
        }
    }
}
