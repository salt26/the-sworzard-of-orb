using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Armor {

    public Element element;
    public List<KeyValuePair<string, int>> effects = new List<KeyValuePair<string, int>>();

    /// <summary>
    /// 유효 방어구 속성을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public Element ValidElement()
    {
        return new Element(element.Fire, element.Ice, element.Nature);
    }
    
    /// <summary>
    /// 방어구의 방어력을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public int Defense()
    {
        // TODO
        return BaseDefense() + ValidElement().Sum();
    }

    /// <summary>
    /// 기본 방어력인 속성 합을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public int BaseDefense()
    {
        return element.Sum();
    }

    /// <summary>
    /// 상대 무기로부터 받는 실제 대미지를 계산하여 반환합니다.
    /// </summary>
    /// <param name="otherWeapon">상대 무기</param>
    /// <param name="bonusDamage">곱해질 추가 대미지</param>
    /// <returns></returns>
    public int ComputeDamage(Weapon otherWeapon, float bonusDamage = 1f)
    {
        //Mathf.Max(0, (int)(bonusDamage * otherWeapon.Attack()) - Defense())

        int baseDamage = Mathf.Max(0, (int)(bonusDamage * otherWeapon.BaseAttack()) - BaseDefense());
        int fire = Mathf.Max(0, otherWeapon.ValidElement().Fire - ValidElement().Fire);
        int ice = Mathf.Max(0, otherWeapon.ValidElement().Ice - ValidElement().Ice);
        int nature = Mathf.Max(0, otherWeapon.ValidElement().Nature - ValidElement().Nature);

        return baseDamage + fire + ice + nature;
    }
}
