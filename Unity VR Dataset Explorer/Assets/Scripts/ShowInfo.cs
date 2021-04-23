using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ShowInfo : MonoBehaviour
{
    public GameObject infoTextObj;
    public GameObject infoBackground;
    public float fadingTime;
    private float infoTime;

    TextMeshPro info;
    SpriteRenderer background;

    bool infoIsShowing;

    public Vector3 bottomLeftPos;

    // Start is called before the first frame update
    void Start()
    {
        infoTextObj.SetActive(true);
        info = infoTextObj.GetComponent<TextMeshPro>();
        background = infoBackground.GetComponent<SpriteRenderer>();
        infoTextObj.SetActive(false);
        infoIsShowing = false;
    }

    public void sendInfo(string infoText, float sendedInfoTime, int position)
    {
        if (infoIsShowing == false)
        {
            //Set Position
            if(position == 0)
            {
                infoTextObj.transform.localPosition = bottomLeftPos;
            } else
            {
                infoTextObj.transform.localPosition = new Vector3(0, 0, infoTextObj.transform.localPosition.z);
            }


            infoIsShowing = true;
            infoTextObj.SetActive(true);
            info.color = new Color32(255, 255, 255, 0);
            background.color = new Color32(10, 10, 10, 0);
            info.text = infoText;
            infoTime = sendedInfoTime;
            StartCoroutine("SetGridWidth");
            //Fade In
            LeanTween.value(gameObject, updateColorAlpha, 0, 255, fadingTime).setOnComplete(startTimer);
        }

    }
    void startTimer()
    {
        StartCoroutine("showInfoForTime");
    }
    IEnumerator showInfoForTime()
    {
        yield return new WaitForSeconds(infoTime);
        //Fade Out
        LeanTween.value(gameObject, updateColorAlpha, 255, 0, fadingTime).setOnComplete(disableInfo);
    }
    void disableInfo()
    {
        infoTextObj.SetActive(false);
        infoIsShowing = false;
    }

    void updateColorAlpha(float alpha)
    {
        info.color = new Color32(255, 255, 255, (byte)alpha);
        background.color = new Color32(10, 10, 10, (byte)(alpha * 0.6f));
    }

    public IEnumerator SetGridWidth()
    {
        yield return new WaitForEndOfFrame();
        background.size = new Vector2(infoTextObj.GetComponent<RectTransform>().rect.width * 13 + 3, infoTextObj.GetComponent<RectTransform>().rect.height * 13 + 1.5f);
    }

    private void Update()
    {
        
        //Test
        if (Input.GetKeyDown(KeyCode.T))
        {
            sendInfo("YoYo My little Bro. This ist ein Text zum Texten. Not more but not weniger. Cheesssy", 5, 0);
        }

        transform.LookAt(Camera.main.transform);
        
    }
}
