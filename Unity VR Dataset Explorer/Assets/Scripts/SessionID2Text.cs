using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SessionID2Text : MonoBehaviour
{

    TextMeshPro sessionID;
    public int textVersion;


    // Start is called before the first frame update
    void Start()
    {
        sessionID = GetComponent<TextMeshPro>();

        if(textVersion == 0)
        {
            sessionID.text = "Session ID: " + Data.sessionID;
        } else
        {
            sessionID.text = Data.sessionID;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
