using System.Collections;
using System.Globalization;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System.Resources;

[assembly: NeutralResourcesLanguageAttribute("en-US")]

public class LoadData : MonoBehaviour
{

    public List<Quest> quests = new List<Quest>();

    public int imagesNumber;
    public bool loadAllImages;

    private void Awake()
    {
        //initialize LeanTween
        LeanTween.init(90000);
    }

    
    // Start is called before the first frame update
    void Start()
    {
        //Set country settings to US
        CultureInfo culture = null;
        culture = CultureInfo.CreateSpecificCulture("en-US");
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        TextAsset img2vec = Resources.Load<TextAsset>("img2vec");

        string[] data = img2vec.text.Split(new char[] { '\n' });

        //Should all Images be loaded?
        if (loadAllImages == true)
        {
            imagesNumber = data.Length;
        }
        Data.maxImages = imagesNumber;
        Debug.Log("Maximum Images: " + Data.maxImages);
        Debug.Log("Total Images: " + (data.Length - 1));

        for (int i = 1; i < data.Length; i++)
        {
            string[] row = data[i].Split(new char[] { ';' });

            Quest q = new Quest();

            q.path = row[0];
            float.TryParse(row[1], out q.tsneX);
            float.TryParse(row[2], out q.tsneY);
            float.TryParse(row[3], out q.tsneZ);
            

            string[] colorStrs = row[4].Split(new char[] { '|' });
            Color32[] colors = new Color32[colorStrs.Length];
            for (int e = 0; e < colorStrs.Length; e++)
            {
                string[] rgbs = colorStrs[e].Split(new char[] { ',' });
                int r = int.Parse(rgbs[0]);
                int g = int.Parse(rgbs[1]);
                int b = int.Parse(rgbs[2]);

                Color32 thisColor = new Color32((byte) r, (byte) g, (byte)b, 255);
                colors[e] = thisColor;
            }
            q.colors = colors;
            
            string[] percentStrs = row[5].Split(new char[] { ',' });
            float[] imageColPercent = new float[percentStrs.Length];
            for (int e = 0; e < percentStrs.Length; e++)
            {
                imageColPercent[e] = float.Parse(percentStrs[e]);
            }
            q.imageColPercent = imageColPercent;

            float.TryParse(row[6], out q.abstractLevel);
            float.TryParse(row[7], out q.composition);

            q.id = row[8];

            q.playerDist = 0;

            quests.Add(q);
        }

    }
}
