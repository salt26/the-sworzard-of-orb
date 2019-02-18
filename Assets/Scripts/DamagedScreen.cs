using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamagedScreen : MonoBehaviour {

    Image screen;
    bool isAnimating;
    bool stop;

	// Use this for initialization
	void Awake () {
        screen = GetComponent<Image>();
        isAnimating = false;
        stop = false;
	}
	
	public void StartEffect(bool isDangerous)
    {
        if (isAnimating) return;
        stop = true;
        StartCoroutine(DamagedEffect(isDangerous));
    }

    IEnumerator DamagedEffect(bool isDangerous)
    {
        isAnimating = true;

        float frame1 = 8f, frame2 = 32f;
        Color originColor = screen.color;
        Color destColor = new Color(1f, 1f, 1f, 0.5f);
        if (isDangerous) destColor = new Color(1f, 1f, 1f, 1f);

        for (int i = 0; i < frame1 + frame2; i++)
        {
            if (i < frame1)
            {
                screen.color = Color.Lerp(originColor, destColor, i / frame1);
            }
            else
            {
                if (i == frame1)
                {
                    isAnimating = false;
                    stop = false;
                }
                screen.color = Color.Lerp(new Color(1f, 1f, 1f, 0f), destColor, (frame1 + frame2 - i) / frame2);
                if (!isAnimating && stop)
                {
                    stop = false;
                    yield break;
                }
            }
            yield return null;
        }
        screen.color = new Color(1f, 1f, 1f, 0f);
    }
}
