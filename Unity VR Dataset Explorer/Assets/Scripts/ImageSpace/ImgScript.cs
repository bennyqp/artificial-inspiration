using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImgScript : MonoBehaviour
{
    float newXpos, newYpos, newZpos;

    public GameObject globalScripts;
    public FilterScript filterScript;
    public ShowSelected showSelected;

    public float tsneX, tsneY, tsneZ;
    public string path;
    public string id;
    public Color32[] colors;
    public float[] imageColPercent;
    public float abstractLevel;
    public float composition;

    public SpriteRenderer thisImg;

    private GameObject animationCurvesObj;
    BennysAnimationCurves animationCurves;

    public bool showSelectedOn;

    // Start is called before the first frame update
    void Awake()
    {
        globalScripts = GameObject.FindGameObjectWithTag("GlobalScripts");
        filterScript = globalScripts.GetComponent<FilterScript>();
        showSelected = globalScripts.GetComponent<ShowSelected>();
        showSelectedOn = false;

        thisImg = GetComponent<SpriteRenderer>();

        animationCurvesObj = GameObject.FindGameObjectWithTag("animationCurveObj");
        animationCurves = animationCurvesObj.GetComponent<BennysAnimationCurves>();
    }

    // Update is called once per frame
    void Update()
    {
        if(showSelectedOn == false)
        {
            transform.LookAt(Camera.main.transform.position);
        }

    }

    

    //----------------------------------------------------------------Animations----------------------------------------------------------------

    public void images2tsneSpace(float animationTime)
    {
        Vector3 imgTsnePos = new Vector3(tsneX, tsneY, tsneZ);
        LeanTween.moveLocal(this.gameObject, imgTsnePos, animationTime).setEase(animationCurves.easeOut);
    }

    public void images2d(float animationTime)
    {
        Vector3 images2dPos = new Vector3(transform.position.x, 0, transform.position.z);
        LeanTween.move(this.gameObject, images2dPos, animationTime).setEase(animationCurves.easyEase);
    }

    public void moveImageToPos(Vector3 newPos, float animationTime)
    {
        LeanTween.move(this.gameObject, newPos, animationTime).setEase(animationCurves.easyEase);
    }

    public void moveImageToLocalPos(Vector3 newPos, float animationTime)
    {
        LeanTween.moveLocal(this.gameObject, newPos, animationTime).setEase(animationCurves.easeOut);
    }

    public void colorSortAnimation(float animationTime)
    {
        int biggestColor = 0;
        for (int i = 1; i <= imageColPercent.Length -1; i++)
        {
            if(imageColPercent[i] > imageColPercent[i-1])
            {
                biggestColor = i;
            }
        }

        float colSortXpos = (colors[biggestColor].r - 127.5f);
        float colSortYpos = (colors[biggestColor].g - 127.5f);
        float colSortZpos = (colors[biggestColor].b - 127.5f);
        Vector3 colorSortPos = new Vector3(colSortXpos, colSortYpos, colSortZpos);
        moveImageToPos(colorSortPos, animationTime);
    }


    //Fade-In and Fade-Out Animations with Enable / Disable 
    
    //Fade-Out Animtionen and Disable
    public void fadeOutAndDisable(float animationTime)
    {
        if (this.gameObject.activeSelf == true)
        {
            LeanTween.value(this.gameObject, updateColorAlpha, 255, 0, animationTime).setOnComplete(disableThisImage);
        } 
    }
    void disableThisImage()
    {
        this.gameObject.SetActive(false);
    }
    //Enable and Fage-In Animtionen
    public void enableAndFadeIn(float animationTime)
    {
        if (this.gameObject.activeSelf == false)
        {
            GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 0);
            this.gameObject.SetActive(true);
            fadeIn(animationTime);
        }
    }
    public void fadeIn(float animationTime)
    {
        LeanTween.value(this.gameObject, updateColorAlpha, 0, 255, animationTime);
    }
    //Update Color Function for both functions above
    void updateColorAlpha(float alpha)
    {
        GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, (byte)alpha);
    }

    //Start Animation
    public IEnumerator thisImgStartAnimation()
    {
        float randomStartAnimationTime = Random.Range(0.1f, 30.0f);

        //Quick Start?
        if (globalScripts.GetComponent<Constructor>().quickStart == true)
        {
            randomStartAnimationTime = 0.01f;
        }

        yield return new WaitForSeconds(randomStartAnimationTime);

        //Fade-In Image and move to StartPosition
        float randomImageStartAnimationTime = Random.Range(5.0f, 15.0f); //"Let it Rain" Ganz nice: (7.0f, 40.0f);

        //Quick Start?
        if (globalScripts.GetComponent<Constructor>().quickStart == true)
        {
            randomImageStartAnimationTime = 0.01f;
        }

        moveImageToLocalPos(new Vector3(tsneX, 0, tsneZ), randomImageStartAnimationTime);
        fadeIn(randomImageStartAnimationTime - randomImageStartAnimationTime / 3f);
    }

}
