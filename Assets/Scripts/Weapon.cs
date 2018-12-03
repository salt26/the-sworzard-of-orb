using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Weapon {

    public Element element;

    [SerializeField]
    private int range;
    [SerializeField]
    private Sprite weaponMarkSprite;
    [SerializeField]
    private Sprite characterSprite;
    [SerializeField]
    private string name;

    public int Range
    {
        get
        {
            return range;
        }
    }

    public Sprite CharacterSprite
    {
        get
        {
            return characterSprite;
        }
    }

    public Sprite WeaponSprite
    {
        get
        {
            return weaponMarkSprite;
        }
    }
    
    /// <summary>
    /// 유효 무기 속성을 반환합니다.
    /// 최댓값이 아닌 속성 값은 모두 0이 됩니다.
    /// </summary>
    /// <returns></returns>
    public Element ValidElement()
    {
        // TODO 일단 (불 속성) = (기본 공격력)
        int max = Mathf.Max(element.Fire, element.Ice, element.Nature);
        int f = max, i = max, n = max;
        if (element.Fire < max) f = 0;
        if (element.Ice < max) i = 0;
        if (element.Nature < max) n = 0;
        Element e = new Element(f, i, n);

        // 무기의 사정거리만큼 대미지 감소 (예: 사정거리 2이면 대미지는 1/2)
        if (Range > 1) e = e * new Vector3(1f / Range, 1f / Range, 1f / Range);
        return e;
    }


    /// <summary>
    /// 무기의 공격력을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public int Damage()
    {
        // TODO
        return ValidElement().Sum();
    }
}
