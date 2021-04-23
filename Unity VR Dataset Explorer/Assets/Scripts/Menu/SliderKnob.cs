using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderKnob : MonoBehaviour
{
    VRSlider vRSlider;

    // Start is called before the first frame update
    void Start()
    {
        vRSlider = transform.parent.gameObject.GetComponent<VRSlider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        vRSlider.knopHover = true;
    }
    private void OnTriggerExit(Collider other)
    {
        vRSlider.knopHover = false;
    }
}
