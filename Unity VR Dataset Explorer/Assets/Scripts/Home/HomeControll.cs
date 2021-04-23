using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeControll : MonoBehaviour
{
    [SerializeField]
    private OVRInput.Controller leftController, rightController;

    public GameObject checkpointMenu;
    public GameObject tourCylinder;

    private CheckpointMenu checkpointMenuScript;

    // Start is called before the first frame update
    void Start()
    {
        checkpointMenuScript = tourCylinder.GetComponent<CheckpointMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, leftController))
        {
            if (checkpointMenu.activeSelf == true)
            {
                checkpointMenuScript.closeCheckpointMenu();
            }
        }
    }
}
