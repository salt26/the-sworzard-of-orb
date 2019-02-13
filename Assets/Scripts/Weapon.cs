using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Weapon {

    public string name;
    public Element element;
    public int range;
    public List<KeyValuePair<string, int>> effects = new List<KeyValuePair<string, int>>();
    public delegate void WeaponEffect(int param);
    //public WeaponEffect afterAttackEffect;

    public int Range
    {
        get
        {
            return range;
        }
    }
    
    /// <summary>
    /// 유효 무기 속성을 반환합니다.
    /// 최댓값이 아닌 속성 값은 모두 0이 되며, 사정거리에 따른 대미지 감소가 적용됩니다.
    /// </summary>
    /// <returns></returns>
    public Element ValidElement()
    {
        // 무기의 사정거리만큼 대미지 감소 (예: 사정거리 2이면 대미지는 1/2)
        Element e1 = element * new Vector3(1f / Range, 1f / Range, 1f / Range);
        int max = Mathf.Max(e1.Fire, e1.Ice, e1.Nature);
        int f = max, i = max, n = max;
        if (e1.Fire < max) f = 0;
        if (e1.Ice < max) i = 0;
        if (e1.Nature < max) n = 0;
        return new Element(f, i, n);
    }

    /// <summary>
    /// 기본 공격력인 속성 합을 반환합니다.
    /// 사정거리에 따른 대미지 감소가 적용됩니다.
    /// </summary>
    /// <returns></returns>
    public int BaseAttack()
    {
        Element e = new Element(element.Fire, element.Ice, element.Nature);
        if (Range > 1) e = e * new Vector3(1f / Range, 1f / Range, 1f / Range);
        return e.Sum();
    }


    /// <summary>
    /// 무기의 공격력을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public int Attack()
    {
        // TODO
        return BaseAttack() + ValidElement().Sum();
    }
}
