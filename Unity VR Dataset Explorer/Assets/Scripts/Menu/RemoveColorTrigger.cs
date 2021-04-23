using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveColorTrigger : MonoBehaviour
{
    FilterColor filterColor;
    private GameObject colorFilter;

    public bool colliderActivated;

    [SerializeField]
    private OVRInput.Controller buttonController;


    // Start is called before the first frame update
    void Start()
    {
        colorFilter = transform.parent.gameObject;
        filterColor = colorFilter.GetComponent<FilterColor>();
        colliderActivated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (colliderActivated == true)
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, buttonController))
            {
                filterColor.removeColor();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        colliderActivated = true;
        GetComponent<SpriteRenderer>().color = filterColor.selectedObject.GetComponent<SpriteRenderer>().color;
    }
    private void OnTriggerExit(Collider other)
    {
        colliderActivated = false;
        GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
    }
}
