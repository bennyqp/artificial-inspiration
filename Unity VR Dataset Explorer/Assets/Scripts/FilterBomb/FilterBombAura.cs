using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FilterBombAura : MonoBehaviour
{

    private Transform mainCamera;
    private ParticleSystem aura;
    private SphereCollider bombCollider;
    private GameObject particleObj;
    private GameObject notActiveObj;
    private FilterBomb filterBomb;

    public bool auraActive;

    public int state;
    public Color32 inactiveColor;

    private int loopColorIndex;
    private bool breakCoolorLoop;


    // Start is called before the first frame update
    void Awake()
    {
        mainCamera = Camera.main.transform;
        particleObj = transform.GetChild(0).gameObject;
        notActiveObj = transform.GetChild(1).gameObject;
        aura = particleObj.GetComponent<ParticleSystem>();
        bombCollider = GetComponent<SphereCollider>();
        filterBomb = GetComponent<FilterBomb>();
    }

    private void Start()
    {
        changeState(1);
        notActiveObj.transform.localScale = new Vector3(20, 20, 20);
    }

    // Update is called once per frame
    void Update()
    {
        float distToCam = Vector3.Distance(transform.position, mainCamera.position);

        if(state == 0)
        {
            if (distToCam < 120)
            {
                aura.startSize = distToCam;
            }  
        } else
        {
            if(filterBomb.checkpoint == false && distToCam < 50)
            {
                notActiveObj.transform.localScale = new Vector3(distToCam / 10, distToCam / 10, distToCam / 10);
            }
            if (filterBomb.checkpoint == true && distToCam < 100)
            {
                notActiveObj.transform.localScale = new Vector3(distToCam / 5, distToCam / 5, distToCam / 5);
            }
        }

        if (distToCam > 1.6f && distToCam < 120)
        {
            bombCollider.radius = distToCam / 2;
        }

    }

    public void itsAcheckpoint()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<LookAt>().enabled = true;
        transform.position = new Vector3(transform.position.x + 2, transform.position.y, transform.position.z + 3);
        GameObject checkpointIcon = transform.GetChild(2).gameObject;
        checkpointIcon.SetActive(true);
        TextMeshPro checkpointIconText = checkpointIcon.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
        checkpointIconText.text = (filterBomb.tourIndex + 1).ToString();
    }

    public void changeState(int newState)
    {
        state = newState;
        if (newState == 0)
        {
            aura.Play();
            notActiveObj.SetActive(false);
            changeColors(newState);

            float distToCam = Vector3.Distance(transform.position, mainCamera.position);
            if (distToCam >= 120)
            {
                aura.startSize = distToCam;
            }

        } else if (newState == 1)
        {
            aura.Stop();
            notActiveObj.SetActive(true);
            changeColors(newState);
        } else
        {
            Debug.Log("Error");
        }
    }

    public void changeColors(int currentState)
    {
        breakCoolorLoop = false;
        if (currentState == 0) {
            if (filterBomb.storedColorActivation == false)
            {
                Debug.Log("No Colors in list");
                breakCoolorLoop = true;
                aura.startColor = inactiveColor;
            } else if (filterBomb.storedColors.Count == 1)
            {
                Debug.Log("One Color in list");
                breakCoolorLoop = true;
                aura.startColor = filterBomb.storedColors[0];
            } else
            {
                Debug.Log("more Colors in list");
                StartCoroutine(loopColors());
            }

        } else if(currentState == 1)
        {
            breakCoolorLoop = true;
            if (filterBomb.storedColors.Count > 0)
            {
                notActiveObj.GetComponent<SpriteRenderer>().color = new Color32(filterBomb.storedColors[0].r, filterBomb.storedColors[0].g, filterBomb.storedColors[0].b, 170);
            } else
            {
                notActiveObj.GetComponent<SpriteRenderer>().color = inactiveColor;
            }
        } else
        {
            Debug.Log("Error");
        }
    }

    IEnumerator loopColors()
    {
        if (breakCoolorLoop == true)
        {
            breakCoolorLoop = false;
            yield break;
        }

        if (loopColorIndex >= filterBomb.storedColors.Count)
        {
            loopColorIndex = 0;
        }
        aura.startColor = filterBomb.storedColors[loopColorIndex];

        yield return new WaitForSeconds(2);
        loopColorIndex = loopColorIndex + 1;
        StartCoroutine(loopColors());
    }
}
