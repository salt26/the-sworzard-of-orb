using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager gm;

    public MapManager map;
    public CharacterMover character;
    public List<EnemyMover> enemies = new List<EnemyMover>();
    
    private int turn;   // 0이면 플레이어의 이동 턴, 1이면 적들의 이동 턴

    public int Turn
    {
        get
        {
            return turn;
        }
    }
    
	void Awake () {
		if (gm != null)
        {
            Destroy(this);
        }
        gm = this;
        DontDestroyOnLoad(this);
	}

    void Start()
    {
        turn = 0;
    }

    void FixedUpdate () {
		if (turn == 1)
        {
            bool b = true;
            foreach (EnemyMover e in enemies)
            {
                if (!e.Moved)
                {
                    b = false;
                    break;
                }
            }
            if (b) NextTurn();
        }
	}

    public void NextTurn()
    {
        turn = (turn + 1) % 2;
    }
}
