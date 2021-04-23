using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UpdateLeftControlerMenu : MonoBehaviour
{
    public GameObject mandatoryLeft, handLeft, bombLeft, mainLeft, tourLeft, checkpointLeft;
    public GameObject hand, bomb;
    public GameObject applyFilterButton, addFilterButton, removeFilterbombButton;
    bool handActivated, bombActivated;
    bool hasChanged;
    public bool sync = true;

    // Start is called before the first frame update
    void Start()
    {
        handActivated = false;
        bombActivated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (sync == true)
        {
            handLeft.transform.localPosition = mandatoryLeft.transform.localPosition;
            bombLeft.transform.localPosition = mandatoryLeft.transform.localPosition;
            mainLeft.transform.localPosition = mandatoryLeft.transform.localPosition;
            tourLeft.transform.localPosition = mandatoryLeft.transform.localPosition;
            checkpointLeft.transform.localPosition = mandatoryLeft.transform.localPosition;
        }

        if (hand.activeSelf != handActivated || bomb.activeSelf != bombActivated)
        {
            hasChanged = true;
            handActivated = hand.activeSelf;
            bombActivated = bomb.activeSelf;
        }

        if (bomb.activeSelf == true && hasChanged == true)
        {
            applyFilterButton.transform.localPosition = new Vector3(applyFilterButton.transform.localPosition.x, removeFilterbombButton.transform.localPosition.y, applyFilterButton.transform.localPosition.z);
            hasChanged = false;
        }
        if (hand.activeSelf == true && hasChanged == true)
        {
            applyFilterButton.transform.localPosition = new Vector3(applyFilterButton.transform.localPosition.x, addFilterButton.transform.localPosition.y, applyFilterButton.transform.localPosition.z);
            hasChanged = false;
        }
    }
}
