using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [Header("Weapon Mark")]
    public Image weaponMark;

    [Header("Restart Text")]
    public GameObject restartText;

    [Header("Debugging")]
    public int turnNumber = 0;
    
    private int turn;   // 0이면 플레이어의 이동 턴, 1이면 적들의 이동 턴, 2이면 턴이 넘어가는 중

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
            // 기존 gm을 지우고, 새 GameManger가 gm이 됨
            Destroy(gm.gameObject);
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

        // TODO 디버깅용 재시작 기능
        // 최종 버전에서는 없어야 함
        if (!player.Alive && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
	}

    /// <summary>
    /// 턴을 넘깁니다.
    /// 피격 애니메이션을 재생하고, 모든 애니메이션이 끝날 때까지 기다렸다가, 사망 판정 후 턴이 넘어갑니다.
    /// </summary>
    public void NextTurn()
    {
        StartCoroutine(NextTurnWithDelay());
    }

    IEnumerator NextTurnWithDelay()
    {
        bool ready;
        int oldTurn = turn;
        turn = 2;   // 임시 턴 (피격 애니메이션 재생 중 키 입력으로 한 턴에 여러 번 행동하는 것을 방지)

        if (oldTurn == 0)
        {
            // 턴을 넘길 때의 플레이어의 현재 체력을 기억
            player.oldHealth = player.currentHealth;
            foreach (Character e in enemies)
            {
                e.DamagedAnimation();
            }
        }
        else if (oldTurn == 1)
        {
            // 턴을 넘길 때의 각 적의 현재 체력을 기억
            foreach (Character e in enemies)
            {
                e.oldHealth = e.currentHealth;
            }
            player.DamagedAnimation();
        }

        while (true)
        {
            ready = true;
            foreach (Character e in enemies)
            {
                if (e.Alive && e.Mover.IsMoving)
                {
                    ready = false;
                    break;
                }
            }
            if (player.Alive && player.Mover.IsMoving) ready = false;

            if (ready) break;
            else yield return null;
        }

        // 사망 여부 확인
        player.DeathCheck();
        foreach (Character e in enemies)
        {
            e.DeathCheck();
        }

        turn = (oldTurn + 1) % 2;
        if (turn == 0)
        {
            turnMark.sprite = myTurn;
            turnMark.color = myTurnColor;
        }
        else
        {
            turnNumber++;   // TODO 디버깅 용
            turnMark.sprite = enemyTurn;
            turnMark.color = enemyTurnColor;
        }
    }
}
