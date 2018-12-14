using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 각종 아이템 프리팹의 레퍼런스를 들고 있습니다.
/// </summary>
public class ItemManager : MonoBehaviour {

    public static ItemManager im;

    [SerializeField]
    private GameObject gold;

    [SerializeField]
    private List<GameObject> items;

    public GameObject Gold
    {
        get
        {
            return gold;
        }
    }

    /// <summary>
    /// 아이템 name을 인자로 주면, 해당하는 아이템 프리팹을 찾아 반환합니다.
    /// 프리팹이 없으면 null을 반환합니다.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject GetItemPrefab(string name)
    {
        foreach (GameObject g in items)
        {
            if (g == null || g.GetComponent<Item>() == null)
            {
                continue;
            }
            else if (g.GetComponent<Item>().name.Equals(name))
            {
                return g;
            }
        }
        return null;
    }

    /// <summary>
    /// 아이템 name이 등록된 아이템인지 확인합니다.
    /// 등록된 아이템이면 true를 반환합니다.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool IsRegisteredItem(string name)
    {
        foreach (GameObject g in items)
        {
            if (g == null || g.GetComponent<Item>() == null)
            {
                continue;
            }
            else if (g.GetComponent<Item>().name.Equals(name))
            {
                return true;
            }
        }
        return false;
    }

    void Awake()
    {
        im = this;
    }
    
}
