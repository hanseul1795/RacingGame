using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressAnyButton : MonoBehaviour
{
    AsyncOperation loadingLevel;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("load");
        
    }

    IEnumerator load()
    {
        loadingLevel = SceneManager.LoadSceneAsync(1);
        loadingLevel.allowSceneActivation = false;
        yield return loadingLevel;
    }
        // Update is called once per frame
        void Update()
    {
        if(Input.anyKey)
        {
            loadingLevel.allowSceneActivation = true;
        }
    }
}
