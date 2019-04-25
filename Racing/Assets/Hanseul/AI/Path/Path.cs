using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Color lineColor;
    public List<CheckPoint> checkPoints = new List<CheckPoint>();

    void Start()
    {
        CheckPoint[] array = FindObjectsOfType<CheckPoint>();
        int count = array.Length;
        for (int i = 0; i < count; ++i)
        {
            checkPoints.Add(array[i]);
        }
    }
    
    void Update()
    {

    }
    public void OnDrawGizmos()
    {
        int CheckPointCount = checkPoints.Count;

        Gizmos.color = lineColor;
        for (int i = 0; i < CheckPointCount; ++i)
        {
            Vector3 CurrentPos = checkPoints[i].GetComponent<Transform>().position;
            Vector3 Prev = Vector3.zero;

            if (i > 0)
            {
                Prev = checkPoints[i - 1].GetComponent<Transform>().position;
            }
            else if (i == 0 && CheckPointCount > 1)
            {
                Prev = checkPoints[CheckPointCount - 1].GetComponent<Transform>().position;
            }
            Gizmos.DrawLine(Prev, CurrentPos);
            checkPoints[i].OnDrawGizmos();
        }
    }
}