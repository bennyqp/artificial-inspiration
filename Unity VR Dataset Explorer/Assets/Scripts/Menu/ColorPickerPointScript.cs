using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPickerPointScript : MonoBehaviour
{
    public GameObject controllerObj;
    public Color32 selectedColor;

    private Vector3 oldPos;
    private Vector3 oldInsideCubePos;

    [SerializeField]
    private OVRInput.Controller colorPickerController;

    private bool colliderActivated;
    private Vector3 differenceVec;

    [SerializeField]
    private GameObject localPositionHelper;

    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = new Vector3(255 / 2, 255 / 2, 255 / 2);
        oldPos = transform.position;

        oldInsideCubePos = transform.position;
        colliderActivated = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform.position);

        if(oldPos != transform.position)
        {
            selectedColor = new Color32((byte)transform.localPosition.x, (byte)transform.localPosition.y, (byte)transform.localPosition.z, 255);
            GetComponent<SpriteRenderer>().color = selectedColor;
        }

        if(colliderActivated == true)
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, colorPickerController))
            {
                controllerObj.SetActive(false);
                differenceVec = transform.position - controllerObj.transform.position;
            }
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, colorPickerController))
            {
                Vector3 newPos = controllerObj.transform.position + differenceVec;
                localPositionHelper.transform.position = newPos;
                Vector3 localPositionHelperPosition = localPositionHelper.transform.localPosition;
                if (localPositionHelperPosition.x >= 0 && localPositionHelperPosition.x <= 255 && localPositionHelperPosition.y >= 0 && localPositionHelperPosition.y <= 255 && localPositionHelperPosition.z >= 0 && localPositionHelperPosition.z <= 255)
                {
                    if (controllerObj.activeSelf == true)
                    {
                        controllerObj.SetActive(false);
                    }
                    transform.position = newPos;
                } else
                {
                    if (controllerObj.activeSelf == false)
                    {
                        controllerObj.SetActive(true);
                    }
                }
            }
        }
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, colorPickerController))
        {
            if(controllerObj.activeSelf == false)
            { 
                controllerObj.SetActive(true);
            }
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        colliderActivated = true;
    }

    private void OnTriggerExit(Collider other)
    {
        colliderActivated = false;
    }
}
