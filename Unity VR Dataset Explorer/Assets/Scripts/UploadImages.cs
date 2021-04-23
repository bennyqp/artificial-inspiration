using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UploadImages : MonoBehaviour
{
    VRControll vrControll;
    List<GameObject> selectedImages;

    public bool uploadImagesNow;

    public string imagesUploadUrl;

    private string filePath;

    private string sessionCode;

    // Start is called before the first frame update
    void Start()
    {

        vrControll = GetComponent<VRControll>();
        sessionCode = Data.sessionID;

    }

    // Update is called once per frame
    void Update()
    {
        if (uploadImagesNow == true)
        {

            StartCoroutine(uploadImages());
            uploadImagesNow = false;
        }
    }

    public void saveImages()
    {
        StartCoroutine(uploadImages());
    }

    public IEnumerator uploadImages()
    {
        Debug.Log("Start Upload");
        selectedImages = vrControll.selectedImages;

        string imagesArray = "var images = [";
        string images = "";

        List<string> imageIds = new List<string>();
        foreach (GameObject image in selectedImages)
        {
            imageIds.Add(image.GetComponent<ImgScript>().id);
            imagesArray = imagesArray + "\"" + image.GetComponent<ImgScript>().id + "\", ";
            images = images + image.GetComponent<ImgScript>().id;
        }

        imagesArray = imagesArray + "];";

        sessionCode = sessionCode + ".txt";

        WWWForm form = new WWWForm();
        form.AddField("sessionCode", sessionCode);
        form.AddField("imagesList", images);

        using (UnityWebRequest www = UnityWebRequest.Post(imagesUploadUrl, form))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log("Sorry Error");
                Camera.main.gameObject.GetComponent<ShowInfo>().sendInfo("Error: No network connection.\nCan not save Images.", 3f, 10);
            }
            else
            {
                Debug.Log("Uploaded Images");
                Camera.main.gameObject.GetComponent<ShowInfo>().sendInfo("Successfully saved images", 3f, 10);
            }

        }

        yield return null;
    }
}
