using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constructor : MonoBehaviour
{
    public bool startInExploreMode;
    public bool quickStart;
    public GameObject home;

    public float posScale;
    public float yPosScale;
    float lastPosScale;
    float lastYPosScale;

    public GameObject imageContainer;

    ImgScript imgScript;
    FilterScript filterScript;
    ScaleImageSpace scaleImageSpace;

    GameObject[] generatedImages;
    List<GameObject> hidingFilterBombs;

    LoadData loadData;
    List<Quest> img2vec;
    public GameObject image;

    public bool started;
    private bool firstStart;

    public GameObject homeObj;
    public GameObject homeLocation;
    public GameObject cameraRig;
    public GameObject mainCamera;
    public GameObject mainMenu;

    [Header("Bachground Colors for different Modes")]
    public Color homeBackgroundColor;
    public Color exploreBackgroundColor;
    private Color currentSkyboxColor;

    private HomeScript homeScript;
    private FilterBombFilter filterBombs;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        filterScript = GetComponent<FilterScript>();
        scaleImageSpace = imageContainer.GetComponent<ScaleImageSpace>();
        homeScript = homeObj.GetComponent<HomeScript>();
        filterBombs = GetComponent<FilterBombFilter>();

        lastPosScale = posScale;
        lastYPosScale = yPosScale;

        loadData = GetComponent<LoadData>();
        yield return new WaitForEndOfFrame();
        img2vec = loadData.quests;

        constructTSNE();
        scaleImageSpace.scaleImageSpace(posScale, yPosScale);
        yield return new WaitForEndOfFrame();
        filterScript.startFilter();

        hidingFilterBombs = new List<GameObject>();

        //Handle start of application 
        started = false;
        firstStart = true;
        startHandler();

    }


    //Handle application state

    void startHandler()
    {
        if (startInExploreMode == true)
        {
            currentSkyboxColor = exploreBackgroundColor;
            RenderSettings.skybox.SetColor("_Tint", currentSkyboxColor);
            home.SetActive(false);
            goExplore();
        }
        else
        {
            currentSkyboxColor = homeBackgroundColor;
            RenderSettings.skybox.SetColor("_Tint", currentSkyboxColor);
            home.SetActive(true);
            goHome();
        }
    }

    public void goHome()
    {
        if(firstStart == false)
        {
            home.SetActive(true);
            List<GameObject> imagesAtHome = homeScript.imagesAtHome();
            Debug.Log("imagesAtHome Length: " + imagesAtHome.Count);
            foreach (GameObject activeImage in imagesAtHome)
            {
                if (activeImage.tag == "generatedImage" )
                {
                    //activeImage.GetComponent<ImgScript>().fadeOutAndDisable(1);
                    activeImage.SetActive(false);
                } else if (activeImage.tag == "filterBomb")
                {
                    hidingFilterBombs.Add(activeImage);
                    activeImage.SetActive(false);
                }
            }
        }
        firstStart = false;

        if (mainMenu.activeSelf == true)
        {
            mainMenu.SetActive(false);
        }
        homeLocation.SetActive(true);
        homeScript.loadSelectedImages();
        cameraRig.transform.position = new Vector3(0, 0, 0);
        GetComponent<VRControll>().enabled = false;
        GetComponent<VRNavigation>().spaceMovement = false;
        GetComponent<VRNavigation>().flatNavigation = true;
        filterBombs.disableAllFilterbombs();

        //Change Tint of Skybox
        changeSkyboxTint(homeBackgroundColor, 1f);
    }
    public void goExplore()
    {
        home.SetActive(false);
        //Start startanimation if its the first time
        if (started == false)
        {
            imgStartAnimation();
            started = true;
        } else
        {
            Debug.Log("Active Images Obj Tour: " + Data.activeImageObjs.Count);
            foreach (ActiveImage activeImage in Data.activeImageObjs)
            {
                activeImage.image.GetComponent<ImgScript>().enableAndFadeIn(1);
            }

            //Change Tint of Skybox
            changeSkyboxTint(exploreBackgroundColor, 1f);
        }

        if (hidingFilterBombs.Count > 0)
        {
            foreach (GameObject hidingFilterBomb in hidingFilterBombs)
            {
                hidingFilterBomb.SetActive(true);
            }
            hidingFilterBombs.Clear();
        }

        filterBombs.enhancedClustering = true;
        GetComponent<VRControll>().enabled = true;
        GetComponent<VRControll>().onTour = false;
        GetComponent<VRNavigation>().flatNavigation = false;
        GetComponent<VRNavigation>().spaceMovement = true;
    }
    public void goOnTour()
    {
        Debug.Log("Go on tour");
        home.SetActive(false);
        GetComponent<VRControll>().enabled = true;
        GetComponent<VRControll>().onTour = true;
        GetComponent<VRNavigation>().flatNavigation = false;
        GetComponent<VRNavigation>().spaceMovement = false;
        
        //First Time see images? Fade them in!
        if (firstStart == false)
        {
            foreach (ActiveImage activeImage in Data.activeImageObjs)
            {
                activeImage.image.GetComponent<ImgScript>().fadeOutAndDisable(1);
            }
        }
        firstStart = false;


        //Change Tint of Skybox
        changeSkyboxTint(exploreBackgroundColor, 1f);
    }



    //Construct image space

    public void constructTSNE()
    {
        //initialize Image for each Quest
        for (int i = 0; i < img2vec.Count; i++)
        {
            //instantiate new Image
            Vector3 imageInstantiateVec = new Vector3(img2vec[i].tsneX, img2vec[i].tsneY, img2vec[i].tsneZ);

            //instantiate new Image
            GameObject newImg = Instantiate(image, imageInstantiateVec, Quaternion.identity);

            //Set Color of Image to transparent
            newImg.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 0);

            //Move new Image into ImageContainer 
            newImg.transform.parent = imageContainer.transform;


            //Load sprite into image Object
            Sprite thisSprite = Resources.Load<Sprite>(img2vec[i].path);
            newImg.GetComponent<SpriteRenderer>().sprite = thisSprite;

            //Write all necessary information into ImgScript
            imgScript = newImg.GetComponent<ImgScript>();
            imgScript.tsneX = img2vec[i].tsneX;
            imgScript.tsneY = img2vec[i].tsneY;
            imgScript.tsneZ = img2vec[i].tsneZ;
            imgScript.path = img2vec[i].path;
            imgScript.id = img2vec[i].id;
            imgScript.colors = img2vec[i].colors;
            imgScript.imageColPercent = img2vec[i].imageColPercent;
            imgScript.abstractLevel = img2vec[i].abstractLevel;
            imgScript.composition = img2vec[i].composition;

            //Create ActiveImage Object to Add to different Lists
            ActiveImage thisImageObject = new ActiveImage { image = newImg, bombVecs = new List<Vector3>(), bombDists = new List<float>() };

            //Add to allImages List
            newImg.SetActive(false);
            Data.allImageObjs.Add(thisImageObject);
            //And also add to the allImagesBackup List
            Data.allImageObjsBackup.Add(thisImageObject);
            if (i <= Data.maxImages)
            {
                //Create random Y-StartPos and move active Image to this position
                float randomImageYstartPos = Random.Range(100.0f / yPosScale, 350.0f / yPosScale);
                newImg.transform.position = new Vector3(img2vec[i].tsneX, randomImageYstartPos, img2vec[i].tsneZ);

                newImg.SetActive(true);
                Data.activeImageObjs.Add(thisImageObject);
            }
        }

        //Fill generatedImages with all generated Images
        generatedImages = GameObject.FindGameObjectsWithTag("generatedImage");
    }

    // Update is called once per frame
    void Update()
    {
        if (lastPosScale != posScale || lastYPosScale != yPosScale)
        {
            lastPosScale = posScale;
            lastYPosScale = yPosScale;
            scaleImageSpace.scaleImageSpace(posScale, yPosScale);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            images2tsneSpace();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            goExplore();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            goHome();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            images2d();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            //Current active Images
            GetComponent<TourScript>().countActiveImages();
        }
    }

    //Start Start-Animation
    void imgStartAnimation()
    {
        foreach (ActiveImage activeImage in Data.activeImageObjs)
        {
            activeImage.image.SetActive(true);
            ImgScript thisImgScript = activeImage.image.GetComponent<ImgScript>();
            thisImgScript.StartCoroutine("thisImgStartAnimation");
        }
    }

    //3D Ansicht
    public void images2tsneSpace()
    {
        LeanTween.cancelAll(true);
        foreach (ActiveImage activeImage in Data.activeImageObjs)
        {
            activeImage.image.GetComponent<ImgScript>().images2tsneSpace(3);
        }
    }
    //2D Ansicht
    public void images2d()
    {
        LeanTween.cancelAll(true);
        foreach (ActiveImage activeImage in Data.activeImageObjs)
        {
            activeImage.image.GetComponent<ImgScript>().images2d(3);
        }
    }




    //Change Skybox Tint

    public void changeSkyboxTint(Color newSkyCol, float animationTime)
    {
        Color thisSkyboxColor = currentSkyboxColor;
        LeanTween.value(gameObject, thisSkyboxColor, newSkyCol, animationTime).setOnUpdate(updateSkyboxTint);
    }
    private void updateSkyboxTint(Color newSkyCol) {
        RenderSettings.skybox.SetColor("_Tint", newSkyCol);
        currentSkyboxColor = newSkyCol;
    }

}
