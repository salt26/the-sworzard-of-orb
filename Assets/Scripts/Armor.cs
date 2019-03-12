using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Armor {

    public Element element;
    //public ItemManager.Effect armorEffect;
    public List<KeyValuePair<string, int>> armorEffects = new List<KeyValuePair<string, int>>();
    //public ItemManager.Effect activeArmorEffect;    // 해당 캐릭터의 턴이 끝날 때 발동, 몬스터만 사용
    public List<KeyValuePair<string, int>> activeArmorEffects = new List<KeyValuePair<string, int>>();  // 해당 캐릭터의 턴이 끝날 때 발동, 몬스터만 사용
    public List<KeyValuePair<string, int>> effects = new List<KeyValuePair<string, int>>();
    
    public Dictionary<string, int> Effects
    {
        get
        {
            Dictionary<string, int> e = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> p in effects)
            {
                if (e.ContainsKey(p.Key))
                {
                    e[p.Key] += p.Value;
                }
                else
                {
                    e.Add(p.Key, p.Value);
                }
            }
            return e;
        }
    }

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
    
    public void InvokeArmorEffects(Character target)
    {
        foreach (KeyValuePair<string, int> p in armorEffects)
        {
            ItemManager.im.GetOrbEffect(p.Key, p.Value)(target);
        }
    }
    
    public void InvokeActiveArmorEffects(Character target)
    {
        foreach (KeyValuePair<string, int> p in activeArmorEffects)
        {
            ItemManager.im.GetOrbEffect(p.Key, p.Value)(target);
        }
    }
}
