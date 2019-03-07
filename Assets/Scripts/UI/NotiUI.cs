﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotiUI : MonoBehaviour
{
    public Text notiText;
    public int notiParam;
    private string originalNotiContent;

    private bool isAnimating;
    private bool stopAnimation;

    // Start is called before the first frame update
    void Start()
    {
        isAnimating = false;
        stopAnimation = false;
        notiParam = 0;
        originalNotiContent = "";
    }
    
    public void SetNotiText(string word)
    {
        StartCoroutine(Animation(word));
    }

    IEnumerator Animation(string word)
    {
        float frame = 120f;
        float frame2 = 20f;
        if (isAnimating)
            stopAnimation = true;
        yield return null;
        isAnimating = true;
        GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 0.8f);
        originalNotiContent = word;
        RefreshNotiText();

        for (int i = 0; i < frame; i++)
        {
            if (stopAnimation)
            {
                break;
            }
            yield return null;
        }

        for (int i = 0; i < frame2; i++)
        {
            if (stopAnimation)
            {
                stopAnimation = false;
                break;
            }
            GetComponent<Image>().color = Color.Lerp(new Color(0.6f, 0.6f, 0.6f, 0.8f), new Color(1f, 1f, 1f, 0f), i / frame2);
            yield return null;
        }
        GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        originalNotiContent = "";
        RefreshNotiText();
        isAnimating = false;
    }
    
    public void RefreshNotiText()
    {
        notiText.text = StringManager.sm.Translate(originalNotiContent).Replace("@", notiParam.ToString());
    }
}