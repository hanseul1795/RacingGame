using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Renderer toTile;
    public Vector2 tileSpeed;
    private Vector2 tile;

    private Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = toTile.material;
    }

    // Update is called once per frame
    void Update()
    {
        tile += tileSpeed * Time.deltaTime;
        mat.SetTextureOffset("_MainTex", tile);
    }
}
