﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject LapCounter;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate(string p_text)
    {
        if (LapCounter)
        {
            LapCounter.GetComponent<TMPro.TextMeshProUGUI>().text = p_text;
            LapCounter.SetActive(true);
        }
    }
}
