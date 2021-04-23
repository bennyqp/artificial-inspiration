using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SliderHover : MonoBehaviour
{
    private UIDesigner uiDesigner;

    public GameObject sliderName;
    public GameObject activatorTopObj, activatorBackObj;

    public Color32 hoverBack;
    private Color32 normalBack, normalTop, normalText;

    private SpriteRenderer backSprite, topSprite;
    private TextMeshPro text;

    bool pressed, hover;

    [SerializeField]
    private OVRInput.Controller buttonController;


    // Start is called before the first frame update
    void Start()
    {
        uiDesigner = GameObject.FindGameObjectWithTag("GlobalScripts").GetComponent<UIDesigner>();
        backSprite = activatorBackObj.GetComponent<SpriteRenderer>();
        topSprite = activatorTopObj.GetComponent<SpriteRenderer>();
        text = sliderName.GetComponent<TextMeshPro>();

        pressed = false;
    }

    private void Update()
    {
        if (hover == true)
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, buttonController))
            {
                pressed = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        hover = true;
        if (other.tag == "rightHand")
        {
            normalText = text.color;
            normalBack = backSprite.color;
            normalTop = topSprite.color;

            changeSliderColor(uiDesigner.uiHoverColor, uiDesigner.uiHoverColor);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        hover = false;
        if (pressed == false)
        {
            if (other.tag == "rightHand")
            {
                changeSliderColor(normalText, normalBack);
            }
        }
        pressed = false;
    }

    private void changeSliderColor(Color32 textColor, Color32 activatorBack)
    {
        text.color = textColor;
        backSprite.color = activatorBack;
    }
}
