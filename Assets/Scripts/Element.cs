﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Element {

    public int fire;
    private int ice, nature;

    public Element(int fire, int ice, int nature)
    {
        this.fire = fire;
        this.ice = ice;
        this.nature = nature;
    }

    // TODO 임시 생성자
    public Element(int fire)
    {
        this.fire = fire;
        this.ice = 0;
        this.nature = 0;
    }

    #region 프로퍼티 (Fire, Ice, Nature)

    public int Fire
    {
        get
        {
            return fire;
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
            return ice;
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
            return nature;
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

    public static Element operator *(Vector3 times, Element a)
    {
        return new Element((int)(a.fire * times.x),
            (int)(a.ice * times.y),
            (int)(a.nature * times.z));
    }

    public static Element operator *(Element a, Vector3 times)
    {
        return new Element((int)(a.fire * times.x),
            (int)(a.ice * times.y),
            (int)(a.nature * times.z));
    }

    public static implicit operator Vector3(Element a)
    {
        return new Vector3(a.fire, a.ice, a.nature);
    }
    #endregion
      
    public int Sum()
    {
        return fire + ice + nature;
    }
}
