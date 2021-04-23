using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VRSlider : MonoBehaviour
{
    public bool active;
    public float sliderMin, sliderMax;
    public Color32 deactivationColor;
    public bool startActivated;

    public bool alwayActivated;

    [SerializeField]
    private OVRInput.Controller leftController, rightController;
    public GameObject rightHandAnchor;
    public GameObject controllerObj;

    public GameObject sliderKnop;
    public GameObject sliderLine;
    private LineRenderer sliderLineRenderer;
    public GameObject sliderValueText;
    private TextMeshPro sliderValueTextMesh;
    public GameObject sliderNameText;
    private TextMeshPro sliderNameTextMesh;
    public GameObject helperPositionObj;
    public GameObject sliderActivator;

    private LineRenderer line;
    public float lineMin, lineMax;
    private float sliderScale;

    public bool knopHover;

    public float sliderValue;
    public int sliderIntValue; 



    // Start is called before the first frame update
    void Awake()
    {
        sliderLineRenderer = sliderLine.GetComponent<LineRenderer>();
        lineMin = sliderLineRenderer.GetPosition(0).x;
        lineMax = sliderLineRenderer.GetPosition(1).x;

        sliderValueTextMesh = sliderValueText.GetComponent<TextMeshPro>();
        sliderNameTextMesh = sliderNameText.GetComponent<TextMeshPro>();

        
        active = true;
        if (alwayActivated == false)
        {
            if (startActivated == false)
            {
                activateSlider();
            }
        } else
        {
            sliderActivator.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (active == true)
        {
            if (knopHover == true)
            {
                if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, rightController))
                {
                    helperPositionObj.transform.position = rightHandAnchor.transform.position;
                    helperPositionObj.transform.localPosition = new Vector3(helperPositionObj.transform.localPosition.x, 0, 0);
                    if (lineMin <= helperPositionObj.transform.localPosition.x && helperPositionObj.transform.localPosition.x <= lineMax)
                    {
                        sliderKnop.transform.localPosition = helperPositionObj.transform.localPosition;
                        if (controllerObj.activeSelf == true)
                        {
                            controllerObj.SetActive(false);
                        }
                        sliderValue = map(sliderKnop.transform.localPosition.x, lineMin, lineMax, sliderMin, sliderMax);
                        sliderIntValue = Mathf.RoundToInt(sliderValue);
                        sliderValueTextMesh.text = sliderIntValue.ToString();
                    }
                }
                else
                {
                    EnableController();
                }
            }
            else
            {
                EnableController();
            }
        }
    }

    public void activateSlider()
    {
        Color32 activationColor;

        if (active == true)
        {
            activationColor = deactivationColor;
        } else
        {
            activationColor = new Color32(255, 255, 255, 255);
        }
        sliderKnop.GetComponent<SpriteRenderer>().color = activationColor;
        sliderLineRenderer.endColor = activationColor;
        sliderLineRenderer.startColor = activationColor;
        sliderValueTextMesh.color = activationColor;
        sliderNameTextMesh.color = activationColor;
        sliderActivator.GetComponent<SpriteRenderer>().color = activationColor;
        GameObject sliderActivationPoint = sliderActivator.transform.GetChild(0).gameObject;
        sliderActivationPoint.SetActive(!active);
        active = !active;
    }

    public void restoreSliderValue(float newValue)
    {
        sliderValue = newValue;
        sliderIntValue = Mathf.RoundToInt(newValue);
        sliderValueTextMesh.text = sliderIntValue.ToString();
        newValue = map(newValue, sliderMin, sliderMax, lineMin, lineMax);

        sliderKnop.transform.localPosition = new Vector3(newValue, 0, 0);
        helperPositionObj.transform.localPosition = new Vector3(newValue, 0, 0);
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    private void EnableController()
    {
        if (controllerObj.activeSelf == false)
        {
            controllerObj.SetActive(true);
        }
    }
}
