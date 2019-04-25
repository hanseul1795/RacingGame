using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupEvent : MonoBehaviour
{
    //public delegate void EventHandler();
    //public event EventHandler Pickup;

    public Transform floating;
    private float val = 0;

    // Start is called before the first frame update
    void Start()
    {
        //floating = transform.GetChild(0);
        val = Random.Range(1, 10);
    }

    // Update is called once per frame
    void Update()
    {
        val += Time.deltaTime;
        floating.localPosition = new Vector3(0.0f, (-(0.2f * Mathf.Sin(floating.localPosition.y + val))), 0.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        GetComponentInParent<PickupObject>().OnChildTriggerEnter(other);

        if (other.gameObject.CompareTag("Car"))
        {
            //if (Pickup != null)
            //    Pickup();
        }
    }
}
