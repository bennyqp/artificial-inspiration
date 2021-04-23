using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorColliderTrigger : MonoBehaviour
{

    FilterColor filterColor;
    private GameObject colorFilter;

    // Start is called before the first frame update
    void Start()
    {
        colorFilter = transform.parent.gameObject;
        filterColor = colorFilter.GetComponent<FilterColor>();
    }

    private void OnTriggerEnter(Collider other)
    {
        filterColor.thisColorActivation(this.gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        filterColor.thisColorDeactivation();
    }
}
