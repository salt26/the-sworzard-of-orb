using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

/// <summary>
/// 각종 아이템의 정보를 들고 있습니다.
/// </summary>
public class ItemManager : MonoBehaviour {

    public static ItemManager im;

    [SerializeField]
    private GameObject gold;

    [SerializeField]
    private List<GameObject> items; // TODO 지우기

    /// <summary>
    /// Key는 아이템 ID(오브는 1xx, 기타 아이템은 xx), Value는 아이템 정보입니다.
    /// </summary>
    public Dictionary<int, ItemInfo> itemInfo;

    public GameObject Gold
    {
        get
        {
            return gold;
        }
    }
    
    void Awake()
    {
        im = this;
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

    public GameObject GetItemPrefab(int id)
    {
        // TODO items 대신 itemInfo 사용
        // TODO 리스트 인덱스 대신 id 필드 사용 
        if (id < 0 || id >= items.Count ||
            items[id] == null || items[id].GetComponent<Item>() == null)
            return null;
        else return items[id];
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

    /// <summary>
    /// 아이템의 효과를 발동시킵니다.
    /// </summary>
    /// <param name="effectName">효과 메서드 이름</param>
    /// <param name="param">효과의 수치 인자</param>
    public bool InvokeEffect(string effectName, int param)
    {
        if (effectName == null || effectName.Equals("")) return false;
        MethodInfo mi = typeof(ItemEffect).GetMethod(effectName);
        if (mi == null)
        {
            Debug.LogWarning("There is no effect method named '" + effectName + "'!");
            return false;
        }
        mi.Invoke(null, new object[] { param });
        return true;
    }


    #region 아이템 효과(Effect) 메서드
    
    /// <summary>
    /// 아이템 효과를 정의한 클래스입니다.
    /// </summary>
    private class ItemEffect
    {
        // 이 클래스 안에는 아이템 효과 메서드만 정의되어야 하며,
        // 이 안의 모든 메서드는 int 인자 하나를 받아 void 타입을 반환하는 static 메서드여야 합니다.

        public static void Heal(int heal)
        {
            GameManager.gm.player.Healed(heal);
        }
    }
    
    #endregion
}

[System.Serializable]
public class ItemInfo
{
    public enum Type { Orb, Consumable };

    public string name;     // 아이템 이름(Primary key)
    public Type type;

    public string tooltip;  // 툴팁

    public int level;       // Orb에만 존재
    public Element stat;    // Orb에만 존재

    public int price;       // 상점에서 구매할 때의 가격

    /*
    public Sprite onTileSprite;     // 맵의 타일 위에서 보여질 이미지
    public Sprite inventorySprite;  // 인벤토리에서 보여질 이미지
    */

    public string effectName;
    public int effectParam;
}