using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class VRButton : MonoBehaviour
{

    public UnityEvent function2run;

    public bool activateHover;
    public Color32 normalColor, hoverColor = new Color32(255, 255, 255, 255);
    public bool isCloseButton;

    [SerializeField]
    private OVRInput.Controller buttonController;

    private bool colliderActivated;

    private UIDesigner uiDesigner;
    public bool useGlobalHoverColor;

    private void Awake()
    {
        if(function2run == null)
        {
            function2run = new UnityEvent();
        }
    }

    private void Start()
    {
        colliderActivated = false;

        if(activateHover == true)
        {
            //Set to normal Color
            changeColor(normalColor);
        }

        uiDesigner = GameObject.FindGameObjectWithTag("GlobalScripts").GetComponent<UIDesigner>();
        if (useGlobalHoverColor == true)
        {
            hoverColor = uiDesigner.uiHoverColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (colliderActivated == true)
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, buttonController))
            {
                //If it's a close button, reset this Script before closing
                if (isCloseButton == true)
                {
                    colliderActivated = false;
                    changeColor(normalColor);
                }


                function2run.Invoke();
            }
        }
    }

    private void changeColor(Color32 newColor)
    {
        if(GetComponent<SpriteRenderer>() != null)
        {
            GetComponent<SpriteRenderer>().color = newColor;
        }
        if(GetComponent<TextMeshPro>() != null)
        {
            GetComponent<TextMeshPro>().color = newColor;
        }
    }

    public void closeButton()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "rightHand")
        {
            colliderActivated = true;
            if (activateHover == true)
            {
                changeColor(hoverColor);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "rightHand")
        {
            colliderActivated = false;
            if (activateHover == true)
            {
                changeColor(normalColor);
            }
        }
    }
}
