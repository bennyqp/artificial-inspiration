using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UIDesigner : MonoBehaviour
{

    public Color32 uiBackgroundColor;
    public Color32 uiNormalColor;
    public Color32 uiHoverColor;

    private string currentColorString;
    private string checkColorString;

    public bool hasChanged;

    // Start is called before the first frame update
    void Start()
    {
        currentColorString = createString(uiBackgroundColor, uiNormalColor, uiHoverColor);
        checkColorString = currentColorString;
    }

    // Update is called once per frame
    void Update()
    {
        currentColorString = createString(uiBackgroundColor, uiNormalColor, uiHoverColor);

        if (currentColorString != checkColorString)
        {
            checkColorString = currentColorString;
            hasChanged = true; 
        }
    }

    string createString(Color32 color1, Color32 color2, Color32 color3)
    {
        string color1string = color1.ToString();
        string color2string = color2.ToString();
        string color3string = color3.ToString();

        string colorString = color1string + color2string + color3string;

        return colorString;
    }

    IEnumerator setChangedBool()
    {
        yield return new WaitForEndOfFrame();
        hasChanged = false;
    }
}
