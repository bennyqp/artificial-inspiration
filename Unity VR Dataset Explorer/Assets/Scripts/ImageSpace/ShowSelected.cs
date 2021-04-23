using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowSelected : MonoBehaviour
{
    public GameObject imageContainer;
    public GameObject mainCam;
    public GameObject showSavedButton;

    VRControll vrControll;

    public List<ActiveImage> storedActiveImageObjs;
    public List<GameObject> selectedImages;
    private List<GameObject> images2fadeout;

    public bool showSelected;

    private GameObject animationCurvesObj;
    BennysAnimationCurves animationCurves;

    private List<Vector3> storedPositions = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        storedActiveImageObjs = new List<ActiveImage>();
        images2fadeout = new List<GameObject>();
        vrControll = GetComponent<VRControll>();

        animationCurvesObj = GameObject.FindGameObjectWithTag("animationCurveObj");
        animationCurves = animationCurvesObj.GetComponent<BennysAnimationCurves>();


        showSelected = false;
    }

    public void MenuShowImages()
    {
        if(showSelected == false)
        {
            ShowSelectedImages();
            showSavedButton.GetComponent<TextMeshPro>().text = "show all Images";

        }
        else
        {
            ShowAllImages();
            showSavedButton.GetComponent<TextMeshPro>().text = "show saved Images";
        }
        showSelected = !showSelected;

    }

    private void ShowSelectedImages()
    {
        selectedImages = vrControll.selectedImages;

        //Store Active Images right now
        storedActiveImageObjs.Clear();
        foreach (ActiveImage activeImage in Data.activeImageObjs)
        {
            storedActiveImageObjs.Add(activeImage);
        }

        //Clear List of Images that have to be faded out again
        images2fadeout.Clear();

        //Clear activeImages List 
        Data.activeImageObjs.Clear();
        
        //Fadeout all Images that are not selcted and remove from allImages List
        foreach (ActiveImage thisImageObj in Data.allImageObjs)
        {
            GameObject thisImage = thisImageObj.image;
            if (selectedImages.Contains(thisImage) == false)
            {
                if (thisImage.activeSelf == true)
                {
                    thisImage.GetComponent<ImgScript>().fadeOutAndDisable(1);
                }
                StartCoroutine(removeFromAllImagesList(thisImageObj));
            } else
            {
                Data.activeImageObjs.Add(thisImageObj);
                if (thisImage.activeSelf == false)
                {
                    thisImage.GetComponent<ImgScript>().enableAndFadeIn(1);
                    images2fadeout.Add(thisImage);
                }
            }
        }

        //Save current positions of selected Images
        foreach(GameObject selctedImage in selectedImages)
        {
            storedPositions.Add(selctedImage.transform.position);
        }

        float[] xPos = new float[selectedImages.Count + 1];
        float[] yPos = new float[selectedImages.Count + 1];
        xPos[0] = 0;
        yPos[0] = 0;

        for (int i = 1; i < selectedImages.Count; i++)
        {
            
            float step1 = 4 * (i - 1) + 1;
            float step2 = Mathf.Sqrt(step1);
            float step3 = step2 % 4;
            float step4 = Mathf.Floor(step3);
            float step5 = step4 * Mathf.PI / 2;
            int step6x = (int) Mathf.Sin(step5);
            int step6y = (int) Mathf.Cos(step5);

            xPos[i] = xPos[i - 1] + step6x;
            yPos[i] = yPos[i - 1] + step6y;
        }

        Vector3 horizontalForward = new Vector3(mainCam.transform.forward.x, 0, mainCam.transform.forward.z);
        Vector3 posInFront = mainCam.transform.position + Vector3.Normalize(horizontalForward) * 30;
        GameObject savedImageDisplay = new GameObject();
        savedImageDisplay.transform.position = posInFront;
        Quaternion camRotation = new Quaternion();
        camRotation.eulerAngles = new Vector3(0, mainCam.transform.eulerAngles.y, 0);
        savedImageDisplay.transform.rotation = camRotation;

        camRotation.eulerAngles = new Vector3(camRotation.eulerAngles.x, Mathf.Abs(camRotation.eulerAngles.y) - 180, camRotation.eulerAngles.z);



        for (int i = 0; i < selectedImages.Count; i++)
        {
            Vector3 newImgPos = new Vector3(xPos[i] * 4, yPos[i] * 4, 0);

            selectedImages[i].transform.parent = savedImageDisplay.transform;
            selectedImages[i].transform.localScale = new Vector3(1, 1, 1);

            float animationTime = 5f / selectedImages.Count;
            LeanTween.moveLocal(selectedImages[i], newImgPos, animationTime * i).setEase(animationCurves.easeOut);
            selectedImages[i].GetComponent<ImgScript>().showSelectedOn = true;
            
            LeanTween.rotate(selectedImages[i], camRotation.eulerAngles, animationTime * i).setEase(animationCurves.easeOut);
        }

    }

    private void ShowAllImages()
    {
        Data.allImageObjs.Clear();
        foreach (ActiveImage addThisImg in Data.allImageObjsBackup)
        {
            Data.allImageObjs.Add(addThisImg);
        }

        List<ActiveImage> images2return = new List<ActiveImage>();
        foreach (ActiveImage image2return in Data.activeImageObjs)
        {
            images2return.Add(image2return);
        }

        Data.activeImageObjs.Clear();
        foreach (ActiveImage activeImage in storedActiveImageObjs)
        {
            Data.activeImageObjs.Add(activeImage);
            GameObject thisImage = activeImage.image;
            thisImage.GetComponent<ImgScript>().enableAndFadeIn(2);
        }

        GameObject imageDisplayObj = gameObject;
        for (int i = 0; i < images2return.Count; i++)
        {
            //Get imageDisplayObj
            if (i == 0)
            {
                imageDisplayObj = images2return[i].image.transform.parent.gameObject;
            }

            images2return[i].image.transform.parent = imageContainer.transform;

            Vector3 storedPosition = storedPositions[i];
            images2return[i].image.GetComponent<ImgScript>().moveImageToPos(storedPosition, 2);
            images2return[i].image.GetComponent<ImgScript>().showSelectedOn = false;
            if (images2fadeout.Contains(images2return[i].image))
            {
                images2return[i].image.GetComponent<ImgScript>().fadeOutAndDisable(1);
            }

            if (i == images2return.Count - 1)
            {
                Destroy(imageDisplayObj);
            }
        }

        Debug.Log("ActiveImages Length: " + Data.activeImageObjs.Count);
    }

    IEnumerator removeFromAllImagesList(ActiveImage imageObj2remove)
    {
        //Replace the AllImages list with only the visible objects, so that filters and everything else only act on the selected images
        yield return new WaitForSeconds(2);
        Data.allImageObjs.Remove(imageObj2remove);
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            ShowSelectedImages();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            ShowAllImages();
        }
    }
}
