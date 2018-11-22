using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity {

    public enum Type { Player, Enemy };

    [Header("Stat")]
    public Type type;
    public int maxHealth;
    public int currentHealth;   // TODO private로 바꾸기
    public Element weapon;      // 무기의 속성과 공격력
    public Element armor;       // 방어구의 속성과 방어력

    private Mover mover;

    public Mover Mover
    {
        get
        {
            return mover;
        }
    }
    
	void Awake () {
        mover = GetComponent<Mover>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
