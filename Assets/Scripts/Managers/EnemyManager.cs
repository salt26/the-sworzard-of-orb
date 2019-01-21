using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임에 등장하는 모든 적의 정보를 가지고 있습니다.
/// </summary>
public class EnemyManager : MonoBehaviour {

    public static EnemyManager em;

    /// <summary>
    /// key는 몬스터 ID, value는 몬스터 정보입니다.
    /// </summary>
    public Dictionary<int, EnemyInfo> enemyInfo;
    public GameObject monsterPrefab;

    void Awake()
    {
        em = this;
    }
    
    /// <summary>
    /// 적 name과 level을 인자로 주면, 해당하는 적 정보를 찾아 반환합니다.
    /// 적 name 정보는 있지만 level이 맞지 않는 경우, name이 맞는 적 정보를 하나 찾아 반환합니다.
    /// 정보가 없으면 null을 반환합니다.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public EnemyInfo FindEnemyInfo(string name, int level)
    {
        foreach (KeyValuePair<int, EnemyInfo> ei in enemyInfo)
        {
            if (ei.Value.name.Equals(name) && ei.Value.level == level)
            {
                return ei.Value;
            }
            else if (ei.Value.name.Equals(name))
            {
                return ei.Value;
            }
        }
        return null;
    }

    /// <summary>
    /// 적 id를 인자로 주면, 해당하는 적 정보를 찾아 반환합니다.
    /// 정보가 없으면 null을 반환합니다.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public EnemyInfo FindEnemyInfo(int id)
    {
        if (enemyInfo.ContainsKey(id)) return enemyInfo[id];
        return null;
    }
}

[System.Serializable]
public class EnemyInfo
{
    public enum Type { Normal, Elite, Boss };

    public string name;     // 이름(Primary key)
    public int level = 1;   // 레벨(Primary key)
    public Type type;
    public int size = 1;    // 크기(충돌 판정이 이루어지는 타일 수)
    public int maxHealth;
    public Weapon weapon;       // 무기의 속성과 공격력
    public Element weaponDelta; // 레벨 당 증가하는 무기 속성
    public Armor armor;         // 방어구의 속성과 방어력
    public Element armorDelta;  // 레벨 당 증가하는 방어구 속성

    public EnemyMover.DistanceType distanceType;    // 거리 측정 방식(반드시 설정해야만 함!)
    public int sightDistance;   // 적의 시야
    public int leaveDistance;   // 적의 최대 이동 거리

    public int gold;            // 적이 떨어뜨릴 화폐 양
    public List<EnemyDropItemInfo> dropItems;           // 적이 떨어뜨릴 아이템 목록("Gold" 제외)
}

[System.Serializable]
public class EnemyDropItemInfo
{
    public int itemID;
    public int count;
    public float probability;
}