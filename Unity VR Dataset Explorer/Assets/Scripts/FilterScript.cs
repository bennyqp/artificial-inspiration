using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterScript : MonoBehaviour
{
    private GameObject player;

    public List<Color32> filterColors;
    public int colorFilterThreshold;
    public bool activateColorFilter;

    public float abstractLevel;
    public float abstractThreshold;
    public bool activateAbstractFilter = false;

    public float filter2Level;
    public float filter2Threshold;
    public bool filter2activated;

    public bool letsFilterBool = false;
    public bool letDisableFilter = false;
    public bool sortColors = false;

    public LoadData loadData;

    private float maxDist;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("MainCamera");

        maxDist = Vector3.Distance(new Vector3(0, 0, 0), new Vector3(255, 255, 255));
    }

    public void startFilter()
    {
        loadData = GetComponent<LoadData>();
        Debug.Log("Filter ready");
    }

    public void disableAllFilters()
    {
        
        foreach (ActiveImage thisImage in Data.activeImageObjs)
        {
            ImgScript thisImgScript = thisImage.image.GetComponent<ImgScript>();
            thisImgScript.images2tsneSpace(3);
        }
        
        if(Data.activeImageObjs.Count <= Data.maxImages)
        {
            int i = 0;
            while (Data.activeImageObjs.Count < Data.maxImages)
            {
                ActiveImage thisImage = Data.allImageObjs[i];
                if (thisImage.image.activeSelf == false)
                {
                    Data.activeImageObjs.Add(thisImage);
                    ImgScript thisImgScript = thisImage.image.GetComponent<ImgScript>();
                    thisImgScript.enableAndFadeIn(3);
                    thisImgScript.images2tsneSpace(3);
                }
                i = i + 1;
            }
        }
    }

    public void letsFilter()
    {
        //Create list of image objects to be filtered
        List<Quest> images2filter = new List<Quest>(loadData.quests);

        //Reset distance to player for all images
        foreach (Quest image in images2filter)
        {
            image.playerDist = 0;
        }

        //Filter counter to divide the player distance at the end by this
        int filterCount = 0;

        //COLOR FILTER
        if (activateColorFilter ==  true)
        {
            images2filter = colorFilter(images2filter);
            filterCount += 1;
        }

        //ABSTRACT-LEVEL FILTER
        if(activateAbstractFilter == true)
        {
            images2filter = abstractFilter(images2filter);
            filterCount += 1;
        }

        //FILTER2 – Here the composition filter 
        if (filter2activated == true)
        {
            images2filter = filter2(images2filter);
            filterCount += 1;
        }

        //Create dictionary with matching values
        Dictionary<string, float> filteredImgs = new Dictionary<string, float>();
        foreach (Quest thisFilteredImage in images2filter)
        {
            float finalPlayerDistance = 100;
            if (filterCount != 0)
            {
                finalPlayerDistance = thisFilteredImage.playerDist / filterCount;
            }
            filteredImgs.Add(thisFilteredImage.path, finalPlayerDistance);
        }


        //Show/hide images and move to new position 
        if (filterCount > 0)
        {
            LeanTween.cancelAll(true);
            Data.activeImageObjs.Clear();
            foreach (ActiveImage thisImage in Data.allImageObjs)
            {
                string thisImagePath = thisImage.image.GetComponent<ImgScript>().path;
                if (filteredImgs.ContainsKey(thisImagePath))
                {
                    if (Data.activeImageObjs.Count < Data.maxImages)
                    {
                        Data.activeImageObjs.Add(thisImage);
                        thisImage.image.GetComponent<ImgScript>().enableAndFadeIn(1);
                        Vector3 filteredPos = player.transform.position + Vector3.Normalize(thisImage.image.transform.position - player.transform.position) * (filteredImgs[thisImagePath] + 10);
                        thisImage.image.GetComponent<ImgScript>().moveImageToPos(filteredPos, 2);
                    }
                }
                else
                {
                    if (thisImage.image.activeSelf == true)
                    {
                        thisImage.image.GetComponent<ImgScript>().fadeOutAndDisable(1);
                    }
                }
            }
        } else
        {
            Camera.main.GetComponent<ShowInfo>().sendInfo("No Filter activated.", 2f, 10);
        }
    }

    private List<Quest> filter2(List<Quest> images2filter)
    {
        List<Quest> filteredImages = new List<Quest>();
        foreach (Quest img in images2filter)
        {
            float unmapedFilter2Dist = Mathf.Abs(filter2Level * 100 - img.composition);
            float filter2Dist = map(unmapedFilter2Dist, 0f, 100f, 0f, maxDist);

            if (filter2Dist >= filter2Level - filter2Threshold && filter2Dist <= filter2Level + filter2Threshold)
            {
                img.playerDist = img.playerDist + filter2Dist;
                filteredImages.Add(img);
            }
        }
        return filteredImages;
    }

    private List<Quest> abstractFilter(List <Quest> images2filter)
    {
        List<Quest> filteredImages = new List<Quest>();
        foreach (Quest img in images2filter)
        {
            float unmapedAbstractDist = Mathf.Abs(abstractLevel - img.abstractLevel);
            float abstractDist = map(unmapedAbstractDist, 0f, 1, 0f, maxDist);

            if (abstractDist >= abstractLevel - abstractThreshold && abstractDist <= abstractLevel + abstractThreshold)
            {
                if (abstractLevel == 0 && abstractDist <= 1)
                {
                    abstractDist = Random.Range(0.0f, 200.0f);
                }
                img.playerDist = img.playerDist + abstractDist;
                filteredImages.Add(img);
            }
        }
        return filteredImages;
        
    }

    private List<Quest> colorFilter(List <Quest> images2filter)
    {
        for (int i = 0; i < filterColors.Count; i++)
        {
            Vector3 filterColorVector = new Vector3(filterColors[i].r, filterColors[i].g, filterColors[i].b);
            List<Quest> filteredImages = new List<Quest>();
            foreach (Quest img in images2filter)
            {
                float minColorDistance = 9999999999999999999;
                bool addToList = false;
                float thisColorPercent = 0;
                float thisFinalColorDist = 0;

                for (int e = 0; e < img.colors.Length; e++)
                {
                    Vector3 colorVector = new Vector3(img.colors[e].r, img.colors[e].g, img.colors[e].b);
                    float thisColorDistance = Vector3.Distance(filterColorVector, colorVector);
                    if (thisColorDistance < colorFilterThreshold && thisColorDistance < minColorDistance)
                    {
                        addToList = true;
                        minColorDistance = thisColorDistance;
                        thisColorPercent = img.imageColPercent[e];
                    }
                }
                if (addToList == true)
                {
                    filteredImages.Add(img);
                    thisFinalColorDist += minColorDistance / thisColorPercent / 10;

                    if (i == filterColors.Count - 1)
                    {
                        img.playerDist = img.playerDist + thisFinalColorDist / filterColors.Count;
                    }
                }
            }
            images2filter = filteredImages;
        }
        return images2filter;
    }

    public void sortImagesByColor()
    {
        LeanTween.cancelAll(true);
        foreach (ActiveImage activeImage in Data.activeImageObjs)
        {
            ImgScript thisImgScript = activeImage.image.GetComponent<ImgScript>();
            thisImgScript.colorSortAnimation(3);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(letsFilterBool == true)
        {
            letsFilter();
            letsFilterBool = false;
        }
        if(letDisableFilter == true)
        {
            disableAllFilters();
            letDisableFilter = false;
        }
        if (sortColors == true)
        {
            sortImagesByColor();
            sortColors = false;
        }
    }

    //map function
    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
