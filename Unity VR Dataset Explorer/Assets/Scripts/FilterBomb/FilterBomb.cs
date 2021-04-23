using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterBomb : MonoBehaviour
{
    public bool active;
    public bool influence;
    public int state;
    [SerializeField]
    private OVRInput.Controller leftController, rightController;
    private GameObject leftHandAnchor, rightHandAnchor;
    private bool leftHandHover, rightHandHover;

    private bool grabedBomb;

    private Vector3 differenceVec;

    private GameObject globalScripts;
    private VRNavigation vRNavigation;
    private VRControll vRControll;
    private FilterBombAura filterBombAura;
    private GameObject cameraRig;
    private GameObject filterMenu;
    private FilterMenu filterMenuScript;
    private FilterBombFilter filterBombFilter;


    //saved Filter-Settings
    public List<Color32> storedColors;
    public bool storedColorActivation;
    public float storedAbstractLevel;
    public bool storedAbstractActivation;
    public float storedFilter2Level;
    public bool storedFilter2Activation;
    public int filterBombIndex;

    //Check if any Settings or the position has changed
    public bool hasChanged;
    public string checkChangeCode;

    //Is this a regular FilterBomb or a checkpoint for a tour?
    public bool checkpoint;
    public int tourIndex;

    // Start is called before the first frame update
    void Start()
    {
        globalScripts = GameObject.FindGameObjectWithTag("GlobalScripts");
        leftHandAnchor = GameObject.FindGameObjectWithTag("leftHand");
        rightHandAnchor = GameObject.FindGameObjectWithTag("rightHand");
        cameraRig = GameObject.FindGameObjectWithTag("OVRCameraRig");
        filterMenu = GameObject.FindGameObjectWithTag("filterMenu");

        vRNavigation = globalScripts.GetComponent<VRNavigation>();
        vRControll = globalScripts.GetComponent<VRControll>();
        filterBombFilter = globalScripts.GetComponent<FilterBombFilter>();
        filterBombAura = GetComponent<FilterBombAura>();

        //If it's a regular Filterbomb connect the FilterMenu Script
        if (checkpoint == false)
        {
            //Set FilterBomb active
            active = true;
            hasChanged = true;

            //Add FilterBomb to Filerbomb-List
            filterBombFilter.allFilterBombs.Add(gameObject);
            filterBombIndex = 999;

            filterMenuScript = filterMenu.GetComponent<FilterMenu>();
            //Close old Menu and open Bomb Menu
            filterMenuScript.closeFilterMenu();
            filterMenu.SetActive(true);
            filterMenuScript.openFilterMenu(true, gameObject);
            state = 1;
            changeState();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //All this stuff only needs to be done if it's a regular FilterBomb
        if(checkpoint == false)
        {

            //Grab Filterbomb and move with it 
            if (rightHandHover == true && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, rightController))
            {
                vRNavigation.somethingGrabed = true;
                vRControll.somethingGrabed = true;
                differenceVec = transform.position - rightHandAnchor.transform.position;
                grabedBomb = true;
            }
            if (grabedBomb == true && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, rightController))
            {
                transform.position = rightHandAnchor.transform.position + differenceVec;
            }
            if (grabedBomb == true && OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, rightController))
            {
                vRNavigation.somethingGrabed = false;
                vRControll.somethingGrabed = false;
                GetComponent<Rigidbody>().velocity = OVRInput.GetLocalControllerVelocity(rightController) + cameraRig.GetComponent<Rigidbody>().velocity;
                grabedBomb = false;
            }

            //Open Filter-Bomb Filter Menu
            if (leftHandHover == true && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, leftController))
            {
                if (filterMenu.activeSelf == false)
                {
                    filterMenu.SetActive(true);
                    filterMenuScript.openFilterMenu(true, gameObject);
                }
            }
        }

    }

    public void checkChange()
    {
        //Check if Filterbomb Settings or Position has changed
        if (hasChanged == false)
        {
            if (checkChangeCode != generatedChangeCode())
            {
                hasChanged = true;
            }
        }
    }

    public bool ready2filter()
    {
        bool ready = false;

        if (storedAbstractActivation == true || storedFilter2Activation == true)
        {
            if (active == true)
            {
                ready = true;
            }
        }
        if( storedColorActivation == true && storedColors.Count > 0 && active == true)
        {
            ready = true;
        }
        return ready;
    }

    public string generatedChangeCode()
    {
        string colorString = "";
        foreach(Color32 thisColor in storedColors)
        {
            colorString = colorString + thisColor.ToString();
        }
        return storedFilter2Activation.ToString() + storedFilter2Level.ToString() + storedAbstractActivation.ToString() + storedAbstractLevel.ToString() + storedColorActivation.ToString() + colorString + transform.position.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == leftHandAnchor)
        {
            leftHandHover = true;
            vRControll.blockLaser = true;
        }
        if (other.gameObject == rightHandAnchor)
        {
            rightHandHover = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == leftHandAnchor)
        {
            leftHandHover = false;
            vRControll.blockLaser = false;
        }
        if (other.gameObject == rightHandAnchor)
        {
            rightHandHover = false;
        }
    }

    public int getNewState()
    {
        int newState = 1;
        if (influence == true)
        {
            newState = 0;
        }
        return newState;
    }

    public void changeState()
    {
        filterBombAura.changeState(state);
    }
}
