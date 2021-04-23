using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CheckpointMenu : MonoBehaviour
{
    public GameObject checkpointMenu;
    public GameObject globalScripts;
    public GameObject checkpointPrefab;
    public GameObject checkpointIconsContainer; 
    public float checkpointGab;
    public GameObject checkpoint1, checkpoint2;
    public GameObject newCheckpointIcon;
    public GameObject checkpointIconsBackground;
    private SpriteRenderer checkpointBackground;
    public GameObject checkpointMenuTitel;
    public GameObject removeCheckpointButton;
    public GameObject colorPicker;

    public GameObject colorFilter;
    private FilterColor filterColor;

    public GameObject abstractFilter;
    VRSlider filterAbstract;

    public GameObject newFilter;
    VRSlider filter2;

    TourScript tourScript;

    public List<TourCheckpoint> tourCheckpoints;

    public GameObject currentCheckpointIcon;
    private TourCheckpoint currentCheckpoint;

    // Start is called before the first frame update
    void Start()
    {
        tourScript = globalScripts.GetComponent<TourScript>();
        tourCheckpoints = tourScript.tourCheckpoints;

        filterColor = colorFilter.GetComponent<FilterColor>();
        filterAbstract = abstractFilter.GetComponent<VRSlider>();
        filter2 = newFilter.GetComponent<VRSlider>();

        checkpointBackground = checkpointIconsBackground.GetComponent<SpriteRenderer>();

        Destroy(checkpoint1);
        Destroy(checkpoint2);
        addNewCheckpoint();
        addNewCheckpoint();

        Debug.Log("Tour Checkpoints: " + tourCheckpoints.Count);
    }

    public void openCheckpointMenu()
    {
        int currentCheckpointIndex = currentCheckpointIcon.GetComponent<CheckpointIconScript>().checkpointIndex;
        currentCheckpoint = tourCheckpoints[currentCheckpointIndex];

        //Setting the correct title of the menu
        if (currentCheckpointIndex == 0)
        {
            checkpointMenuTitel.GetComponent<TextMeshPro>().text = "Tour Start: ";
        } else if (currentCheckpointIndex > 0 && currentCheckpointIndex < tourCheckpoints.Count - 1)
        {
            checkpointMenuTitel.GetComponent<TextMeshPro>().text = "Checkpoint " + (currentCheckpointIndex + 1).ToString();
        } else
        {
            checkpointMenuTitel.GetComponent<TextMeshPro>().text = "Tour End: ";
        }

        //Show the Remove Checkpoint button only if the checkpoint index is greater than 2
        if (currentCheckpointIndex < 2)
        {
            removeCheckpointButton.SetActive(false);
        } else
        {
            removeCheckpointButton.SetActive(true);
        }

        filterColor.restoreColors(currentCheckpoint.colors);
        filterColor.active = currentCheckpoint.colorActivation;

        filterAbstract.restoreSliderValue(currentCheckpoint.abstractLevel);
        filterAbstract.active = !currentCheckpoint.abstractActivation;
        filterAbstract.activateSlider();

        filter2.restoreSliderValue(currentCheckpoint.filter2Level);
        filter2.active = !currentCheckpoint.filter2Activation;
        filter2.activateSlider();
    }

    public void closeCheckpointMenu()
    {
        if(currentCheckpoint != null)
        {
            currentCheckpoint.colors.Clear();
            for (int i = 0; i < filterColor.filterColorObjs.Count; i++)
            {
                currentCheckpoint.colors.Add(filterColor.filterColorObjs[i].GetComponent<SpriteRenderer>().color);
            }
            currentCheckpoint.colorActivation = filterColor.active;

            currentCheckpoint.abstractLevel = filterAbstract.sliderValue;
            currentCheckpoint.abstractActivation = filterAbstract.active;

            currentCheckpoint.filter2Level = filter2.sliderValue;
            currentCheckpoint.filter2Activation = filter2.active;
        }

        //Reset Filter Menu
        filterColor.clearColors();

        if(colorPicker.activeSelf == true)
        {
            colorPicker.SetActive(false);
        }

        checkpointMenu.SetActive(false);
    }

    public void addNewCheckpoint()
    {
        TourCheckpoint newCheckpoint = new TourCheckpoint();
        newCheckpoint.colors = new List<Color32>(0);
        newCheckpoint.colorActivation = false;
        newCheckpoint.abstractLevel = 0;
        newCheckpoint.abstractActivation = false;
        newCheckpoint.filter2Level = 0;
        newCheckpoint.filter2Activation = false;
        newCheckpoint.checkpointIndex = tourCheckpoints.Count;
        GameObject thisNewCheckpoint = Instantiate(checkpointPrefab);
        newCheckpoint.checkpointIcon = thisNewCheckpoint;
        tourCheckpoints.Add(newCheckpoint);

        thisNewCheckpoint.transform.parent = checkpointIconsContainer.transform;
        thisNewCheckpoint.transform.localPosition = new Vector3(newCheckpoint.checkpointIndex * checkpointGab, 0, 0);
        thisNewCheckpoint.transform.localScale = new Vector3(0.016f, 0.016f, 0.016f);
        thisNewCheckpoint.GetComponent<CheckpointIconScript>().checkpointMenuObj = checkpointMenu;
        thisNewCheckpoint.GetComponent<CheckpointIconScript>().checkpointMenu = this;
        thisNewCheckpoint.GetComponent<CheckpointIconScript>().checkpointIndex = newCheckpoint.checkpointIndex;
        thisNewCheckpoint.GetComponent<CheckpointIconScript>().updateNumber(newCheckpoint.checkpointIndex);

        LeanTween.moveLocal(newCheckpointIcon, new Vector3(tourCheckpoints.Count * checkpointGab, 0, 0), 1);
        LeanTween.moveLocal(checkpointIconsContainer, new Vector3(-tourCheckpoints.Count * checkpointGab / 2, checkpointIconsContainer.transform.localPosition.y, 0) , 1);

        LeanTween.moveLocal(checkpointIconsBackground, new Vector3(tourCheckpoints.Count * checkpointGab / 2f, 0, 0), 1);
        float newBackgroundWidth = (tourCheckpoints.Count + 1) * 3 + 1.8f;
        LeanTween.value(checkpointIconsBackground, resizeCheckpointsBackground, checkpointBackground.size.x, newBackgroundWidth, 1);

        if (checkpointMenu.activeSelf == true)
        {
            closeCheckpointMenu();
            currentCheckpointIcon = thisNewCheckpoint;
            openCheckpointMenu();
        } else
        {
            currentCheckpointIcon = thisNewCheckpoint;
        }
    }

    public void removeCheckpoint()
    {
        tourCheckpoints.Remove(currentCheckpoint);
        Destroy(currentCheckpointIcon);

        for (int i = 0; i < tourCheckpoints.Count; i++)
        {  
            tourCheckpoints[i].checkpointIcon.GetComponent<CheckpointIconScript>().checkpointIndex = i;
            tourCheckpoints[i].checkpointIcon.GetComponent<CheckpointIconScript>().updateNumber(i);
            LeanTween.moveLocal(tourCheckpoints[i].checkpointIcon, new Vector3(i * checkpointGab, 0, 0), 1);
        }
        LeanTween.moveLocal(newCheckpointIcon, new Vector3(tourCheckpoints.Count * checkpointGab, 0, 0), 1);
        LeanTween.moveLocal(checkpointIconsContainer, new Vector3(-tourCheckpoints.Count * checkpointGab / 2, checkpointIconsContainer.transform.localPosition.y, 0), 1);

        LeanTween.moveLocal(checkpointIconsBackground, new Vector3(tourCheckpoints.Count * checkpointGab / 2f, 0, 0), 1);
        float newBackgroundWidth = (tourCheckpoints.Count + 1) * 3 + 1.8f;
        LeanTween.value(checkpointIconsBackground, resizeCheckpointsBackground, checkpointBackground.size.x, newBackgroundWidth, 1);

        currentCheckpoint = tourCheckpoints[tourCheckpoints.Count - 1];
        currentCheckpointIcon = currentCheckpoint.checkpointIcon;
        closeCheckpointMenu();
    }

    void resizeCheckpointsBackground(float newWidth)
    {
        checkpointBackground.size = new Vector2(newWidth, checkpointBackground.size.y);
    }

    public void startTourButton()
    {
        if(checkpointMenu.activeSelf == true)
        {
            closeCheckpointMenu();
        }
        tourScript.startTour();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
