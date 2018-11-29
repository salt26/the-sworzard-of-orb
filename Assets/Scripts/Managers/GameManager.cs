using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager gm;

    // 여기에 등록되지 않은 캐릭터, 적 및 모든 개체는 게임 내에서 상호작용 불가
    public MapManager map;
    public Character player;
    public List<Character> enemies = new List<Character>();

    [Header("Turn Mark")]
    public Image turnMark;
    public Sprite myTurn;
    public Color myTurnColor;
    public Sprite enemyTurn;
    public Color enemyTurnColor;
    
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
        map.SetEntityOnTile(player, player.GetComponent<Transform>().position);

        // TODO 시작 시에 존재하는 모든 적의 충돌 판정 크기가 타일 1개 크기라고 가정
        //      또한, 다른 개체와 같은 타일에 겹쳐 있는 상태로 시작하는 적이 없다고 가정
        foreach (Character e in enemies)
        {
            map.SetEntityOnTile(e, e.GetComponent<Transform>().position);
        }
    }

    void FixedUpdate () {
		if (turn == 1)
        {
            bool b = true;
            foreach (Character e in enemies)
            {
                if (e.type != Character.Type.Enemy || !e.Alive) continue;
                EnemyMover em = (EnemyMover)e.Mover;
                if (!em.Moved)
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
        player.DeathCheck();
        foreach (Character e in enemies)
        {
            e.DeathCheck();
        }

        turn = (turn + 1) % 2;
        if (turn == 0)
        {
            turnMark.sprite = myTurn;
            turnMark.color = myTurnColor;
        }
        else
        {
            turnMark.sprite = enemyTurn;
            turnMark.color = enemyTurnColor;
        }
    }
}
