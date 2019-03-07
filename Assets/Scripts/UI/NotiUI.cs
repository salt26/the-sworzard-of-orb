using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotiUI : MonoBehaviour
{
    public Text notiText;
    private string originalNotiContent;

    private bool isAnimating;
    private bool stopAnimation;

    // Start is called before the first frame update
    void Start()
    {
        isAnimating = false;
        stopAnimation = false;
    }
    
    public void SetNotiText(string word)
    {
        StartCoroutine(Animation(word));
    }

    IEnumerator Animation(string word)
    {
        float frame = 100f;
        float frame2 = 20f;
        if (isAnimating)
            stopAnimation = true;
        yield return null;
        isAnimating = true;
        GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.4745098f);
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
            GetComponent<Image>().color = Color.Lerp(new Color(1f, 1f, 1f, 0.4745098f), new Color(1f, 1f, 1f, 0f), i / frame2);
            yield return null;
        }
        GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        originalNotiContent = "";
        RefreshNotiText();
        isAnimating = false;
    }
    
    public void RefreshNotiText()
    {
        notiText.text = StringManager.sm.Translate(originalNotiContent);
    }
}
