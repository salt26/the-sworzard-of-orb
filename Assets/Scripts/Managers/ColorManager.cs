using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour {

    public static ColorManager cm;

    public Color baseColor;
    public Color fireColor;
    public Color iceColor;
    public Color natureColor;
    public List<Color> monsterNumberColors;
    public List<Color> levelColors;

    void Awake()
    {
        cm = this;
    }

    public static Color ChangeAlpha(Color c, float alpha)
    {
        return new Color(c.r, c.g, c.b, alpha);
    }
    
    /// <summary>
    /// 이미지에서 가장 자주 등장하는 색을 찾아 반환합니다.
    /// 이미지가 픽셀 그림이고 사용된 색의 종류가 많지 않다고 가정합니다.
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    public static Color ExtractRepresentative(Texture2D image)
    {
        Color[] colors = image.GetPixels();
        Dictionary<Color, int> count = new Dictionary<Color, int>();
        foreach (Color c in colors)
        {
            if (c.a < 0.5f) continue;

            if (count.ContainsKey(c)) count[c]++;
            else count.Add(c, 1);
        }
        int max = -1;
        Color argmax = new Color();
        foreach (KeyValuePair<Color, int> p in count)
        {
            if (p.Value > max)
            {
                max = p.Value;
                argmax = p.Key;
            }
        }
        return argmax;
    }
}
