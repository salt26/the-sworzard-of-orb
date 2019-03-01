using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : Entity {

    public enum Type { Player, Enemy };

    [Header("Stat")]
    public Type type;
    public new string name;
    public int level = 1;
    public int size = 1;            // 크기(충돌 판정이 이루어지는 타일 수)
    [SerializeField]
    private int maxHealth;
    public int currentHealth;       // TODO private로 바꾸기
    public List<Weapon> weapons;    // 무기의 속성과 공격력
    public Armor armor;             // 방어구의 속성과 방어력

    [Header("Reference")]
    public Slider healthBar;
    public StatusUI statusUI;
    public GameObject canvas;

    [HideInInspector]
    public int oldHealth;       // 플레이어가 자신의 턴을 넘길 때 남아있던 체력
    [HideInInspector]
    public int poisonDamage;
    [HideInInspector]
    public int gustDamage;
    [HideInInspector]
    public int bonusMaxHealth;
    [HideInInspector]
    public int trueOldHealth;   // 전 턴에 남아있던 체력 (반사 효과 구현에 필요)
    [HideInInspector]
    public int regeneratedHealth;
    [HideInInspector]
    public float reflectDamage;
    [HideInInspector]
    public Vector3 gustDirection;
    [HideInInspector]
    public Vector3 reflectDirection;
    [HideInInspector]
    public bool hasStuned;
    [HideInInspector]
    public bool hasReflected;

    private Mover mover;
    private bool alive = true;  // 살아 있는 동안 true
    private bool hasDamaged = false;
    //private bool hasHealed = false;
    private int weaponNum = -1;
    private Vector3 damagedDirection = new Vector3();
    private bool isCriticalDamage = false;

    public Mover Mover
    {
        get
        {
            return mover;
        }
    }

    public bool Alive
    {
        get
        {
            return alive;
        }
    }

    public int MaxHealth
    {
        get
        {
            return maxHealth + bonusMaxHealth;
        }
    }

    public Weapon EquippedWeapon
    {
        get
        {
            if (weaponNum >= 0 && weaponNum < weapons.Count)
                return weapons[weaponNum];
            else
                return null;
        }
    }
    
	void Awake () {
        mover = GetComponent<Mover>();
	}

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (type == Type.Enemy)
        {
            //Debug.Log("Enemy initialized. (call second)");
            EnemyInfo ei = EnemyManager.em.FindEnemyInfo(name, level);
            size = ei.size;
            maxHealth = ei.maxHealth;
            weapons = new List<Weapon> { ei.weapon.Clone() };
            armor = ei.armor.Clone();
            weapons[0].element += ei.weaponDelta * (level - 1);
            armor.element += ei.armorDelta * (level - 1);
            //Debug.Log(name + "(Lv." + level + "): W" + weapons[0].element + ", A" + armor.element);
            ((EnemyMover)mover).distanceType = ei.distanceType;
            ((EnemyMover)mover).sightDistance = ei.sightDistance;
            ((EnemyMover)mover).leaveDistance = ei.leaveDistance;
        }
        currentHealth = maxHealth;
        bonusMaxHealth = 0;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
        oldHealth = currentHealth;
        trueOldHealth = currentHealth;
        poisonDamage = 0;
        gustDamage = 0;
        reflectDamage = 0;
        hasStuned = false;
        hasReflected = false;
        ToggleWeapon();
        
        if (statusUI != null)
        {
            statusUI.UpdateAll(this, currentHealth);
        }
    }

    /// <summary>
    /// 대미지를 받을 때 호출됩니다.
    /// 해당 턴의 모든 대미지 계산이 완료된 후, 반드시 DamagedAnimation()이 호출되어야 합니다.
    /// </summary>
    /// <param name="damage"></param>
    public void Damaged(int damage, Vector3 direction, bool isCritical)
    {
        if (!Alive) return;
        currentHealth -= damage;
        damagedDirection = direction;
        isCriticalDamage = isCritical;
        hasDamaged = true;
    }

    /// <summary>
    /// 공격자의 무기에 적용되어 있던 effects를 이 대상에게 적용합니다.
    /// </summary>
    /// <param name="effects"></param>
    public void DamagedWithEffects(ItemManager.Effect effects)
    {
        if (effects != null && effects.GetInvocationList().Length > 0)
            effects(this);
    }

    /// <summary>
    /// 턴이 끝난 캐릭터의 방어구에 적용되어 있던 effects를 자기 자신에게 적용합니다.
    /// </summary>
    /// <param name="effects"></param>
    public void DefendWithEffects(ItemManager.Effect effects)
    {
        if (effects != null && effects.GetInvocationList().Length > 0)
        {
            effects(this);
        }
    }

    /// <summary>
    /// 대미지 계산이 완료된 후, 피격 애니메이션을 재생하기 위해 호출됩니다.
    /// </summary>
    public void DamagedAnimation()
    {
        if (hasDamaged)
        {
            hasDamaged = false;
            StartCoroutine(Mover.DamagedAnimation(oldHealth, healthBar, statusUI, damagedDirection, isCriticalDamage));
            damagedDirection = new Vector3();
        }
    }

    /// <summary>
    /// 중독 효과의 대미지를 받을 때 호출됩니다.
    /// 대미지 계산이 완료된 후 중독 애니메이션까지 재생됩니다.
    /// </summary>
    public void Poisoned()
    {
        if (!Alive) return;
        oldHealth = currentHealth;
        currentHealth -= poisonDamage;
        poisonDamage = 0;
        StartCoroutine(Mover.PoisonedAnimation(oldHealth, healthBar, statusUI));
    }

    /// <summary>
    /// 돌풍 효과의 대미지를 받을 때 호출됩니다.
    /// 대미지 계산이 완료된 후 돌풍 피격 애니메이션까지 재생됩니다.
    /// </summary>
    public void Gusted()
    {
        if (!Alive || gustDamage == 0) return;
        oldHealth = currentHealth;
        currentHealth -= gustDamage;
        gustDamage = 0;
        StartCoroutine(Mover.GustedAnimation(oldHealth, healthBar, statusUI, gustDirection));
    }

    /// <summary>
    /// 반사 효과의 대미지를 받을 때 호출됩니다.
    /// 대미지 계산이 완료된 후 반사 피격 애니메이션까지 재생됩니다.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="direction"></param>
    public void Reflected()
    {
        if (!Alive || !hasReflected) return;
        oldHealth = currentHealth;
        currentHealth -= (int)reflectDamage;
        reflectDamage = 0;
        hasReflected = false;
        StartCoroutine(Mover.ReflectedAnimation(oldHealth, healthBar, statusUI, reflectDirection));
    }

    public void Healed(int heal)
    {
        if (!Alive) return;
        oldHealth = currentHealth;
        currentHealth += heal;
        if (currentHealth > MaxHealth) currentHealth = MaxHealth;

        StartCoroutine(Mover.HealedAnimation(oldHealth, healthBar, statusUI));
    }

    public void Regenerated()
    {
        if (!Alive || regeneratedHealth == 0) return;
        oldHealth = currentHealth;
        currentHealth += regeneratedHealth;
        regeneratedHealth = 0;
        if (currentHealth > MaxHealth) currentHealth = MaxHealth;

        StartCoroutine(Mover.RegeneratedAnimation(oldHealth, healthBar, statusUI));
    }

    public void DeathCheck()
    {
        if (currentHealth <= 0 && Alive)
        {
            // 죽음
            // TODO 죽는 애니메이션과 스프라이트
            alive = false;
            foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
                sr.enabled = false;

            foreach (Image i in GetComponentsInChildren<Image>())
                i.enabled = false;
            mover.Death();
        }
    }

    /// <summary>
    /// 플레이어 캐릭터를 최대 체력으로 부활시킵니다.
    /// 이 메서드가 호출된 후에 gm.ChangeScene()이 호출됨을 가정합니다.
    /// </summary>
    public void Revive()
    {
        if (!alive && type == Type.Player)
        {
            currentHealth = 0;
            alive = true;
            Healed(MaxHealth);
            foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
                sr.enabled = true;

            foreach (Image i in GetComponentsInChildren<Image>())
                i.enabled = true;
            mover.enabled = true;
        }
    }

    public void ToggleWeapon()
    {
        if (type != Type.Player)
        {
            // TODO 적은 무기 변경 불가
            weaponNum = 0;
            GetComponent<SpriteRenderer>().sprite = 
                Resources.Load("Monsters/" + StringManager.ToPascalCase(name), typeof(Sprite)) as Sprite;
            return;
        }
        weaponNum++;
        if (weaponNum >= weapons.Count || weaponNum < 0) weaponNum = 0;

        GetComponent<SpriteRenderer>().sprite = GetComponent<PlayerMover>().characterSprite[weaponNum];
        GameManager.gm.weaponMark.sprite = GetComponent<PlayerMover>().weaponMarkSprite[weaponNum];
        
        if (statusUI != null)
            statusUI.UpdateAttackText(EquippedWeapon);

        /*
        if (range == 1)
        {
            // 창으로 변경
            GetComponent<SpriteRenderer>().sprite = Resources.Load("PowerCharacter2", typeof(Sprite)) as Sprite;
            range = 2;
            weapon.Range = range;
        }
        else
        {
            // 칼로 변경
            GetComponent<SpriteRenderer>().sprite = Resources.Load("PowerCharacter", typeof(Sprite)) as Sprite;
            range = 1;
            weapon.Range = range;
        }
        */
    }

    void OnMouseDown()
    {
        SelectThisCharacter();
    }

    public void SelectThisCharacter()
    {
        if (type == Type.Player || !Alive) return;
        GameManager.gm.SelectCharacter(this);
        statusUI.UpdateAll(this, currentHealth);
    }
}
