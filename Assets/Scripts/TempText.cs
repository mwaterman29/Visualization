using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class TempText : MonoBehaviour
{

    public static TMP_Text tempText; // text to assign temporarily to
    public TMP_Text tempTextReference;

    private static float timer; //time for fade in out

    private static float length; //length of current time

    public static Color textColor; // text color

    public static float fadeInDec, fadeOutDec;
    public static int seqIndex;
    private static List<(string, int)> seqSet = new List<(string, int)>();

    // Start is called before the first frame update
    void Start()
    {
        //init
        fadeInDec = fadeOutDec = 0.2f;
        textColor = new Color(1, 1, 1, 1);
        assignTempText(tempTextReference);
        //setTempText("testo", 4);
    }



    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            if (seqIndex == seqSet.Count)
            {
                tempText.text = "";
            }
            else
            {
                setTempText(seqSet[seqIndex].Item1, seqSet[seqIndex].Item2);
                seqIndex++;
            }
        }
    }

    void LateUpdate()
    {
        float percentage = timer / length;
        if (percentage > (1 - fadeInDec))
        {
            textColor.a = ((1 - percentage) / fadeInDec);
        }
        else if (percentage < (1 - fadeInDec) && percentage > fadeOutDec)
        {
            textColor.a = 1;
        }
        else if (percentage < fadeOutDec)
        {
            textColor.a = percentage / fadeOutDec;
        }
        if (timer == 0)
        {
            tempText.text = ""; // delete text;
        }
        tempText.color = textColor;
    }
    public static void setTempText(string text, float time)
    {
        //Debug.Log($"temp text set: {text} for {time}");
        length = time;
        timer = time;
        tempText.text = text;
    }

    public static void interruptEnd()
    {
        seqSet.Clear();
        seqIndex = 0;
        timer = -1;
        tempText.text = "";
    }

    public static void sequencer(string sequence, char split)
    {
        seqSet.Clear();

        string[] parts = sequence.Split(split);
        for (int i = 0; i < parts.Length; i += 2)
        {
            seqSet.Add((parts[i], int.Parse(parts[i + 1])));
        }

        seqIndex = 0;
    }

    public static void assignTempText(TMP_Text t)
    {
        tempText = t;
    }

    public static void assignTextColor(Color c)
    {
        textColor = c;
    }
}


