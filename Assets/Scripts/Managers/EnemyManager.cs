using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임에 등장하는 모든 적의 정보를 가지고 있습니다.
/// </summary>
public class EnemyManager : MonoBehaviour {

    public static EnemyManager em;

    [SerializeField]
    private List<EnemyInfo> enemyInfo;

    void Awake()
    {
        em = this;
    }
    
    /// <summary>
    /// 적 name과 level을 인자로 주면, 해당하는 적 정보를 찾아 반환합니다.
    /// 정보가 없으면 null을 반환합니다.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public EnemyInfo FindEnemyInfo(string name, int level)
    {
        foreach (EnemyInfo ei in enemyInfo)
        {
            if (ei.name.Equals(name) && ei.level == level)
            {
                return ei;
            }
        }
        return null;
    }
}

[System.Serializable]
public class EnemyInfo
{
    public string name;
    public int level = 1;
    public int size = 1;    // 크기(충돌 판정이 이루어지는 타일 수)
    public int maxHealth;
    public Weapon weapon;   // 무기의 속성과 공격력
    public Armor armor;     // 방어구의 속성과 방어력

    public int sightDistance;
    public int leaveDistance;

    public int gold;
    public List<GameObject> items;
}