using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public WaypointSystem[] waypointSystems;

    public int maxLaps;
    public bool raceStart = false;

    public GameObject raceEnd;
    private GameObject[] cars;
    private bool finish;
    private bool slowmo = false;

    public AnimationCurve shotCurve;

    private void Start()
    {
        cars = GameObject.FindGameObjectsWithTag("Car");
        finish = false;
    }

    private void Update()
    {
        foreach (GameObject car in cars)
        {
            CarCheckpoint checkPoint = car.GetComponentInChildren<CarCheckpoint>();
            if (checkPoint)
                if (checkPoint.currentLap > maxLaps)
                {
                    finish = true;

                    if(slowmo)
                        car.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
                    else
                        car.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;

                    //CarController cc = car.GetComponent<CarController>();
                    //if (cc) cc.enabled = false;
                    //Debug.Log("Race Finished");
                }
        }

        if (finish)
        {
            if (!raceEnd.activeSelf)
                StartCoroutine(FinishPicture());
                //raceEnd.SetActive(true);

            //Time.timeScale = 0.1f;
        }
    }

    IEnumerator FinishPicture()
    {
        raceEnd.SetActive(true);

        float t = 0;

        Time.timeScale = 0.5f;
        slowmo = true;

        Keyframe lastframe = shotCurve[shotCurve.length - 1];
        while (Time.timeScale < lastframe.value)
        {
            t += TimeController.deltaTime;
            Time.timeScale = shotCurve.Evaluate(t);

            yield return null;
        }
        slowmo = false;

        yield return new WaitForSeconds(0.3f);


        yield return null;

    }
}
