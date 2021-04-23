using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CheckpointIconScript : MonoBehaviour
{
    public GameObject checkpointNumber;
    public int checkpointIndex;

    public GameObject checkpointMenuObj;
    public CheckpointMenu checkpointMenu;

    [SerializeField]
    private OVRInput.Controller buttonController;

    private bool colliderActivated;
    public Color32 normalTextColor, hoverTextColor;
    public Color32 normalSpriteColor, hoverSpriteColor;
    public GameObject textObj;

    private GameObject globalScripts;
    private UIDesigner uiDesigner;

    public bool addButton;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().color = normalSpriteColor;
        textObj.GetComponent<TextMeshPro>().color = normalTextColor;

        globalScripts = GameObject.FindGameObjectWithTag("GlobalScripts");
        uiDesigner = globalScripts.GetComponent<UIDesigner>();

        normalTextColor = uiDesigner.uiNormalColor;
        hoverTextColor = uiDesigner.uiHoverColor;
        normalSpriteColor = uiDesigner.uiNormalColor;
        hoverSpriteColor = uiDesigner.uiHoverColor;
    }

    // Update is called once per frame
    void Update()
    {
        if(addButton == false)
        {
            if (colliderActivated == true)
            {
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, buttonController))
                {
                    opencheckpointMenu();
                }
            }
        }
    }

    public void updateNumber(int newNumber)
    {
        checkpointNumber.GetComponent<TextMeshPro>().text = (newNumber + 1).ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        colliderActivated = true;
        GetComponent<SpriteRenderer>().color = hoverSpriteColor;
        textObj.GetComponent<TextMeshPro>().color = hoverTextColor;

    }
    private void OnTriggerExit(Collider other)
    {
        colliderActivated = false;
        GetComponent<SpriteRenderer>().color = normalSpriteColor;
        textObj.GetComponent<TextMeshPro>().color = normalTextColor;
    }

    void opencheckpointMenu()
    {
        if (checkpointMenuObj.activeSelf == true)
        {
            checkpointMenu.closeCheckpointMenu();
        }
        checkpointMenuObj.SetActive(true);
        checkpointMenu.currentCheckpointIcon = gameObject;
        checkpointMenu.openCheckpointMenu();
    }

}
