using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Data : MonoBehaviour
{
    public static List<ActiveImage> activeImageObjs, allImageObjs, allImageObjsBackup;
    public static int maxImages;
    public static string sessionID;

    void Awake()
    {
        activeImageObjs = new List<ActiveImage>();
        allImageObjs = new List<ActiveImage>();
        allImageObjsBackup = new List<ActiveImage>();
        sessionID = generateSessionId();
    }

    string generateSessionId()
    {
        DateTime localDate = DateTime.Now;
        //localDate.ToString("de-DE");

        string day = localDate.ToString("dd");
        string time = localDate.ToString("HHmmss");

        string id = day + time;
        return id;
    }

}
