using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    Image screen;
    bool isAnimating;

    // Use this for initialization
    void Awake()
    {
        screen = GetComponent<Image>();
        isAnimating = false;
    }

    public void StartEffect()
    {
        if (isAnimating) return;
        StartCoroutine(DamagedEffect());
    }

    IEnumerator DamagedEffect()
    {
        isAnimating = true;

        float frame1 = 80f;
        Color originColor = new Color(1f, 1f, 1f, 0f);
        Color destColor = new Color(1f, 1f, 1f, 1f);

        for (int i = 0; i < frame1; i++)
        {
            screen.color = Color.Lerp(originColor, destColor, i / frame1);
            yield return null;
        }
        screen.color = new Color(1f, 1f, 1f, 1f);
    }
}
