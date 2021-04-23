using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FilterMenu : MonoBehaviour
{
    [SerializeField]
    private OVRInput.Controller leftController, rightController;

    public GameObject globalScripts;
    public GameObject colorFilter;
    public GameObject abstractFilter;
    public GameObject clusteringObj;
    public GameObject newFilter;

    public GameObject mandatoryComp;
    public GameObject handComp;
    public GameObject bombComp;

    FilterScript filterScript;
    FilterBombFilter filterBombFilter;
    FilterColor filterColor;
    VRSlider filterAbstract;
    VRSlider filter2;
    VRSlider clustering;
    VRNavigation vrNavigation;
    VRControll vrControll;

    private bool openedByBomb;
    private FilterBomb currentFilterBomb;
    public GameObject newFilterBomb;
    public GameObject renderingCam;
    private GameObject animationCurvesObj;
    BennysAnimationCurves animationCurves;

    public GameObject filterBombActivationButton; 

    //saved Filter-Settings
    public List<Color32> storedColors;
    public bool storedColorActivation;
    public float storedAbstractLevel;
    public bool storedAbstractActivation;
    public float storedFilter2Level;
    public bool storedFilter2Activation;

    // Start is called before the first frame update
    void Start()
    {
        filterScript = globalScripts.GetComponent<FilterScript>();
        filterBombFilter = globalScripts.GetComponent<FilterBombFilter>();
        filterColor = colorFilter.GetComponent<FilterColor>();
        filterAbstract = abstractFilter.GetComponent<VRSlider>();
        clustering = clusteringObj.GetComponent<VRSlider>();
        filter2 = newFilter.GetComponent<VRSlider>();
        vrNavigation = globalScripts.GetComponent<VRNavigation>();
        vrControll = globalScripts.GetComponent<VRControll>();

        animationCurvesObj = GameObject.FindGameObjectWithTag("animationCurveObj");
        animationCurves = animationCurvesObj.GetComponent<BennysAnimationCurves>();

        GetComponent<UpdateLeftControlerMenu>().sync = false;
    }

    public void letsFilter()
    {
        if (openedByBomb == false)
        {
            //Farbfilter vorbereiten
            filterScript.activateColorFilter = filterColor.active;
            if (filterColor.active == true)
            {
                filterScript.filterColors.Clear();
                for (int i = 0; i < filterColor.filterColorNumber; i++)
                {
                    filterScript.filterColors.Add(filterColor.filterColorObjs[i].GetComponent<SpriteRenderer>().color);
                }
            }

            //Abstract Filter vorbereiten
            filterScript.activateAbstractFilter = filterAbstract.active;
            filterScript.abstractLevel = filterAbstract.sliderValue / 100;

            //Neuen Filter vorbereiten
            filterScript.filter2activated = filter2.active;
            filterScript.filter2Level = filter2.sliderValue / 100;

            //Lets Filter
            filterScript.letsFilter();
        } else
        {
            //Farbfilter vorbereiten
            currentFilterBomb.storedColorActivation = filterColor.active;
            if (filterColor.active == true)
            {
                currentFilterBomb.storedColors.Clear();
                for (int i = 0; i < filterColor.filterColorNumber; i++)
                {
                    currentFilterBomb.storedColors.Add(filterColor.filterColorObjs[i].GetComponent<SpriteRenderer>().color);
                }
            }

            //Abstract Filter vorbereiten
            currentFilterBomb.storedAbstractActivation = filterAbstract.active;
            currentFilterBomb.storedAbstractLevel = filterAbstract.sliderValue / 100;

            //Neuen Filter vorbereiten
            currentFilterBomb.storedFilter2Activation = filter2.active;
            currentFilterBomb.storedFilter2Level = filter2.sliderValue / 100;

            //Clustering einfügen
            filterBombFilter.clusteringFaktor = 10.1f - clustering.sliderValue; 

            //Lets FilterBomb
            filterBombFilter.letsFilterBomb();
        }


    }

    public void openFilterMenu(bool bombOpend, GameObject filterBomb)
    {
        vrNavigation.movementAllowed = false;
        vrControll.blockLaser = true;
        openedByBomb = bombOpend;

        if(bombOpend == true)
        {
            bombComp.SetActive(true);
            currentFilterBomb = filterBomb.GetComponent<FilterBomb>();
            filterColor.restoreColors(currentFilterBomb.storedColors);
            filterColor.active = currentFilterBomb.storedColorActivation;

            filterAbstract.restoreSliderValue(currentFilterBomb.storedAbstractLevel);
            filterAbstract.active = !currentFilterBomb.storedAbstractActivation;
            filterAbstract.activateSlider();

            filter2.restoreSliderValue(currentFilterBomb.storedFilter2Level);
            filter2.active = !currentFilterBomb.storedFilter2Activation;
            filter2.activateSlider();

            changeFilterBombActivationButtonText();
        } else
        {
            handComp.SetActive(true);
            filterColor.restoreColors(storedColors);
            filterColor.active = storedColorActivation;

            filterAbstract.restoreSliderValue(storedAbstractLevel);
            filterAbstract.active = !storedAbstractActivation;
            filterAbstract.activateSlider();

            filter2.restoreSliderValue(storedFilter2Level);
            filter2.active = !storedFilter2Activation;
            filter2.activateSlider();
        }
    }

    public void closeFilterMenu()
    {
        if (openedByBomb == true)
        {
            currentFilterBomb.storedColors.Clear();
            for (int i = 0; i < filterColor.filterColorObjs.Count; i++)
            {
                currentFilterBomb.storedColors.Add(filterColor.filterColorObjs[i].GetComponent<SpriteRenderer>().color);
            }
            currentFilterBomb.storedColorActivation = filterColor.active;

            currentFilterBomb.storedAbstractLevel = filterAbstract.sliderValue;
            currentFilterBomb.storedAbstractActivation = filterAbstract.active;

            currentFilterBomb.storedFilter2Level = filter2.sliderValue;
            currentFilterBomb.storedFilter2Activation = filter2.active;
        } else
        {
            storedColors.Clear();
            for (int i = 0; i < filterColor.filterColorObjs.Count; i++)
            {
                storedColors.Add(filterColor.filterColorObjs[i].GetComponent<SpriteRenderer>().color);
            }
            storedColorActivation = filterColor.active;

            storedAbstractLevel = filterAbstract.sliderValue;
            storedAbstractActivation = filterAbstract.active;

            storedFilter2Level = filter2.sliderValue;
            storedFilter2Activation = filter2.active;
        }

        //Reset Filer Menu
        filterColor.clearColors();

        vrNavigation.movementAllowed = true;
        vrControll.blockLaser = false;

        handComp.SetActive(false);
        bombComp.SetActive(false);
        gameObject.SetActive(false);
    }

    public void AddNewFilterbomb()
    {
        Vector3 nullVec = renderingCam.transform.position;
        Vector3 randomPosInFront = nullVec + renderingCam.transform.forward * 0.6f + new Vector3(Random.RandomRange(0.0f, 0.3f), Random.RandomRange(0.0f, 0.3f), Random.RandomRange(0.0f, 0.3f));
        GameObject thisNewFilterBomb = Instantiate(newFilterBomb, nullVec - new Vector3(0, 0.5f, 0), Quaternion.identity);
        LeanTween.move(thisNewFilterBomb, randomPosInFront, 1).setEase(animationCurves.easeOut);
    }

    public void RemoveFilterBomb()
    {
        filterBombFilter.allFilterBombs.Remove(currentFilterBomb.gameObject);
        Destroy(currentFilterBomb.gameObject);
        closeFilterMenu();
    }


    public void filterBombActivation()
    {
        currentFilterBomb.active = !currentFilterBomb.active;
        changeFilterBombActivationButtonText();
        if(currentFilterBomb.active == true)
        {
            currentFilterBomb.state = currentFilterBomb.getNewState();
        } else
        {
            currentFilterBomb.state = 1;
        }

        currentFilterBomb.changeState();
    }
    void changeFilterBombActivationButtonText()
    {
        if (currentFilterBomb.active == true)
        {
            filterBombActivationButton.GetComponent<TextMeshPro>().text = "Deactivate FilterBomb";
        }
        else
        {
            filterBombActivationButton.GetComponent<TextMeshPro>().text = "Activate FilterBomb";
        }
    }


    // Update is called once per frame
    void Update()
    {

        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, leftController) && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, rightController))
        {
            letsFilter();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            AddNewFilterbomb();
        }




        /*
        //Filter Menü verschieben
        if (Input.GetKeyDown(KeyCode.Q))
        {
            float check = transform.position.x;
            check -= 0.01f;
            transform.position = new Vector3(check, transform.position.y, transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            float check = transform.position.y;
            check += 0.01f;
            transform.position = new Vector3(transform.position.x, check, transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            float check = transform.position.z;
            check -= 0.01f;
            transform.position = new Vector3(transform.position.x, transform.position.y, check);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            float check = transform.position.x;
            check += 0.01f;
            transform.position = new Vector3(check, transform.position.y, transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            float check = transform.position.y;
            check -= 0.01f;
            transform.position = new Vector3(transform.position.x, check, transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            float check = transform.position.z;
            check += 0.01f;
            transform.position = new Vector3(transform.position.x, transform.position.y, check);
        }
        */
    }
}
