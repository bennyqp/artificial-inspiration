using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AddSessionID : MonoBehaviour
{

    TextMeshPro text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshPro>();
        text.text = text.text + " " + Data.sessionID;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
