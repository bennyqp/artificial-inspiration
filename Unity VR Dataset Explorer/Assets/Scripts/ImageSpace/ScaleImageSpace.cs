using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleImageSpace : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void scaleImageSpace(float scaleFactor, float scaleYFactor)
    {
        transform.localScale = new Vector3(scaleFactor, scaleYFactor, scaleFactor);
        
        foreach (Transform child in transform)
        {
            child.transform.localScale = new Vector3(1 / scaleFactor, 1 / scaleYFactor, 1 / scaleFactor);
        }
        
    }

    //Calculate Position in WoldSpace Scale
    public Vector3 getGlobalPosition(Vector3 woldSpaceVec)
    {
        float scale = transform.localScale.x;
        float scaleY = transform.localScale.y;

        float newXpos = woldSpaceVec.x * scale;
        float newYpos = woldSpaceVec.y * scaleY;
        float newZpos = woldSpaceVec.z * scale;

        woldSpaceVec = new Vector3(newXpos, newYpos, newZpos);
        return woldSpaceVec;
    }

    public float getLocalYpos(float yPos)
    {
        float scaleY = transform.localScale.y;
        float newYpos = yPos * scaleY;
        return newYpos;
    }
}
