using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartImageScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Start Image Animations

    //Fade-Out Animtionen and Disable
    public void fadeOutAndDisable(float animationTime)
    {
        if (this.gameObject.activeSelf == true)
        {
            LeanTween.value(gameObject, updateColorAlpha, 255, 0, animationTime).setOnComplete(disableThisImage);
        }
    }
    void disableThisImage()
    {
        gameObject.SetActive(false);
    }
    void updateColorAlpha(float alpha)
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, (byte)alpha);
    }
}
