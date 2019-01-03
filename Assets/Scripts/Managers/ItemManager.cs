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

    /// <summary>
    /// Key는 아이템 ID(오브는 1xx, 기타 아이템은 xx), Value는 아이템 정보입니다.
    /// </summary>
    public Dictionary<int, ItemInfo> itemInfo;
    public GameObject itemPrefab;
    public GameObject orbPrefab;

    public const float sellDiscount = 0.6f; // 아이템 구매 가격에 이 비율이 곱해진 것이 판매 가격

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
    /// 아이템 게임오브젝트를 생성하여 반환합니다.
    /// 아이템 정보가 존재하지 않으면 null을 반환합니다.
    /// </summary>
    /// <param name="id">아이템 ID</param>
    /// <param name="pos">생성할 위치</param>
    /// <returns></returns>
    public GameObject CreateItem(int id, Vector3 pos)
    {
        if (itemInfo.ContainsKey(id))
        {
            if (itemInfo[id].type == ItemInfo.Type.Consumable)
            {
                GameObject g = Instantiate(itemPrefab, pos, Quaternion.identity);
                g.GetComponent<Item>().Initialize(itemInfo[id].name);
                return g;
            }
            else
            {
                GameObject g = Instantiate(orbPrefab, pos, Quaternion.identity);
                g.GetComponent<Orb>().Initialize(itemInfo[id].name);
                return g;
            }
        }
        return null;
    }

    /// <summary>
    /// 아이템 name을 인자로 주면, 해당하는 아이템 정보를 찾아 반환합니다.
    /// 정보가 없으면 null을 반환합니다.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public ItemInfo FindItemInfo(string name)
    {
        foreach (KeyValuePair<int, ItemInfo> ii in itemInfo)
        {
            if (ii.Value.name.Equals(name))
            {
                return ii.Value;
            }
        }
        return null;
    }

    /// <summary>
    /// 아이템 id를 인자로 주면, 해당하는 아이템 정보를 찾아 반환합니다.
    /// 정보가 없으면 null을 반환합니다.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ItemInfo FindItemInfo(int id)
    {
        if (itemInfo.ContainsKey(id)) return itemInfo[id];
        return null;
    }

    /// <summary>
    /// 아이템 name을 인자로 주면, 해당하는 아이템 스프라이트를 반환합니다.
    /// 정보가 없으면 null을 반환합니다.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Sprite GetItemSprite(string name)
    {
        ItemInfo ii = FindItemInfo(name);
        if (ii == null) return null;
        else if (ii.type == ItemInfo.Type.Consumable)
            return Resources.Load("Items/" + StringUtility.ToPascalCase(name), typeof(Sprite)) as Sprite;
        else return Resources.Load("Items/Orbs/" + StringUtility.ToPascalCase(name), typeof(Sprite)) as Sprite;
    }

    /// <summary>
    /// 아이템 id를 인자로 주면, 해당하는 아이템 스프라이트를 반환합니다.
    /// 정보가 없으면 null을 반환합니다.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Sprite GetItemSprite(int id)
    {
        ItemInfo ii = FindItemInfo(id);
        if (ii == null) return null;
        else if (ii.type == ItemInfo.Type.Consumable)
            return Resources.Load("Items/" + StringUtility.ToPascalCase(ii.name), typeof(Sprite)) as Sprite;
        else return Resources.Load("Items/Orbs/" + StringUtility.ToPascalCase(ii.name), typeof(Sprite)) as Sprite;
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

    public string effectName;
    public int effectParam;
    
    /// <summary>
    /// 구매 가격
    /// </summary>
    public int BuyPrice
    {
        get
        {
            return price;
        }
    }

    /// <summary>
    /// 판매 가격
    /// </summary>
    public int SellPrice
    {
        get
        {
            return (int)(price * ItemManager.sellDiscount);
        }
    }

    public bool Use()
    {
        if (type == Type.Consumable)
        {
            return ItemManager.im.InvokeEffect(effectName, effectParam);
        }
        else
        {
            // TODO 오브 사용 시 지금은 현재 장착한 무기에 스탯이 추가됨
            GameManager.gm.player.EquippedWeapon.element += stat;
            GameManager.gm.player.statusUI.UpdateAttackText(GameManager.gm.player.EquippedWeapon);
            return true;
        }
    }
}