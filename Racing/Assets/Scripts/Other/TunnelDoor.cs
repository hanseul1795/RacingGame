using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TunnelDoor : MonoBehaviour
{
    public GameObject door;
    public GameObject[] collider;

    [ColorUsageAttribute(true, true)]
    public Color closedColor;
    [ColorUsageAttribute(true, true)]
    public Color openColor;

    public TextMeshProUGUI text;

    public bool closed = true;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (closed)
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_MainColor", closedColor);
            door.GetComponent<Renderer>().material.SetColor("_MainColor", closedColor);
            text.text = "Closed";

            foreach (GameObject c in collider)
            {
                if (!c.activeSelf)
                {
                    c.SetActive(true);
                }
            }
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_MainColor", openColor);
            door.GetComponent<Renderer>().material.SetColor("_MainColor", openColor);
            text.text = "Open";

            foreach (GameObject c in collider)
            {
                if (c.activeSelf)
                {
                    c.SetActive(false);
                }
            }
        }
    }
}
