using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UIBackground : MonoBehaviour
{

    private UIDesigner uiDesigner;
    private SpriteRenderer background;

    // Start is called before the first frame update
    void Start()
    {
        uiDesigner = GameObject.FindGameObjectWithTag("GlobalScripts").GetComponent<UIDesigner>();
        background = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (uiDesigner.hasChanged == true)
        {
            background.color = uiDesigner.uiBackgroundColor;
        }
    }
}
