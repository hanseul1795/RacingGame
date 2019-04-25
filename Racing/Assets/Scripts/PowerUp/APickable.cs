using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType
{
    BACKUSE,
    FRONTUSE,
    NONE
};

abstract public class APickable : MonoBehaviour
{
    public      float activeDuration    =   1.0f;

    public      bool  used              =   false;
    protected   float timer             =   0.0f;

    public PickupType type              = PickupType.NONE;
    [HideInInspector]
    public bool       readyToDestroy    = false;  
    public GameObject effectPrefab;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (used) timer += Time.deltaTime;

        if (timer >= activeDuration)
        {
            readyToDestroy = true;
        }
    }

    public void Activate()
    {
        used = false;
        timer = 0.0f;
    }

    public abstract void Use();
}
