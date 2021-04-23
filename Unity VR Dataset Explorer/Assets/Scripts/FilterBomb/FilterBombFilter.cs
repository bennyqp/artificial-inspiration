using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterBombFilter : MonoBehaviour
{
    public bool enhancedClustering;
    public float clusteringFaktor;
    private float lastClusteringFaktor;
    public int clusterX;

    public List<GameObject> allFilterBombs;

    Constructor constructor; 

    private float maxDist;

    // Start is called before the first frame update
    void Start()
    {
        constructor = GetComponent<Constructor>();

        maxDist = Vector3.Distance(new Vector3(0, 0, 0), new Vector3(255, 255, 255));
        lastClusteringFaktor = 20;
        clusterX = 1;
    }

    // Update is called once per frame
    void Update()
    {

        //Start Filterbombs with Keyboard
        if (Input.GetKeyDown(KeyCode.J))
        {
            letsFilterBomb();
        }
        
    }
    
    public void letsFilterBomb()
    {
        //How many Bobs are active?
        int activeBombsNumber = 0;
        foreach (GameObject filterBombObj in allFilterBombs)
        {
            if (filterBombObj.GetComponent<FilterBomb>().ready2filter() == true || filterBombObj.GetComponent<FilterBomb>().checkpoint == true)
            {
                activeBombsNumber = activeBombsNumber + 1;
            }
        }

        clusterX = activeBombsNumber;
        if (enhancedClustering == true && activeBombsNumber == 2)
        {
            clusterX = 3;
            if (clusteringFaktor > 8)
            {
                enhancedClustering = false;
            }
            else
            {
                enhancedClustering = true;
            }
        }

        if(activeBombsNumber > 0)
        {
            //How many Images can one FilterBomb attract to not exceed MaxImages?
            int imagesPerFilterBomb = Mathf.RoundToInt(Data.maxImages / activeBombsNumber);

            //Check if ClusteringFaktor has changed
            if( lastClusteringFaktor != clusteringFaktor)
            {
                foreach (GameObject thisFilterBomb in allFilterBombs)
                {
                    thisFilterBomb.GetComponent<FilterBomb>().hasChanged = true;
                    lastClusteringFaktor = clusteringFaktor;
                }
            }

            //Clear Active Images List
            Data.activeImageObjs.Clear();
            foreach (GameObject thisFilterBomb in allFilterBombs)
            {
                if (thisFilterBomb.GetComponent<FilterBomb>().ready2filter() == true || thisFilterBomb.GetComponent<FilterBomb>().checkpoint == true)
                {
                    //Add Vectors for each filterBomb to each image 
                    addBombVecs(thisFilterBomb);

                    //Adding most fitting images for each filterBomb to activeImagesList
                    addMostRelevantImagesForThisBomb(thisFilterBomb, imagesPerFilterBomb);
                }
                    
            }

            //Fade-In all relevant Images and Fade-Out all the others
            foreach (ActiveImage thisImage in Data.allImageObjs)
            {
                if (Data.activeImageObjs.Contains(thisImage) == true)
                {
                    if (thisImage.image.activeSelf == false)
                    {
                        thisImage.image.GetComponent<ImgScript>().enableAndFadeIn(2);
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

            //Move Images to new position
            letsBombFilter();
        } else
        {
            Camera.main.GetComponent<ShowInfo>().sendInfo("No FilterBomb activated.", 2f, 10);
        }
    }

    public void addMostRelevantImagesForThisBomb(GameObject thisFilterBomb, int imagesPerFilterBomb)
    {
        int thisBombIndex = thisFilterBomb.GetComponent<FilterBomb>().filterBombIndex;

        Data.allImageObjs.Sort(delegate (ActiveImage a, ActiveImage b)
        {
            return (a.bombDists[thisBombIndex]).CompareTo(b.bombDists[thisBombIndex]);
        });
        int i = 0;
        int addedImages = 0;
        while (addedImages < imagesPerFilterBomb && i < Data.allImageObjs.Count)
        {
            if (Data.activeImageObjs.Contains(Data.allImageObjs[i]) == false)
            {
                Data.activeImageObjs.Add(Data.allImageObjs[i]);
                addedImages = addedImages + 1;
            }
            i = i + 1;
        }
    }

    public void addBombVecs(GameObject thisFilterBomb)
    {
        FilterBomb thisFilterBombScript = thisFilterBomb.GetComponent<FilterBomb>();

        List<Color32> filterBombColors = thisFilterBombScript.storedColors;
        bool filterBombColorActivation = thisFilterBombScript.storedColorActivation;

        float filterBombAbstractLevel = thisFilterBombScript.storedAbstractLevel;
        bool filterBombAbstractActivation = thisFilterBombScript.storedAbstractActivation;

        float filterBombFilter2Level = thisFilterBombScript.storedFilter2Level;
        bool filterBombFilter2Activation = thisFilterBombScript.storedFilter2Activation;

        thisFilterBombScript.checkChange();
        if (thisFilterBombScript.hasChanged == true)
        {
            bool thisBombAlreadyInVecList;
            //Check if this Filterbomb already has a Vector and an Distance in the List
            if (thisFilterBombScript.filterBombIndex < Data.allImageObjs[0].bombDists.Count)
            {
                thisBombAlreadyInVecList = true;
            }
            else
            {
                thisBombAlreadyInVecList = false;
            }


            foreach (ActiveImage thisImgObj in Data.allImageObjs)
            {

                float filterDistThisImg = 0;
                ImgScript thisImgScript = thisImgObj.image.GetComponent<ImgScript>();
                int filterCount = 0;


                //ColorFilter
                if (filterBombColorActivation == true)
                {
                    filterCount = filterCount + 1;
                    foreach (Color32 thisFilterBombColor in filterBombColors)
                    {
                        Vector3 filterBombColorVector = new Vector3(thisFilterBombColor.r, thisFilterBombColor.g, thisFilterBombColor.b);

                        float thisFilterBombColorDist = 9999999999999999999;
                        float thisColorPercent = 0;


                        for (int i = 0; i < thisImgScript.colors.Length; i++)
                        {
                            Color32 thisImgColor = thisImgScript.colors[i];
                            Vector3 thisImgColorVector = new Vector3(thisImgColor.r, thisImgColor.g, thisImgColor.b);
                            float thisColorDistance = Vector3.Distance(filterBombColorVector, thisImgColorVector);

                            if (thisColorDistance < thisFilterBombColorDist)
                            {
                                thisFilterBombColorDist = thisColorDistance;
                                thisColorPercent = thisImgScript.imageColPercent[i];
                            }


                        }

                        thisFilterBombColorDist = thisFilterBombColorDist / thisColorPercent / 10;

                        filterDistThisImg = filterDistThisImg + thisFilterBombColorDist;
                    }

                    filterDistThisImg = filterDistThisImg / filterBombColors.Count;
                }


                //AbstractFilter
                if (filterBombAbstractActivation == true)
                {
                    filterCount = filterCount + 1;

                    float unmapedAbstractDist = Mathf.Abs(filterBombAbstractLevel - thisImgScript.abstractLevel);
                    float abstractDist = map(unmapedAbstractDist, 0f, 1, 0f, maxDist);

                    if (abstractDist >= 1)
                    {
                        abstractDist = Random.Range(0.0f, 200.0f);
                    }

                    filterDistThisImg = filterDistThisImg + abstractDist;

                }

                //Filter2 – Composition filter
                if (filterBombFilter2Activation == true)
                {
                    filterCount = filterCount + 1;

                    float unmapedFilter2Dist = Mathf.Abs(filterBombFilter2Level * 100 - thisImgScript.composition);
                    float filter2Dist = map(unmapedFilter2Dist, 0f, 100f, 0f, maxDist);

                    filterDistThisImg = filterDistThisImg + filter2Dist;

                }


                //Divide final distance by the number of applied filters
                filterDistThisImg = filterDistThisImg / filterCount;
                //If the distance is 0 (so the image perfectly matches the FilterBomb settings), we need to make the distance very very small to avoid an error and make the impact for this filter very large
                if (filterDistThisImg == 0)
                {
                    filterDistThisImg = 0.1f;
                }


                //Calculate new image position for this filterbomb depending on the calculated distance

                //Get original position of the image
                Vector3 originalImagePos = new Vector3(thisImgScript.tsneX * constructor.posScale, thisImgScript.tsneY * constructor.yPosScale, thisImgScript.tsneZ * constructor.posScale);

                //Get new Vector for Image    
                Vector3 thisBombFilteredPos = thisFilterBomb.transform.position + Vector3.Normalize(originalImagePos - thisFilterBomb.transform.position) * (filterDistThisImg + 5);


                //Add vector and distance to object-array
                //If no cector and no distance exists for this FilterBomb
                if (thisBombAlreadyInVecList == false)
                {
                    thisImgObj.bombDists.Add(filterDistThisImg);
                    thisImgObj.bombVecs.Add(thisBombFilteredPos);
                }
                else //If there is already a vector and a distance for this FilterBomb override the old one
                {
                    thisImgObj.bombDists[thisFilterBombScript.filterBombIndex] = filterDistThisImg;
                    thisImgObj.bombVecs[thisFilterBombScript.filterBombIndex] = thisBombFilteredPos;
                }
            }

            //Set FilterBombIndex if not already exists
            if (thisBombAlreadyInVecList == false)
            {
                thisFilterBombScript.filterBombIndex = Data.allImageObjs[0].bombDists.Count - 1;
            }

            //Set the current FilterBomb settings to
            thisFilterBombScript.hasChanged = false;
            thisFilterBombScript.checkChangeCode = thisFilterBombScript.generatedChangeCode();
        }

    }



    public void letsBombFilter()
    {
        //check all active Filterbombs
        List<int> activeBombIndices = new List<int>(0);
        foreach (GameObject filterBomb in allFilterBombs)
        {
            FilterBomb filterBombScript = filterBomb.GetComponent<FilterBomb>();
            if (filterBombScript.ready2filter() == true)
            {
                activeBombIndices.Add(filterBombScript.filterBombIndex);

                //change the color of the FilerBomb Aura, but only if its a real filterBomb and no Tour-Checkpoint
                if(filterBomb.GetComponent<FilterBomb>().checkpoint == false)
                {
                    filterBomb.GetComponent<FilterBomb>().state = 0;
                    filterBomb.GetComponent<FilterBomb>().influence = true;
                    filterBomb.GetComponent<FilterBomb>().changeState();
                } else //If it is a checkpoint, disable the Aura completely
                {
                    filterBomb.GetComponent<FilterBombAura>().itsAcheckpoint();
                }
            } else
            {
                filterBomb.GetComponent<FilterBomb>().influence = false;
            }
        }

        foreach (ActiveImage thisImgObj in Data.activeImageObjs)
        {
            //Get intensety-percent of each bomb applied to the image
            float[] thisImageBombPercent = new float[thisImgObj.bombDists.Count];

            float allBombDistsThisImg = 0;
            for (int i = 0; i < thisImgObj.bombDists.Count; i++)
            {
                //Only take the distances from the active FilterBombs
                if (activeBombIndices.Contains(i) == true)
                {
                    allBombDistsThisImg = allBombDistsThisImg + thisImgObj.bombDists[i];
                }
            }

            for (int i = 0; i < thisImgObj.bombDists.Count; i++)
            {
                if (activeBombIndices.Contains(i) == true)
                {
                    float thisBombDistPercent = map(thisImgObj.bombDists[i], 0, allBombDistsThisImg, 0, 1);
                    
                    //If there is more then one FilterBomb applied, reverse percentage since a small distance should make a bigger impact
                    if (activeBombIndices.Count > 1)
                    {
                        thisBombDistPercent = 1f - thisBombDistPercent;
                        thisBombDistPercent = thisBombDistPercent / (activeBombIndices.Count - 1);
                    }
                    

                    thisImageBombPercent[i] = thisBombDistPercent;
                }
            }

            float thisImagePercentTotal = 0;
            foreach (float percent in thisImageBombPercent)
            {
                thisImagePercentTotal = thisImagePercentTotal + percent;
            }


            //Enhanced Clustering 
            //Find heaviest bomb
            if (thisImageBombPercent.Length > 2 && enhancedClustering == true)
            {
                int bestBomb = 0;
                float largestPercent = 0;
                for (int i = 0; i < thisImageBombPercent.Length; i++)
                {
                    if (activeBombIndices.Contains(i) == true)
                    {
                        if (thisImageBombPercent[i] > largestPercent)
                        {
                            largestPercent = thisImageBombPercent[i];
                            bestBomb = i;
                        }
                    }
                }
                //Adjust pecentages
                int x = clusterX;
                float s = thisImageBombPercent[bestBomb];
                float c = clusteringFaktor;
                float newPercent = (x + (s * c) / (-s + 1)) / (c + x + (s * c) / (-s + 1));
                thisImageBombPercent[bestBomb] = newPercent;

                float percentLeftover = 1f - newPercent;
                float percentLeftoverTotal = 0f;
                for (int i = 0; i < thisImageBombPercent.Length; i++)
                {
                    if (activeBombIndices.Contains(i) == true && i != bestBomb)
                    {
                        percentLeftoverTotal = percentLeftoverTotal + thisImageBombPercent[i];
                    }
                }

                for (int i = 0; i < thisImageBombPercent.Length; i++)
                {
                    if (activeBombIndices.Contains(i) == true && i != bestBomb)
                    {
                        float thisNewPercent = map(thisImageBombPercent[i], 0f, percentLeftoverTotal, 0f, percentLeftover);
                        thisImageBombPercent[i] = thisNewPercent;
                    }
                }
            }



            Vector3 finalAllBombsVec = new Vector3(0, 0, 0);
            for (int i = 0; i < thisImgObj.bombVecs.Count; i++)
            {
                if (activeBombIndices.Contains(i) == true)
                {
                    finalAllBombsVec = finalAllBombsVec + thisImgObj.bombVecs[i] * thisImageBombPercent[i];
                }
            }

            //Move image to corresponding position
            if (activeBombIndices.Count > 0 && thisImgObj.image.activeSelf == true)
            {
                thisImgObj.image.GetComponent<ImgScript>().moveImageToPos(finalAllBombsVec, 3);
            }
        }
        enhancedClustering = true;
    }

    public void disableAllFilterbombs()
    {
        foreach (GameObject thisFilterBomb in allFilterBombs)
        {
            FilterBomb thisFBscript = thisFilterBomb.GetComponent<FilterBomb>();
            thisFBscript.influence = false;
            thisFBscript.state = 1;
            thisFBscript.changeState();
        }
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    

}
