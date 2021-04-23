using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TourScript : MonoBehaviour
{
    public float timeForTour; 
    public GameObject cameraRig;
    public GameObject imageContainer;
    public GameObject filterBomb;
    public List<TourCheckpoint> tourCheckpoints;
    public float tourZstartPos, tourZendPos;

    private FilterBombFilter filterBombFilter;
    private Constructor constructor;
    private VRNavigation vrNavigation;

    private int nextCheckpointIndex;

    private GameObject animationCurvesObj;
    BennysAnimationCurves animationCurves;


    //Tour ID
    int tourTween;
    [Header("Tour Menu Components")]
    public GameObject tourMenu;
    public GameObject tourPauseButton;


    [Header("Nur zu Testzwecken;")]
    public List<Color32> colors;
    public bool colorActivation;
    public float abstractLevel;
    public bool abstractActivation;
    public float filter2Level;
    public bool filter2Activation;
    public int checkpointIndex;
    public Vector3 checkpointPosition;


    // Start is called before the first frame update
    void Awake()
    {
        filterBombFilter = GetComponent<FilterBombFilter>();
        constructor = GetComponent<Constructor>();
        vrNavigation = GetComponent<VRNavigation>();

        tourCheckpoints = new List<TourCheckpoint>();

        animationCurvesObj = GameObject.FindGameObjectWithTag("animationCurveObj");
        animationCurves = animationCurvesObj.GetComponent<BennysAnimationCurves>();
    }

    void createNewCheckpoint()
    {
        TourCheckpoint newCheckCheck = new TourCheckpoint();
        newCheckCheck.colors = new List<Color32>();
        newCheckCheck.colors.Add(new Color32((byte)Random.RandomRange(0, 255), (byte)Random.RandomRange(0, 255), (byte)Random.RandomRange(0, 255), 255));
        newCheckCheck.colorActivation = colorActivation;
        newCheckCheck.abstractLevel = abstractLevel;
        newCheckCheck.abstractActivation = abstractActivation;
        newCheckCheck.filter2Level = filter2Level;
        newCheckCheck.filter2Activation = filter2Activation;
        newCheckCheck.checkpointIndex = tourCheckpoints.Count;

        tourCheckpoints.Add(newCheckCheck);
    }

    public void startTour()
    {
        //If Checkpoint Menu is open, close and save parameters first

        //Check if all necessary checkpoint parameters are set
        List<int> checkpointsNotReady = new List<int>();
        foreach(TourCheckpoint checkpoint in tourCheckpoints)
        {
            bool ready = false;
            if (checkpoint.abstractActivation == true || checkpoint.filter2Activation == true)
            {
                ready = true;
            }
            if (checkpoint.colorActivation == true && checkpoint.colors.Count > 0)
            {
                ready = true;
            }

            if (ready == false)
            {
                checkpointsNotReady.Add(checkpoint.checkpointIndex);
            } 
        }


        //Only start the tour if all checkpoints are ready

        if(checkpointsNotReady.Count == 0)
        {
            constructor.goOnTour();
            vrNavigation.movementAllowed = false;

            //Get start and end position for the tour
            tourZstartPos = -imageContainer.transform.localScale.z * 1.66f;
            tourZendPos = imageContainer.transform.localScale.z * 1.66f;

            //Delete all FilterBombs and remove from list
            GameObject[] currentFilterBombs = GameObject.FindGameObjectsWithTag("filterBomb");
            foreach (GameObject thisFilterBomb in currentFilterBombs)
            {
                Destroy(thisFilterBomb);
            }
            filterBombFilter.allFilterBombs.Clear();

            //How far is each Checkpoint away from each other?
            float completeTourDist = Mathf.Abs(tourZstartPos - tourZendPos);
            float distPerCheckpoint = completeTourDist / (tourCheckpoints.Count - 1);

            for (int i = 0; i < tourCheckpoints.Count; i++)
            {
                //Calculate Position of this Checkpoint in ImageCoainterSpace 
                float randomXPos = Random.Range(tourZstartPos / 2, tourZendPos / 2);
                float randomYPos = Random.Range(tourZstartPos / 2, tourZendPos / 2);
                Vector3 thisBombVector = new Vector3(randomXPos, randomYPos, tourZstartPos + distPerCheckpoint * i);

                //Convert position of this checkpoint to WorldSpace 
                //thisBombVector = imageContainer.GetComponent<ScaleImageSpace>().getGlobalPosition(thisBombVector);
                tourCheckpoints[i].checkpointPosition = thisBombVector;

                //Instantiate new FilterBombCheckpoint
                GameObject newCheckpointObj = Instantiate(filterBomb, thisBombVector, Quaternion.identity);
                FilterBomb newCheckpoint = newCheckpointObj.GetComponent<FilterBomb>();

                //Set the new FilterBomb to a checkpoint instead of a FilterBomb
                newCheckpoint.checkpoint = true;

                //Set necessary settings and add new FilterBomb to the List
                newCheckpoint.hasChanged = true;
                newCheckpoint.active = true;
                newCheckpoint.filterBombIndex = 999;
                newCheckpoint.tourIndex = i;
                filterBombFilter.allFilterBombs.Add(newCheckpointObj);

                //Fill the new FilterBombCheckpoint
                newCheckpoint.storedColors = tourCheckpoints[i].colors;
                newCheckpoint.storedColorActivation = tourCheckpoints[i].colorActivation;
                newCheckpoint.storedAbstractLevel = tourCheckpoints[i].abstractLevel;
                newCheckpoint.storedAbstractActivation = tourCheckpoints[i].abstractActivation;
                newCheckpoint.storedFilter2Level = tourCheckpoints[i].filter2Level;
                newCheckpoint.storedFilter2Activation = tourCheckpoints[i].filter2Activation;
            }

            if(tourCheckpoints.Count > 2)
            {
                filterBombFilter.enhancedClustering = true;
                filterBombFilter.clusteringFaktor = 4;
            } else
            {
                filterBombFilter.enhancedClustering = false;
            }

            filterBombFilter.letsFilterBomb();
            StartCoroutine("letsTour");
        } else
        {
            if (checkpointsNotReady.Count == 1)
            {
                Camera.main.gameObject.GetComponent<ShowInfo>().sendInfo("Checkpoint " + (checkpointsNotReady[0] + 1) + " is not ready yet.", 3f, 10);
            }
            else
            {
                string checkpointsNotReadyString = "";
                foreach (int i in checkpointsNotReady)
                {
                    if (i == checkpointsNotReady.Count - 1)
                    {
                        checkpointsNotReadyString = checkpointsNotReadyString + " and " + (checkpointsNotReady[i] + 1);
                    }
                    else
                    {
                        if (i == checkpointsNotReady.Count - 2)
                        {
                            checkpointsNotReadyString = checkpointsNotReadyString + (checkpointsNotReady[i] + 1);
                        } else
                        {
                            checkpointsNotReadyString = checkpointsNotReadyString + (checkpointsNotReady[i] + 1) + ", ";
                        }
                    }
                }
                Camera.main.gameObject.GetComponent<ShowInfo>().sendInfo("Checkpoints " + checkpointsNotReadyString + " are not ready yet.", 3f, 10);
            }
        }
        
    }


    public void countActiveImages()
    {
        int activeImages = 0;
        foreach(Transform child in imageContainer.transform)
        {
            if(child.gameObject.activeSelf == true)
            {
                activeImages = activeImages + 1;
            }
        }
    }

    IEnumerator letsTour()
    {
        //Lets move the CameraRig to the startPos
        cameraRig.transform.position = tourCheckpoints[0].checkpointPosition;

        //Wait until all Images are in their place
        yield return new WaitForSeconds(3.5f);

        //Reset the nextCheckpointIndex
        nextCheckpointIndex = 1;

        //And lets start the tour
        moveToNextCheckpoint();
    }

    void moveToNextCheckpoint()
    {
        float animationTime = timeForTour / (tourCheckpoints.Count - 1);

        Debug.Log("animation time: " + animationTime);

        //Start to first checkpoint but there are more than two checkpoints in total
        if ( nextCheckpointIndex == 1 && tourCheckpoints.Count > 2)
        {
            tourTween = LeanTween.move(cameraRig, tourCheckpoints[1].checkpointPosition, animationTime).setEase(animationCurves.tourStart).setOnComplete(moveToNextCheckpoint).id;
        }

        //Standard movement between two checkpoints
        if (nextCheckpointIndex > 1 && nextCheckpointIndex < tourCheckpoints.Count - 1)
        {
            tourTween = LeanTween.move(cameraRig, tourCheckpoints[nextCheckpointIndex].checkpointPosition, animationTime).setOnComplete(moveToNextCheckpoint).id;
        }

        //If next checkpoint is the last checkpoint
        if (nextCheckpointIndex == tourCheckpoints.Count - 1)
        {
            tourTween = LeanTween.move(cameraRig, tourCheckpoints[nextCheckpointIndex].checkpointPosition, animationTime).setEase(animationCurves.tourEnd).setOnComplete(endTour).id;
        }

        nextCheckpointIndex = nextCheckpointIndex + 1;
    }

    void endTour()
    {
        Camera.main.gameObject.GetComponent<ShowInfo>().sendInfo("Tour ended\nFeel free to explore now!", 3f, 0);
        vrNavigation.movementAllowed = true;

        //delete all FilterBombs and remove from list
        GameObject[] currentFilterBombs = GameObject.FindGameObjectsWithTag("filterBomb");
        foreach (GameObject thisFilterBomb in currentFilterBombs)
        {
            Destroy(thisFilterBomb);
        }
        filterBombFilter.allFilterBombs.Clear();

        constructor.started = true;
        constructor.goExplore();
    }

    //Pause / Resume / Stop Tour

    public void pauseTour()
    {
        if(LeanTween.isPaused(tourTween) == false)
        {
            LeanTween.pause(tourTween);
            tourPauseButton.GetComponent<TextMeshPro>().text = "Resume Tour";
        } else
        {
            LeanTween.resume(tourTween);
            tourPauseButton.GetComponent<TextMeshPro>().text = "Pause Tour";
        }
    }

    public void resumeTour()
    {
        LeanTween.resume(tourTween);
    }

    public void stopTour()
    {
        LeanTween.cancel(tourTween);
        tourMenu.SetActive(false);
        endTour();
    }
}
