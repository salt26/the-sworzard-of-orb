using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

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

    /// <summary>
    /// 오브 조합 레시피입니다. Key는 OrbIngredient, Value는 조합 결과로 나오는 오브의 id입니다.
    /// </summary>
    public Dictionary<OrbIngredient, int> orbRecipe;
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
    /// 아이템 name을 인자로 주면, 해당하는 아이템 id를 찾아 반환합니다.
    /// 정보가 없으면 -1을 반환합니다.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int FindItemID(string name)
    {
        foreach (KeyValuePair<int, ItemInfo> ii in itemInfo)
        {
            if (ii.Value.name.Equals(name))
            {
                return ii.Key;
            }
        }
        return -1;
    }

    /// <summary>
    /// 세 재료 오브 name을 인자로 주면, 이들을 조합해 나오는 오브의 id를 반환합니다.
    /// 만약 조합 레시피가 존재하지 않으면 -1을 반환합니다.
    /// 만약 오브가 아닌 것이 포함되어 있으면 -2를 반환합니다.
    /// </summary>
    /// <param name="orbName1"></param>
    /// <param name="orbName2"></param>
    /// <param name="orbName3"></param>
    /// <returns></returns>
    public int FindOrbCombResultID(string orbName1, string orbName2, string orbName3)
    {
        int id1 = FindItemID(orbName1), id2 = FindItemID(orbName2), id3 = FindItemID(orbName3);
        if (id1 >= 100 && id2 >= 100 && id3 >= 100)
        {
            OrbIngredient oi = new OrbIngredient(id1, id2, id3);
            if (orbRecipe.ContainsKey(oi))
            {
                return orbRecipe[oi];
            }
            else return -1;
        }
        return -2;
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
    public bool InvokeItemEffect(string effectName, int param)
    {
        if (effectName == null || effectName.Equals("")) return false;
        MethodInfo mi = typeof(ItemEffect).GetMethod(effectName);
        if (mi == null)
        {
            Debug.LogWarning("There is no item effect method named '" + effectName + "'!");
            return false;
        }
        return (bool)mi.Invoke(null, new object[] { param });
    }

    public Action GetOrbEffect(string effectName, int param)
    {
        if (effectName == null) return null;
        MethodInfo mi = typeof(OrbEffect).GetMethod(effectName);
        if (mi == null)
        {
            Debug.LogWarning("There is no orb effect method named '" + effectName + "'!");
            return null;
        }
        return () => mi.Invoke(null, new object[] { param });
    }


    #region 아이템 효과(Effect) 메서드
    
    /// <summary>
    /// 아이템 효과를 정의한 클래스입니다.
    /// </summary>
    private class ItemEffect
    {
        // 이 클래스 안에는 아이템 효과 메서드만 정의되어야 하며,
        // 이 안의 모든 메서드는 int 인자 하나를 받아 bool 타입을 반환하는 static 메서드여야 합니다.
        // 반환하는 값은 아이템이 성공적으로 사용되었는지 여부입니다.

        public static bool Heal(int heal)
        {
            GameManager.gm.player.Healed(heal);
            return true;
        }

        public static bool Transform(int level)
        {
            Inventory inv = GameManager.gm.player.GetComponent<Inventory>();

            List<ItemInfo> l = new List<ItemInfo>();
            foreach (KeyValuePair<int, ItemInfo> p in ItemManager.im.itemInfo)
            {
                // TODO 레벨에 맞는 오브만 골라서 리스트 만들기
                if (p.Value.type == ItemInfo.Type.Orb && p.Value.level == level)
                {
                    l.Add(p.Value);
                }
            }

            return inv.AddItem(l[UnityEngine.Random.Range(0, l.Count)].name);
        }
    }

    #endregion

    #region 오브 효과(Effect) 메서드

    private class OrbEffect
    {
        public static void Stun(int probability)
        {

        }

        public static void Intoxicate(int poisonDamage)
        {

        }

        public static void Flurry(int splashDamage)
        {

        }
    }

    #endregion
}

[System.Serializable]
public class ItemInfo
{
    public enum Type { Orb, Consumable };
    public enum Usage { None, Weapon, Armor };

    public string name;     // 아이템 이름(Primary key)
    public Type type;

    public string tooltip;  // 툴팁

    public int level;       // Orb에만 존재
    public Usage usage;     // Orb에만 존재
    public Element stat;    // Orb에만 존재

    public int price;       // 상점에서 구매할 때의 가격

    public string effectName = "None";
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
            return ItemManager.im.InvokeItemEffect(effectName, effectParam);
        }
        else
        {
            // TODO 오브 사용 시 지금은 현재 장착한 무기에 스탯이 추가됨
            if (usage == Usage.None) return false;
            else if (usage == Usage.Weapon)
            {
                GameManager.gm.player.EquippedWeapon.element += stat;
                GameManager.gm.player.statusUI.UpdateAttackText(GameManager.gm.player.EquippedWeapon);
                if (effectName != null && !effectName.Equals("None"))
                {
                    //GameManager.gm.player.EquippedWeapon.effects.Add(new KeyValuePair<string, int>(effectName, effectParam));
                    GameManager.gm.player.EquippedWeapon.afterAttackEffect += ItemManager.im.GetOrbEffect(effectName, effectParam);
                    
                }
                return true;
            }
            else if (usage == Usage.Armor)
            {
                GameManager.gm.player.armor.element += stat;
                GameManager.gm.player.statusUI.UpdateDefenseText(GameManager.gm.player.armor);
                if (effectName != null && !effectName.Equals("None"))
                {
                    GameManager.gm.player.EquippedWeapon.effects.Add(new KeyValuePair<string, int>(effectName, effectParam));
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

/// <summary>
/// 어떤 오브의 재료가 되는 세 오브의 id를 정렬하여 가질 수 있는 자료 구조입니다.
/// 인덱싱과 Equals()를 지원합니다.
/// </summary>
public class OrbIngredient
{
    private List<int> IDs;

    public OrbIngredient(int orbID1, int orbID2, int orbID3)
    {
        IDs = new List<int>()
        {
            orbID1,
            orbID2,
            orbID3
        };
        IDs.Sort();
    }

    public int this[int i]
    {
        get { return IDs[i]; }
    }

    public override bool Equals(object obj)
    {
        var v = obj as OrbIngredient;
        if (v != null)
        {
            return this[0] == v[0] && this[1] == v[1] && this[2] == v[2];
        }
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + this[0].GetHashCode();
            hash = hash * 23 + this[1].GetHashCode();
            hash = hash * 23 + this[2].GetHashCode();
            return hash;
        }
    }
}