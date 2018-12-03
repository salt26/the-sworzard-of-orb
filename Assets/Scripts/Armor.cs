using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Armor {

    public Element element;
    
    /// <summary>
    /// 유효 방어구 속성을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public Element ValidElement()
    {
        // TODO 일단 (불 속성) = (기본 방어력)
        return new Element(element.Fire, 0, 0);
    }
    
    /// <summary>
    /// 방어구의 방어력을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public int Guard()
    {
        // TODO
        return ValidElement().Sum();
    }
}
