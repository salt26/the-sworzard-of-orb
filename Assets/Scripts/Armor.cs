using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Armor {

    public Element element;
    public ItemManager.Effect armorEffect;

    /// <summary>
    /// 유효 방어구 속성을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public Element ValidElement
    {
        get
        {
            return new Element(element.Fire, element.Ice, element.Nature);
        }
    }
    
    /// <summary>
    /// 방어구의 방어력을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public int Defense()
    {
        // TODO
        return Mathf.Clamp(BaseDefense() + ValidElement.Sum(), 0, 999);
    }

    /// <summary>
    /// 기본 방어력인 속성 합을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public int BaseDefense()
    {
        return Mathf.Clamp(element.Sum(), 0, 99);
    }

    /// <summary>
    /// 상대 무기로부터 받는 실제 대미지를 계산하여 반환합니다.
    /// </summary>
    /// <param name="otherWeapon">상대 무기</param>
    /// <param name="bonusDamage">곱해질 추가 대미지</param>
    /// <returns></returns>
    public int ComputeDamage(Weapon otherWeapon, bool isCharge)
    {
        //Mathf.Max(0, (int)(bonusDamage * otherWeapon.Attack()) - Defense())
        int baseDamage;
        if (isCharge)
            baseDamage = Mathf.Clamp((int)(otherWeapon.chargeBonus * otherWeapon.BaseAttack()) - BaseDefense(), 0, 999);
        else
            baseDamage = Mathf.Clamp(otherWeapon.BaseAttack() - BaseDefense(), 0, 999);
        int fire = Mathf.Clamp(otherWeapon.ValidElement.Fire - ValidElement.Fire, 0, 99);
        int ice = Mathf.Clamp(otherWeapon.ValidElement.Ice - ValidElement.Ice, 0, 99);
        int nature = Mathf.Clamp(otherWeapon.ValidElement.Nature - ValidElement.Nature, 0, 99);

        return Mathf.Clamp(baseDamage + fire + ice + nature, 0, 999);
    }
}
