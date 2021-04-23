using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRControll : MonoBehaviour
{
    [SerializeField]
    private OVRInput.Controller leftController, rightController;

    public GameObject leftHandAnchor;
    private LineRenderer laser;
    private GameObject hoverImg;
    private bool hovered;
    public List<GameObject> selectedImages;

    public GameObject filterMenu;
    public GameObject filterMenuMandatory;
    public GameObject filterMenuHand;
    public GameObject filterMenuBomb;
    public GameObject vrColorPicker;
    public GameObject removeColorIcon;

    public GameObject savedMenu;
    public GameObject tourMenu;
    public bool blockLaser;

    VRNavigation vrNavigation;
    Constructor constructorScript;
    FilterScript filterScript;
    FilterMenu filterMenuScript;
    FilterBombFilter filterBombs;

    public bool somethingGrabed;
    public bool onTour;

    private bool loadedAllConnections;


    // Start is called before the first frame update
    void Start()
    {
        blockLaser = false;

        filterMenuMandatory.SetActive(true);

        loadedAllConnections = false;
        filterMenuHand.SetActive(true);
        filterMenuBomb.SetActive(true);
        filterMenu.SetActive(true);
        savedMenu.SetActive(true);
        tourMenu.SetActive(true);

        laser = leftHandAnchor.GetComponent<LineRenderer>();
        laser.sortingLayerName = "Foreground";

        vrNavigation = GetComponent<VRNavigation>();
        constructorScript = GetComponent<Constructor>();
        filterScript = GetComponent<FilterScript>();
        filterMenuScript = filterMenu.GetComponent<FilterMenu>();
        filterBombs = GetComponent<FilterBombFilter>();

        somethingGrabed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(onTour == false)
        {
            //Open / Close Filter Menu with Hand
            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, leftController) == true && somethingGrabed == false && savedMenu.activeSelf == false)
            {
                if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, leftController) == false)
                {
                    if (filterMenu.activeSelf == false)
                    {
                        filterMenu.SetActive(true);
                        filterMenuScript.openFilterMenu(false, null);
                    }
                    else
                    {
                        filterMenuScript.closeFilterMenu();
                        vrColorPicker.SetActive(false);
                        removeColorIcon.SetActive(false);
                    }
                }
            }

            //Open / Close Saved Images Menu
            if (OVRInput.GetDown(OVRInput.Button.Start, leftController) == true && filterMenu.activeSelf == false)
            {
                if (savedMenu.activeSelf == false)
                {
                    savedMenu.SetActive(true);
                }
                else
                {
                    savedMenu.SetActive(false);
                }
            }

            //Reset all filters
            if (OVRInput.GetDown(OVRInput.Button.Two, leftController))
            {
                filterBombs.disableAllFilterbombs();
                filterScript.disableAllFilters();
            }

            //Sort images by color 
            if (OVRInput.GetDown(OVRInput.Button.One, leftController))
            {
                filterBombs.disableAllFilterbombs();
                filterScript.sortImagesByColor();
            }

            //3D view
            if (OVRInput.GetDown(OVRInput.Button.Two, rightController))
            {
                filterBombs.disableAllFilterbombs();
                constructorScript.images2tsneSpace();
            }
            //2D view
            if (OVRInput.GetDown(OVRInput.Button.One, rightController))
            {
                filterBombs.disableAllFilterbombs();
                constructorScript.images2d();
            }
        } else
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, leftController) == true)
            {
                if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, leftController) == false)
                {
                    if (tourMenu.activeSelf == true)
                    {
                        tourMenu.SetActive(false);
                    }
                    else
                    {
                        tourMenu.SetActive(true);
                    }
                }
            }
        }

        //Show laser to select something 
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, leftController) == true && blockLaser == false)
        {
            ShootLaser(leftHandAnchor.transform.position + leftHandAnchor.transform.TransformDirection(new Vector3(-0.01f, -0.006f, 0)), leftHandAnchor.transform.forward, 300);
            laser.enabled = true;

            //Add or remove image to selected Images
            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, leftController) == true && hovered == true) 
            {
                if (selectedImages.Contains(hoverImg) == false)
                {
                    selectedImages.Add(hoverImg);
                } else
                {
                    selectedImages.Remove(hoverImg);
                }
            }

        } else
        {
            laser.enabled = false;
        }
       if(OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, leftController) == true)
        {
            if(hoverImg != null)
            {
                unselectImage(hoverImg);
                hovered = false;
            }
        }

       //Let all connections run for one Frame to avoid errors 
       if (loadedAllConnections == false)
        {
            loadedAllConnections = true;
            filterMenuHand.SetActive(false);
            filterMenuBomb.SetActive(false);
            filterMenu.SetActive(false);
            savedMenu.SetActive(false);
            tourMenu.SetActive(false);
        }

    }




    private GameObject hitObj;
    private GameObject prevHitObj;
    private bool firstCollide = true;
    void ShootLaser(Vector3 targetPos, Vector3 direction, float length)
    {
        Ray ray = new Ray(targetPos, direction);
        Vector3 endPos = targetPos + (direction * length);

        if (Physics.Raycast(ray, out RaycastHit rayHit, length))
        {
            //If an generated Image is hit:
            if (rayHit.collider.gameObject.tag == "generatedImage")
            {
                endPos = rayHit.point;
                hitObj = rayHit.collider.gameObject;
                if (firstCollide == true)
                {
                    prevHitObj = hitObj;
                    firstCollide = false;
                }
                if (hovered == false)
                {
                    if (hitObj.GetComponent<SpriteRenderer>() != null)
                    {
                        selectImage(hitObj);
                        hoverImg = hitObj;
                    }
                    hovered = true;
                }
            }

            if (rayHit.collider.gameObject.tag == "filterBomb")
            {
                if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, leftController))
                {
                    filterMenu.SetActive(true);
                    filterMenuScript.openFilterMenu(true, rayHit.collider.gameObject);
                }
            }
            
        } else
        {
            hitObj = null;
        }
        
        if(hitObj != prevHitObj)
        {
            if (prevHitObj.GetComponent<SpriteRenderer>() != null)
            {
                unselectImage(prevHitObj);
            }

            prevHitObj = hitObj;
            hovered = false;

            if (hitObj == null)
            {
                firstCollide = true;
            }
        }

        laser.SetPosition(0, targetPos);
        laser.SetPosition(1, targetPos + (direction * 5));
    }
    void selectImage(GameObject image)
    {
        //image.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
        image.transform.GetChild(0).gameObject.SetActive(true);
    }
    void unselectImage(GameObject image)
    {
        if(selectedImages.Contains(image) == false)
        {
            //image.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            image.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

}
