using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinMessage : MonoBehaviour
{

    public Text text;

    private void OnEnable()
    {
        if (text)
            text.text = "Time: " + (Time.timeSinceLevelLoad/60.0f).ToString("0.00");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
