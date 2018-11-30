using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Element {

    public int fire;
    private int ice, nature;
    private int range = 1;  // 사정거리(무기에만 적용)

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

    #region 프로퍼티 (Fire, Ice, Nature, Range)
    /* TODO
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
    */

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

    public int Range
    {
        get
        {
            return range;
        }
        set
        {
            if (value > 0) range = value;
            else range = 1;
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

    /// <summary>
    /// 유효 방어구 속성을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public Element Armor()
    {
        // TODO 일단 (불 속성) = (기본 방어력)
        return new Element(fire, 0, 0);
    }

    /// <summary>
    /// 유효 무기 속성을 반환합니다.
    /// 최댓값이 아닌 속성 값은 모두 0이 됩니다.
    /// </summary>
    /// <returns></returns>
    public Element Weapon()
    {
        // TODO 일단 (불 속성) = (기본 공격력)
        int max = Mathf.Max(fire, ice, nature);
        int f = max, i = max, n = max;
        if (fire < max) f = 0;
        if (ice < max) i = 0;
        if (nature < max) n = 0;
        Element e = new Element(f, i, n);

        // 무기의 사정거리만큼 대미지 감소 (예: 사정거리 2이면 대미지는 1/2)
        if (Range > 1) e = e * new Vector3(1f / Range, 1f / Range, 1f / Range);
        return e;
    }

    /// <summary>
    /// 방어구의 방어력을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public int Guard()
    {
        // TODO
        return Armor().Sum();
    }

    /// <summary>
    /// 무기의 공격력을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public int Damage()
    {
        // TODO
        return Weapon().Sum();
    }
    
    private int Sum()
    {
        return fire + ice + nature;
    }
}
