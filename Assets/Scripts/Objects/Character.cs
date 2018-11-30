using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : Entity {

    public enum Type { Player, Enemy };

    [Header("Stat")]
    public Type type;
    public int size = 1;        // 크기(충돌 판정이 이루어지는 타일 수)
    public int maxHealth;
    public int currentHealth;   // TODO private로 바꾸기
    public Element weapon;      // 무기의 속성과 공격력
    public Element armor;       // 방어구의 속성과 방어력
    public int range = 1;

    [Header("Reference")]
    public Slider healthBar;

    private Mover mover;
    private bool alive = true;  // 살아 있는 동안 true

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
    
	void Awake () {
        mover = GetComponent<Mover>();
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
        weapon.Range = range;
	}
	
	void Update () {
		
	}

    public void Damaged(int damage)
    {
        int oldHealth = currentHealth;
        currentHealth -= damage;
        StartCoroutine(Mover.DamagedAnimation(oldHealth, healthBar));
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
        if (type != Type.Player) return;
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
    }
}
