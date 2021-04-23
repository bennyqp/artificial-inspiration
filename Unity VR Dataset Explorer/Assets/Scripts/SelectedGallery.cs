using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedGallery : MonoBehaviour
{
    public GameObject globalScripts;
    public GameObject selectedGalleryImages;
    public GameObject selectedImagePrefab;

    public GameObject rightWall, leftWall;
    private float rightWallInit, leftWallInit;  

    public float imageDistance;
    public int maxGalleryImgs;

    VRControll vrControll;

    // Start is called before the first frame update
    void Start()
    {
        vrControll = globalScripts.GetComponent<VRControll>();

        deleteImageGallery();

        rightWallInit = rightWall.transform.position.x - 3 * 5 / 2;
        leftWallInit = leftWall.transform.position.x + 3 * 5 / 2;

        Debug.Log("right: " + rightWallInit);
        Debug.Log("left: " + leftWallInit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateImageGallery()
    {
        deleteImageGallery();

        List<GameObject> selectedImages = vrControll.selectedImages;

        Vector3 currentPos = selectedGalleryImages.transform.position;
        selectedGalleryImages.transform.position = new Vector3((selectedImages.Count - 1) / 2f * imageDistance, currentPos.y, currentPos.z);

        for (int i = 0; i < selectedImages.Count; i++)
        {
            if (i < maxGalleryImgs)
            {
                GameObject newImageObj = Instantiate(selectedImagePrefab);
                newImageObj.transform.parent = selectedGalleryImages.transform;
                newImageObj.transform.localPosition = new Vector3(-i * imageDistance, 0, 0);

                Sprite thisImageSprite = selectedImages[i].GetComponent<SpriteRenderer>().sprite;
                newImageObj.transform.GetChild(0).GetComponent<Renderer>().material.SetTexture("_MainTex", thisImageSprite.texture);
            }
            
        }


        Vector3 rightWallPos = rightWall.transform.position;
        Vector3 leftWallPos = leftWall.transform.position;

        if (selectedImages.Count >= 3 && selectedImages.Count < maxGalleryImgs)
        {
            rightWall.transform.position = new Vector3(selectedImages.Count * 5 / 2 + rightWallInit, rightWallPos.y, rightWallPos.z);
            leftWall.transform.position = new Vector3(-1 * (selectedImages.Count * 5 / 2 - leftWallInit), rightWallPos.y, rightWallPos.z);
        } else if (selectedImages.Count < 3)
        {
            rightWall.transform.position = new Vector3(rightWallInit + 3 * 5 / 2 , rightWallPos.y, rightWallPos.z);
            leftWall.transform.position = new Vector3(leftWallInit - 3 * 5 / 2, leftWallPos.y, leftWallPos.z);
        } else
        {
            rightWall.transform.position = new Vector3((maxGalleryImgs + 1) * 5 / 2 + rightWallInit, rightWallPos.y, rightWallPos.z);
            leftWall.transform.position = new Vector3(-1 * ((maxGalleryImgs + 1) * 5 / 2 - leftWallInit), rightWallPos.y, rightWallPos.z);
        }
    }

    private void deleteImageGallery()
    {
        foreach (Transform child in selectedGalleryImages.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
