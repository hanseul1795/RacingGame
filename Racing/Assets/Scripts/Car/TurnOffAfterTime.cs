using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffAfterTime : MonoBehaviour
{
    public float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        StartCoroutine(TurnOff(timer));
    }

    IEnumerator TurnOff(float p_timer)
    {
        yield return new WaitForSeconds(p_timer);
        gameObject.SetActive(false);
    }
}
