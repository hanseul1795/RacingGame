using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartCounter : MonoBehaviour
{
    //public TextMeshProUGUI counterText;
    public GameObject startTimer;
    public float speed;

    private TextMeshProUGUI counterText;

    // Start is called before the first frame update
    void Start()
    {
        if (!counterText)
            counterText = startTimer.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(counterText)
            counterText.fontSharedMaterial.SetFloat("_GlowOffset", Mathf.Max(0.0f,Mathf.Sin(speed * Time.time)));
    }

    private void OnEnable()
    {
        if(!counterText)
            counterText = startTimer.GetComponentInChildren<TextMeshProUGUI>();

        if (counterText)
            StartCoroutine(CountDown());
    }

    private void OnDisable()
    {
        if (counterText)
            counterText.fontSharedMaterial.SetFloat("_GlowOffset", 1.0f);
    }

    IEnumerator CountDown()
    {
        for(int i = 3; i > 0; --i)
        {
            counterText.text = i.ToString();
            yield return new WaitForSeconds(1.0f);
        }
        counterText.text = "GO";
        GameManager.Instance.raceStart = true;
        yield return new WaitForSeconds(5.0f);
        //gameObject.SetActive(false);
        startTimer.SetActive(false);
    }
}
