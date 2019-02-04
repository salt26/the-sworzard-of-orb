using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceSprite : MonoBehaviour {

    private bool isDisappearing = false;

	public void Disappear(int frame = 15)
    {
        if (isDisappearing) return;
        StartCoroutine(DisappearAnimation(frame));
    }

    IEnumerator DisappearAnimation(int frame)
    {
        isDisappearing = true;
        Color c = GetComponent<SpriteRenderer>().color;
        for (int i = 0; i < frame; i++)
        {
            GetComponent<SpriteRenderer>().color = Color.Lerp(c, ColorManager.ChangeAlpha(c, 0f), i / (float)frame);
            yield return null;
        }
        GetComponent<SpriteRenderer>().color = ColorManager.ChangeAlpha(c, 0f);
        Destroy(gameObject);
    }
}
