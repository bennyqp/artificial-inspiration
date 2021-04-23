using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterColor : MonoBehaviour
{
    public GameObject globalScripts;
    public GameObject renderingCam;
    public GameObject colorPicker;
    public GameObject colorPickerPoint;
    public GameObject removeColorIcon;
    public float menuDist;
    public GameObject newFilterColor;

    private bool colliderActivated;
    private bool syncColors;

    [SerializeField]
    private OVRInput.Controller colorPickerActivatorController, otherController;

    public List<GameObject> filterColorObjs;
    public GameObject addColorObj;
    private GameObject hoverObject;
    public GameObject selectedObject;
    private GameObject prevSelctedObject;
    public int filterColorNumber; 

    FilterScript filterScript;
    RemoveColorTrigger removeColorTrigger;

    public bool active;

    public GameObject colorFilterBackground;
    private SpriteRenderer background;
    private float backgroundStartWidth;

    // Start is called before the first frame update
    void Start()
    {
        syncColors = false;
        filterScript = globalScripts.GetComponent<FilterScript>();
        removeColorTrigger = removeColorIcon.GetComponent<RemoveColorTrigger>();

        background = colorFilterBackground.GetComponent<SpriteRenderer>();
        backgroundStartWidth = background.size.x;

        filterColorNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(colliderActivated == true)
        {
            if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, colorPickerActivatorController))
            {
                prevSelctedObject = selectedObject;
                selectedObject = hoverObject;
                removeColorIcon.SetActive(true);

                if (colorPicker.activeSelf == false)
                {
                    Vector3 horizontalForward = new Vector3(renderingCam.transform.forward.x, 0, renderingCam.transform.forward.z);
                    Vector3 newMenuPos = renderingCam.transform.position + Vector3.Normalize(horizontalForward) * menuDist;
                    Color32 currentColor = selectedObject.GetComponent<SpriteRenderer>().color;
                    colorPickerPoint.transform.localPosition = new Vector3(currentColor.r, currentColor.g, currentColor.b);
                    colorPickerPoint.GetComponent<SpriteRenderer>().color = currentColor;
                    colorPicker.transform.position = newMenuPos;
                    Quaternion colorPickerRotation = new Quaternion();
                    colorPickerRotation.eulerAngles = new Vector3(0, renderingCam.transform.eulerAngles.y, 0);
                    colorPicker.transform.rotation = colorPickerRotation;
                    colorPicker.SetActive(true);
                    syncColors = true;
                }
                else {
                    if(selectedObject == prevSelctedObject)
                    {
                        colorPicker.SetActive(false);
                        removeColorIcon.SetActive(false);
                        selectedObject = null;
                        prevSelctedObject = null;
                        syncColors = false;
                    } else
                    {
                        syncColors = false;
                        Color32 currentColor = selectedObject.GetComponent<SpriteRenderer>().color;
                        colorPickerPoint.transform.localPosition = new Vector3(currentColor.r, currentColor.g, currentColor.b);
                        colorPickerPoint.GetComponent<SpriteRenderer>().color = currentColor;
                        syncColors = true;
                    }
                    
                }
            }
        }

        if (syncColors == true)
        {
            selectedObject.GetComponent<SpriteRenderer>().color = colorPickerPoint.GetComponent<SpriteRenderer>().color;
        }
    }

    public void thisColorActivation(GameObject thisColorObj)
    {
        hoverObject = thisColorObj;
        colliderActivated = true;
    }
    public void thisColorDeactivation()
    {
        colliderActivated = false;
    }

    private float colorDist = 0.14f;
    private float animationTime = 0.5f;

    public void addColor()
    {
        filterColorNumber += 1;
        active = true;

        float newAnchorPos = filterColorNumber * colorDist / 2;
        LeanTween.moveLocal(addColorObj, new Vector3(newAnchorPos, 0, 0), animationTime);

        GameObject addFilterColorObj = Instantiate(newFilterColor, transform);
        addFilterColorObj.transform.localPosition = new Vector3(newAnchorPos - colorDist, 0, 0);

        for (int i = 0; i < filterColorObjs.Count; i++)
        {
            float newPos = newAnchorPos - colorDist * (filterColorObjs.Count - i + 1);
            LeanTween.moveLocal(filterColorObjs[i], new Vector3(newPos, 0, 0), animationTime);
        }

        //Resize Background
        float newBackgroundWidth = (filterColorObjs.Count + 2) * 2f;
        LeanTween.value(colorFilterBackground, resizeColorBackground, background.size.x, newBackgroundWidth, animationTime);

        prevSelctedObject = selectedObject;
        selectedObject = addFilterColorObj;
        Color32 currentColor = selectedObject.GetComponent<SpriteRenderer>().color;
        colorPickerPoint.transform.localPosition = new Vector3(currentColor.r, currentColor.g, currentColor.b);
        colorPickerPoint.GetComponent<SpriteRenderer>().color = currentColor;

        filterColorObjs.Add(addFilterColorObj);
    }

    public void removeColor()
    {
        if (selectedObject != null)
        {
            filterColorNumber -= 1;
            filterColorObjs.Remove(selectedObject);
            syncColors = false;
            colorPicker.SetActive(false);
            Destroy(selectedObject);
            float newBackgroundWidth = 0;

            if (filterColorObjs.Count > 0)
            {
                selectedObject = filterColorObjs[0];
                //Set value for resizing background
                newBackgroundWidth = (filterColorObjs.Count + 1) * 2f;
            } else
            {
                removeColorTrigger.colliderActivated = false;
                removeColorIcon.SetActive(false);
                active = false;
                newBackgroundWidth = backgroundStartWidth;
            }

            float newAnchorPos = filterColorNumber * colorDist / 2;
            LeanTween.moveLocal(addColorObj, new Vector3(newAnchorPos, 0, 0), animationTime);

            for (int i = 0; i < filterColorObjs.Count; i++)
            {
                float newPos = newAnchorPos - colorDist * (filterColorObjs.Count - i);
                LeanTween.moveLocal(filterColorObjs[i], new Vector3(newPos, 0, 0), animationTime);
            }

            //Resize Background
            LeanTween.value(colorFilterBackground, resizeColorBackground, background.size.x, newBackgroundWidth, animationTime);
        }  
    }

    public void clearColors()
    {
        syncColors = false;
        filterColorNumber = 0;
        foreach (GameObject colorObj in filterColorObjs)
        {
            Destroy(colorObj);
        }
        addColorObj.transform.localPosition = new Vector3(0, 0, 0);
        filterColorObjs.Clear();
    }

    public void restoreColors(List<Color32> colors2restore)
    {
        filterColorNumber = colors2restore.Count;
        float newAnchorPos = filterColorNumber * colorDist / 2;
        addColorObj.transform.localPosition = new Vector3(newAnchorPos, 0, 0);

        foreach (Color32 newColor in colors2restore)
        {
            active = true;
            GameObject addFilterColorObj = Instantiate(newFilterColor, transform);
            addFilterColorObj.GetComponent<SpriteRenderer>().color = newColor;
            filterColorObjs.Add(addFilterColorObj);
        }
        for (int i = 0; i < filterColorObjs.Count; i++)
        {
            float newPos = newAnchorPos - colorDist * (filterColorObjs.Count - i);
            filterColorObjs[i].transform.localPosition = new Vector3(newPos, 0, 0);
        }

        float newBackgroundWidth = 0;
        if (filterColorObjs.Count > 0)
        {
            newBackgroundWidth = (filterColorObjs.Count + 1) * 2f;
        }
        else
        {
            newBackgroundWidth = backgroundStartWidth;
        }
        background.size = new Vector2(newBackgroundWidth, background.size.y);
    }

    void resizeColorBackground(float newWidth)
    {
        background.size = new Vector2(newWidth, background.size.y);
    }
}
