using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TextureScaler : MonoBehaviour
{
    Renderer rend;
    public float textureSize;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float scaleX = GetComponent<Transform>().localScale.x / textureSize;
        float scaleY = GetComponent<Transform>().localScale.y / textureSize;

        rend.material.mainTextureScale = new Vector2(scaleX, scaleY);
    }
}
