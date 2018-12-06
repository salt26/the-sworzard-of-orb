﻿using System.Collections;
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
    public int maxHealth;
    public int currentHealth;       // TODO private로 바꾸기
    public List<Weapon> weapons;    // 무기의 속성과 공격력
    public Armor armor;             // 방어구의 속성과 방어력

    [Header("Reference")]
    public Slider healthBar;

    [HideInInspector]
    public int oldHealth;       // 플레이어가 자신의 턴을 넘길 때 남아있던 체력

    private Mover mover;
    private bool alive = true;  // 살아 있는 동안 true
    private bool hasDamaged = false;
    private int weaponNum = -1;

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
            EnemyInfo ei = EnemyManager.em.FindEnemyInfo(name, level);
            size = ei.size;
            maxHealth = ei.maxHealth;
            weapons = new List<Weapon> { ei.weapon };
            armor = ei.armor;
            ((EnemyMover)mover).distanceType = ei.distanceType;
            ((EnemyMover)mover).sightDistance = ei.sightDistance;
            ((EnemyMover)mover).leaveDistance = ei.leaveDistance;
        }
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
        oldHealth = currentHealth;
        ToggleWeapon();
    }

    /// <summary>
    /// 대미지를 받을 때 호출됩니다.
    /// 해당 턴의 모든 대미지 계산이 완료된 후, 반드시 DamagedAnimation()이 호출되어야 합니다.
    /// </summary>
    /// <param name="damage"></param>
    public void Damaged(int damage)
    {
        currentHealth -= damage;
        hasDamaged = true;
    }

    /// <summary>
    /// 대미지 계산이 완료된 후, 피격 애니메이션을 재생하기 위해 호출됩니다.
    /// </summary>
    public void DamagedAnimation()
    {
        if (hasDamaged)
        {
            hasDamaged = false;
            StartCoroutine(Mover.DamagedAnimation(oldHealth, healthBar));
        }
    }

    public void DeathCheck()
    {
        if (currentHealth <= 0 && alive)
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

    public void ToggleWeapon()
    {
        if (type != Type.Player)
        {
            // TODO 적은 무기 변경 불가
            weaponNum = 0;
            GetComponent<SpriteRenderer>().sprite = EquippedWeapon.CharacterSprite;
            return;
        }
        weaponNum++;
        if (weaponNum >= weapons.Count || weaponNum < 0) weaponNum = 0;

        GetComponent<SpriteRenderer>().sprite = EquippedWeapon.CharacterSprite;
        GameManager.gm.weaponMark.sprite = EquippedWeapon.WeaponSprite;

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
}
