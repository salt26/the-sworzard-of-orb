using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public delegate void Effect(Character target);

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
            return Resources.Load("Items/" + StringManager.ToPascalCase(name), typeof(Sprite)) as Sprite;
        else return Resources.Load("Items/Orbs/" + StringManager.ToPascalCase(name), typeof(Sprite)) as Sprite;
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
            return Resources.Load("Items/" + StringManager.ToPascalCase(ii.name), typeof(Sprite)) as Sprite;
        else return Resources.Load("Items/Orbs/" + StringManager.ToPascalCase(ii.name), typeof(Sprite)) as Sprite;
    }

    /// <summary>
    /// 아이템의 효과를 발동시킵니다.
    /// </summary>
    /// <param name="effectName">효과 메서드 이름</param>
    /// <param name="param">효과의 수치 인자</param>
    public bool InvokeItemEffect(string effectName, int param)
    {
        if (effectName == null || effectName.Equals("")) return false;
        MethodInfo mi = typeof(ItemEffect).GetMethod(effectName, BindingFlags.Public | BindingFlags.Static);
        if (mi == null)
        {
            Debug.LogWarning("There is no item effect method named '" + effectName + "'!");
            return false;
        }
        return (bool)mi.Invoke(null, new object[] { param });
    }

    /// <summary>
    /// 아이템의 효과를 발동시키지 않고 반환값만 받습니다.
    /// </summary>
    /// <param name="effectName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public bool InvokeItemUsage(string effectName, int param)
    {
        if (effectName == null || effectName.Equals("")) return false;
        MethodInfo mi = typeof(ItemEffect).GetMethod(effectName + "WithNoEffect", BindingFlags.Public | BindingFlags.Static);
        if (mi == null)
        {
            Debug.LogWarning("There is no item effect method named '" + effectName + "'!");
            return false;
        }
        return (bool)mi.Invoke(null, new object[] { param });
    }

    public Effect GetOrbEffect(string effectName, int param)
    {
        if (effectName == null) return null;
        MethodInfo mi = typeof(OrbEffect).GetMethod(effectName, BindingFlags.Public | BindingFlags.Static);
        if (mi == null)
        {
            Debug.LogWarning("There is no orb effect method named '" + effectName + "'!");
            return null;
        }
        return (Character target) => mi.Invoke(null, new object[] { param, target });
    }


    #region 아이템 효과(Effect) 메서드
    
    /// <summary>
    /// 아이템 효과를 정의한 클래스입니다.
    /// </summary>
    private class ItemEffect
    {
        // 이 클래스 안에는 아이템 효과 메서드(+아이템 사용 가능 조건만 반환하는 메서드)만 정의되어야 하며,
        // 이 안의 모든 메서드는 int 인자 하나를 받아 bool 타입을 반환하는 static 메서드여야 합니다.
        // 반환하는 값은 아이템이 성공적으로 사용되었는지 여부입니다.

        public static bool Heal(int heal)
        {
            Character player = GameManager.gm.player;
            if (player.currentHealth == player.MaxHealth && player.poisonDamage == 0)
            {
                return false;
            }
            player.Healed(heal);
            player.poisonDamage = 0;
            return true;
        }

        public static bool HealWithNoEffect(int heal)
        {
            Character player = GameManager.gm.player;
            if (player.currentHealth == player.MaxHealth && player.poisonDamage == 0)
            {
                return false;
            }
            return true;
        }

        public static bool Transform(int level)
        {
            Inventory inv = GameManager.gm.player.GetComponent<Inventory>();

            List<ItemInfo> l = new List<ItemInfo>();
            foreach (KeyValuePair<int, ItemInfo> p in ItemManager.im.itemInfo)
            {
                // 레벨에 맞는 오브만 골라서 리스트 만들기
                if (p.Value.type == ItemInfo.Type.Orb && p.Value.level == level)
                {
                    l.Add(p.Value);
                }
            }

            return inv.AddItem(l[Random.Range(0, l.Count)].name);
        }

        public static bool TransformWithNoEffect(int level)
        {
            Inventory inv = GameManager.gm.player.GetComponent<Inventory>();

            if (inv.Items.Count >= inv.maxItemNumber)
            {
                return false;
            }
            return true;
        }

        public static bool Return(int anyInt = 0)
        {
            if (!GameManager.gm.IsSceneLoaded || SceneManager.GetActiveScene().name.Equals("Town"))
            {
                return false;
            }
            GameManager.gm.ChangeScene("Town", null);
            return true;
        }

        public static bool ReturnWithNoEffect(int anyInt = 0)
        {
            if (!GameManager.gm.IsSceneLoaded || SceneManager.GetActiveScene().name.Equals("Town"))
            {
                return false;
            }
            return true;
        }
    }

    #endregion

    #region 오브 효과(Effect) 메서드

    private class OrbEffect
    {
        public static void Stun(int probability, Character target)
        {
            if (Random.Range(0f, 1f) < probability / 100f)
            {
                target.hasStuned = true;
            }
        }

        public static void Intoxicate(int poisonDamage, Character target)
        {
            target.poisonDamage += poisonDamage;
            target.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.5f, 0.8f, 1f);
        }

        public static void Flurry(int splashDamage, Character target)
        {
            // Flurry 효과는 몬스터의 무기에 적용할 수 없음 (플레이어가 한 명이므로)
            //Debug.Log("Flurry " + splashDamage + " to " + target.name);

            Vector3 pos = VectorUtility.PositionToInt(target.GetComponent<Transform>().position);
            Vector3[] direction = new Vector3[4] {
                new Vector3(1f, 0f, 0f), new Vector3(-1f, 0f, 0f),
                new Vector3(0f, 1f, 0f), new Vector3(0f, -1f, 0f) };
            foreach (Vector3 d in direction)
            {
                GameObject g = Instantiate(GameManager.gm.player.GetComponent<PlayerMover>().distanceSprite,
                    new Vector3(pos.x + d.x, pos.y + d.y, -0.25f), Quaternion.identity);
                g.GetComponent<SpriteRenderer>().color = new Color(0f, 0.8f, 0.8f, 0.8f);
                g.GetComponent<DistanceSprite>().Disappear(60);

                Entity e = GameManager.gm.map.GetEntityOnTile(pos + d);
                if (e != null && e.GetType().Equals(typeof(Character)) && ((Character)e).type == Character.Type.Enemy)
                {
                    // 진행 방향에 플레이어가 있을 경우
                    Character enemy = (Character)e;
                    enemy.gustDamage += splashDamage;
                    enemy.gustDirection = d;
                }
            }
        }

        public static void Reflect(int percentDamage, Character target)
        {
            if (target.trueOldHealth > target.currentHealth)
            {
                float damage = (target.trueOldHealth - target.currentHealth) * (percentDamage / 100f);
                Vector3 pos = VectorUtility.PositionToInt(target.GetComponent<Transform>().position);
                Vector3[] direction = new Vector3[4] {
                    new Vector3(1f, 0f, 0f), new Vector3(-1f, 0f, 0f),
                    new Vector3(0f, 1f, 0f), new Vector3(0f, -1f, 0f) };
                foreach (Vector3 d in direction)
                {
                    GameObject g = Instantiate(GameManager.gm.player.GetComponent<PlayerMover>().distanceSprite,
                        new Vector3(pos.x + d.x, pos.y + d.y, -0.25f), Quaternion.identity);
                    g.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.8f);
                    g.GetComponent<DistanceSprite>().Disappear(60);

                    Entity e = GameManager.gm.map.GetEntityOnTile(pos + d);
                    if (e != null && e.GetType().Equals(typeof(Character)) && ((Character)e).type != target.type)
                    {
                        // 진행 방향에 적이 있을 경우
                        Character opponent = (Character)e;
                        opponent.hasReflected = true;
                        opponent.reflectDamage += damage;
                        opponent.reflectDirection = d;
                    }
                }
            }
        }

        public static void Regenerate(int heal, Character target)
        {
            target.regeneratedHealth += heal;
        }

        public static void Disguise(int element, Character target)
        {
            // 골렘 몬스터의 방어구 효과입니다.
            EnemyInfo ei;
            switch (element)
            {
                case 0:
                    ei = EnemyManager.em.FindEnemyInfo("Fire golem", GameManager.gm.mapLevel);
                    break;
                case 1:
                    ei = EnemyManager.em.FindEnemyInfo("Ice golem", GameManager.gm.mapLevel);
                    break;
                case 2:
                    ei = EnemyManager.em.FindEnemyInfo("Nature golem", GameManager.gm.mapLevel);
                    break;
                default:
                    return;
            }
            target.ReInitialize(ei.name);
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

    public bool Use(int index = -1)
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
                if (!GameManager.gm.hasIgnoreWeaponOrbMessage && index != -1)
                {
                    GameManager.gm.MessageTurn("Upgrade your weapon?",
                        "When you use 'weapon only' orb, your CURRENT EQUIPPED WEAPON is enhanced. " +
                        "Click 'Yes' to proceed. If you want to enchant another weapon, click 'No' and press 'C' key.",
                        delegate {
                            GameManager.gm.hasIgnoreWeaponOrbMessage = true;
                            GameManager.gm.SameTurnFromMessage();
                            GameManager.gm.player.GetComponent<Inventory>().UseItem(index);
                        }, 
                        delegate {
                            GameManager.gm.SameTurnFromMessage();
                        }, null);
                    return false;
                }
                else
                {
                    UseWeaponOnlyOrb();
                    return true;
                }
            }
            else if (usage == Usage.Armor)
            {
                GameManager.gm.player.armor.element += stat;
                GameManager.gm.player.statusUI.UpdateDefenseText(GameManager.gm.player.armor);
                if (effectName != null && !effectName.Equals("None"))
                {
                    if (GameManager.gm.player.armor.effects.ContainsKey(effectName))
                    {
                        GameManager.gm.player.armor.effects[effectName] += effectParam;
                    }
                    else
                    {
                        GameManager.gm.player.armor.effects.Add(effectName, effectParam);
                    }

                    if (effectName.Equals("Healthier"))
                    {
                        GameManager.gm.player.bonusMaxHealth += effectParam;
                        GameManager.gm.player.healthBar.maxValue += effectParam;
                        GameManager.gm.player.statusUI.UpdateHealthText(GameManager.gm.player.currentHealth,
                            GameManager.gm.player.MaxHealth);
                        GameManager.gm.player.Healed(effectParam);
                    }
                    else
                    {
                        GameManager.gm.player.armor.armorEffect += ItemManager.im.GetOrbEffect(effectName, effectParam);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private void UseWeaponOnlyOrb()
    {
        GameManager.gm.player.EquippedWeapon.element += stat;
        if (effectName != null && !effectName.Equals("None"))
        {
            bool isNotAfter = false;

            if (GameManager.gm.player.EquippedWeapon.effects.ContainsKey(effectName))
            {
                GameManager.gm.player.EquippedWeapon.effects[effectName] += effectParam;
            }
            else
            {
                GameManager.gm.player.EquippedWeapon.effects.Add(effectName, effectParam);
            }

            if (effectName.Equals("Stun"))
            {
                GameManager.gm.player.EquippedWeapon.stunEffectInTooltip = 1f -
                    ((1 - GameManager.gm.player.EquippedWeapon.stunEffectInTooltip) * (1 - (effectParam / 100f)));
            }

            if (effectName.Equals("FireAmp"))
            {
                GameManager.gm.player.EquippedWeapon.FireAmpBonus += effectParam / 100f;
                isNotAfter = true;
            }
            if (effectName.Equals("IceAmp"))
            {
                GameManager.gm.player.EquippedWeapon.IceAmpBonus += effectParam / 100f;
                isNotAfter = true;
            }
            if (effectName.Equals("NatureAmp"))
            {
                GameManager.gm.player.EquippedWeapon.NatureAmpBonus += effectParam / 100f;
                isNotAfter = true;
            }
            if (effectName.Equals("Sharpen"))
            {
                GameManager.gm.player.EquippedWeapon.chargeBonus += effectParam / 100f;
                isNotAfter = true;
            }
            if (effectName.Equals("Drain"))
            {
                GameManager.gm.player.EquippedWeapon.drainedPercent += effectParam / 100f;
                isNotAfter = true;
            }
            if (!isNotAfter)
            {
                GameManager.gm.player.EquippedWeapon.afterAttackEffect += ItemManager.im.GetOrbEffect(effectName, effectParam);
            }
        }
        GameManager.gm.player.statusUI.UpdateAttackText(GameManager.gm.player.EquippedWeapon);
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