using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingSystem : MonoBehaviour
{
    private GameObject[] cars;
    void Start()
    {
        cars = GameObject.FindGameObjectsWithTag("Car");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
