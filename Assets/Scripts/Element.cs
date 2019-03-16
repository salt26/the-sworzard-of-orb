using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Element {
    
    [SerializeField]
    private int fire, ice, nature;

    public Element(int fire, int ice, int nature)
    {
        this.fire = fire;
        this.ice = ice;
        this.nature = nature;
    }

    #region 프로퍼티 (Fire, Ice, Nature)

    public int Fire
    {
        get
        {
            return Mathf.Clamp(fire, 0, 999);
        }
        set
        {
            fire = value;
        }
    }

    public int Ice
    {
        get
        {
            return Mathf.Clamp(ice, 0, 999);
        }
        set
        {
            ice = value;
        }
    }

    public int Nature
    {
        get
        {
            return Mathf.Clamp(nature, 0, 999);
        }
        set
        {
            nature = value;
        }
    }
    #endregion

    #region 연산자 오버로드 (+, -, *, Vector3)
    public static Element operator +(Element a, Element b)
    {
        return new Element(a.fire + b.fire, a.ice + b.ice, a.nature + b.nature);
    }

    public static Element operator -(Element a, Element b)
    {
        return new Element(a.fire - b.fire, a.ice - b.ice, a.nature - b.nature);
    }

    /// <summary>
    /// 각 속성 값은 int
    /// </summary>
    /// <param name="times"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public static Element operator *(Vector3 times, Element a)
    {
        return new Element((int)(a.fire * times.x),
            (int)(a.ice * times.y),
            (int)(a.nature * times.z));
    }

    /// <summary>
    /// 각 속성 값은 int
    /// </summary>
    /// <param name="a"></param>
    /// <param name="times"></param>
    /// <returns></returns>
    public static Element operator *(Element a, Vector3 times)
    {
        return new Element((int)(a.fire * times.x),
            (int)(a.ice * times.y),
            (int)(a.nature * times.z));
    }

    public static Element operator *(int times, Element a)
    {
        return new Element(a.fire * times, a.ice * times, a.nature * times);
    }
    
    public static Element operator *(Element a, int times)
    {
        return new Element(a.fire * times, a.ice * times, a.nature * times);
    }

    public static implicit operator Vector3(Element a)
    {
        return new Vector3(a.fire, a.ice, a.nature);
    }
    #endregion

    public override string ToString()
    {
        return "(" + Fire + "/" + Ice + "/" + Nature + ")";
    }

    public int Sum()
    {
        return Mathf.Clamp(fire + ice + nature, 0, 9999);
    }
}
