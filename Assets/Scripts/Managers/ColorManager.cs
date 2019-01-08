using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour {

    public static ColorManager cm;

    public Color baseColor;
    public Color fireColor;
    public Color iceColor;
    public Color natureColor;

    void Awake()
    {
        cm = this;
    }

    public static Color ChangeAlpha(Color c, float alpha)
    {
        return new Color(c.r, c.g, c.b, alpha);
    }
}
