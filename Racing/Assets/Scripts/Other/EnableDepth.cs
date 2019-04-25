using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EnableDepth : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
    }
}
