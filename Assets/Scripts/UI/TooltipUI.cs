using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour {

    protected bool isDisappearing = false;
    protected float time = -1f;

    void Awake()
    {
        isDisappearing = false;
        time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (time > 0f && Time.time >= time + 5f)
        {
            Disappear();
        }
    }

    public void Disappear()
    {
        if (isDisappearing) return;
        //Debug.Log("Disappear");
        isDisappearing = true;
        StartCoroutine("FadeOut");
    }

    IEnumerator FadeOut()
    {
        time = -1f;
        int frame = 16;
        for (int i = 0; i < frame; i++)
        {
            foreach (Image im in GetComponentsInChildren<Image>())
            {
                im.color = Color.Lerp(ColorManager.ChangeAlpha(im.color, 1f), ColorManager.ChangeAlpha(im.color, 0f), i / (float)frame);
            }
            foreach (Text t in GetComponentsInChildren<Text>())
            {
                t.color = Color.Lerp(ColorManager.ChangeAlpha(t.color, 1f), ColorManager.ChangeAlpha(t.color, 0f), i / (float)frame);
            }
            yield return null;
        }
        Destroy(gameObject);
    }
}
