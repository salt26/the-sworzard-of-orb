using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Weapon {

    public string name;
    public Element element;
    public int range;
    public ItemManager.Effect afterAttackEffect;
    
    private float[] pureAmpBonus = new float[3] { 0f, 0f, 0f };  // Amp 효과로 인한 원소 스탯 증가 비율

    public int Range
    {
        get
        {
            return Mathf.Clamp(range, 1, int.MaxValue);
        }
    }

    /// <summary>
    /// 오브의 FireAmp 효과로 인해 순수하게 오르는 불 속성 대미지의 비율입니다.
    /// 계산된 대미지는 기본 공격력에는 영향을 주지 않고, 사정거리에 영향을 받지 않습니다.
    /// </summary>
    public float FireAmpBonus
    {
        get
        {
            return pureAmpBonus[0];
        }
        set
        {
            pureAmpBonus[0] = value;
        }
    }

    /// <summary>
    /// 오브의 IceAmp 효과로 인해 순수하게 오르는 얼음 속성 대미지의 비율입니다.
    /// 계산된 대미지는 기본 공격력에는 영향을 주지 않고, 사정거리에 영향을 받지 않습니다.
    /// </summary>
    public float IceAmpBonus
    {
        get
        {
            return pureAmpBonus[1];
        }
        set
        {
            pureAmpBonus[1] = value;
        }
    }

    /// <summary>
    /// 오브의 NatureAmp 효과로 인해 순수하게 오르는 자연 속성 대미지의 비율입니다.
    /// 계산된 대미지는 기본 공격력에는 영향을 주지 않고, 사정거리에 영향을 받지 않습니다.
    /// </summary>
    public float NatureAmpBonus
    {
        get
        {
            return pureAmpBonus[2];
        }
        set
        {
            pureAmpBonus[2] = value;
        }
    }

    /// <summary>
    /// 무기 속성을 가공하지 않은 채로 그대로 반환합니다. 읽기 전용입니다.
    /// </summary>
    /// <returns></returns>
    public Element OriginalElement
    {
        get
        {
            return element.Clone();
        }
    }

    /// <summary>
    /// 유효 무기 속성을 반환합니다.
    /// 원래 무기 속성에 대해 사정거리에 따른 대미지 감소를 적용한 후,
    /// 최댓값이 아닌 속성 값은 모두 0으로 만듭니다.
    /// 이후에 Amp 효과를 적용한 대미지를 더하여 반환합니다.
    /// </summary>
    /// <returns></returns>
    public Element ValidElement
    {
        get
        {
            // 무기의 사정거리만큼 대미지 감소 (예: 사정거리 2이면 대미지는 1/2)
            Element e1 = element * new Vector3(1f / Range, 1f / Range, 1f / Range);
            int max = Mathf.Max(e1.Fire, e1.Ice, e1.Nature);
            int f = max, i = max, n = max;
            if (e1.Fire < max) f = 0;
            if (e1.Ice < max) i = 0;
            if (e1.Nature < max) n = 0;
            return new Element(f, i, n) + PureAmpElement;
        }
    }

    /// <summary>
    /// Amp 효과로 순수하게 추가되는 대미지를 반환합니다.
    /// 무기의 사정거리에 따른 대미지 감소를 적용받지 않습니다.
    /// </summary>
    public Element PureAmpElement
    {
        get
        {
            return OriginalElement * (new Vector3(pureAmpBonus[0], pureAmpBonus[1], pureAmpBonus[2]));
        }
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
        return Mathf.Clamp(e.Sum(), 0, 99);
    }


    /// <summary>
    /// 무기의 공격력을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public int Attack()
    {
        // TODO
        return Mathf.Clamp(BaseAttack() + ValidElement.Sum(), 0, 999);
    }
}
