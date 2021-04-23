using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScript : MonoBehaviour
{
    [SerializeField]
    private OVRInput.Controller leftController, rightController;

    public GameObject globalScripts;
    public GameObject startImage;
    public GameObject homeLocation;
    public GameObject checkpointMenu;
    public GameObject tourCylinder;
    public GameObject info;
    public GameObject mainCam;

    private StartImageScript startImageScript;
    private Constructor constructor;
    private CheckpointMenu checkpointMenuScript;
    private TourScript tourScript;

    private SelectedGallery selectedGallery; 

    private GameObject animationCurvesObj;
    BennysAnimationCurves animationCurves;

    public bool started;

    public LayerMask m_LayerMask;


    // Start is called before the first frame update
    void Start()
    {
        startImageScript = startImage.GetComponent<StartImageScript>();
        constructor = globalScripts.GetComponent<Constructor>();
        checkpointMenuScript = tourCylinder.GetComponent<CheckpointMenu>();
        tourScript = globalScripts.GetComponent<TourScript>();
        selectedGallery = GetComponent<SelectedGallery>();

        animationCurvesObj = GameObject.FindGameObjectWithTag("animationCurveObj");
        animationCurves = animationCurvesObj.GetComponent<BennysAnimationCurves>();


        started = false;
        info.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.G)) 
        {
            StartImageSpace();
        }


        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, leftController))
        {
            if (checkpointMenu.activeSelf == true)
            {
                checkpointMenuScript.closeCheckpointMenu();
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.Start, leftController))
        {
            if(info.activeSelf == false)
            {
                Vector3 horizontalForward = new Vector3(mainCam.transform.forward.x, 0, mainCam.transform.forward.z);
                Vector3 posInFront = mainCam.transform.position + Vector3.Normalize(horizontalForward) * 1.75f;
                info.transform.position = posInFront;
                Quaternion camRotation = new Quaternion();
                camRotation.eulerAngles = new Vector3(0, mainCam.transform.eulerAngles.y, 0);
                info.transform.rotation = camRotation;
                info.SetActive(true);
            }
             else
            {
                info.SetActive(false);
            }
        }

    }

    public void loadSelectedImages()
    {
        selectedGallery.updateImageGallery();
    }

   public void StartImageSpace()
    {
        if(constructor.started == false)
        {
            LeanTween.move(startImage, new Vector3(0, 175, 0), 5).setEase(animationCurves.softEaseIn).setOnComplete(constructor.goExplore);
            startImageScript.fadeOutAndDisable(4);
            constructor.changeSkyboxTint(constructor.exploreBackgroundColor, 5f);
        } else
        {
            constructor.goExplore();
        }
        homeLocation.SetActive(false);
    }

    public List<GameObject> imagesAtHome()
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, new Vector3(265, 11, 82), Quaternion.identity, m_LayerMask);

        List<GameObject> imageObjs = new List<GameObject>();
        foreach (Collider collider in hitColliders)
        {
            GameObject hitObj = collider.gameObject;
            imageObjs.Add(hitObj);

        }
        return imageObjs;
    }
}
