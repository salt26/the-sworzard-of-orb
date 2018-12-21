﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager gm;

    public GameObject playerPrefab;
    public GameObject UIPrefab;
    public GameObject selectedBorderPrefab;

    // 여기에 등록되지 않은 캐릭터, 적 및 모든 개체는 게임 내에서 상호작용 불가
    // 등록은 MapEntityInfo에서!
    public Character player;
    [HideInInspector]
    public MapManager map;
    [HideInInspector]
    public List<Character> enemies = new List<Character>();
    [HideInInspector]
    public List<Interactable> interactables = new List<Interactable>();
    
    [HideInInspector]
    public Image turnMark;
    public Sprite myTurn;
    public Color myTurnColor;
    public Sprite enemyTurn;
    public Color enemyTurnColor;

    [HideInInspector]
    public Image weaponMark;
    [HideInInspector]
    public GameObject restartText;
    [HideInInspector]
    public GameObject loadingPanel;

    [SerializeField]
    private Character selectedCharacter = null;
    public GameObject mySelectedBorder;

    [Header("Debugging")]
    public int turnNumber = 0;

    private GameObject UIObject;
    private bool isSceneLoaded = false;
    
    private int turn;   // 0이면 플레이어의 이동 턴, 1이면 적들의 이동 턴, 2이면 턴이 넘어가는 중

    public int Turn
    {
        get
        {
            return turn;
        }
    }

    public bool IsSceneLoaded
    {
        get
        {
            return isSceneLoaded;
        }
    }

    public Character SelectedCharacter
    {
        get
        {
            return selectedCharacter;
        }
    }
    
	void Awake () {
        /*
		if (gm != null)
        {
            // 기존 gm을 지우고, 새 GameManger가 gm이 됨
            Destroy(gm.gameObject);
        }
        */
        if (gm != null)
        {
            Destroy(this.gameObject);
            return;
        }
        gm = this;
        DontDestroyOnLoad(this);
        GameObject p = Instantiate(playerPrefab);
        UIObject = Instantiate(UIPrefab);
        player = p.GetComponent<Character>();
        player.healthBar = UIObject.GetComponent<UIInfo>().healthBar;
        player.statusUI = UIObject.GetComponent<UIInfo>().playerStatusUI;
        player.GetComponent<Inventory>().goldText = UIObject.GetComponent<UIInfo>().goldText;
        player.GetComponent<Inventory>().itemButtons = UIObject.GetComponent<UIInfo>().itemButtons;
        turnMark = UIObject.GetComponent<UIInfo>().turnMark;
        weaponMark = UIObject.GetComponent<UIInfo>().weaponMark;
        restartText = UIObject.GetComponent<UIInfo>().restartText;
        loadingPanel = UIObject.GetComponent<UIInfo>().loadingPanel;
	}

    void Start()
    {
        Initialize();
        isSceneLoaded = true;
    }

    void FixedUpdate () {
        if (!isSceneLoaded) return;

        if (UIObject.GetComponent<UIInfo>().enemyStatusUIGroup.activeInHierarchy && selectedCharacter == null)
        {
            UIObject.GetComponent<UIInfo>().enemyStatusUIGroup.SetActive(false);
        }
        else if (!UIObject.GetComponent<UIInfo>().enemyStatusUIGroup.activeInHierarchy && selectedCharacter != null)
        {
            UIObject.GetComponent<UIInfo>().enemyStatusUIGroup.SetActive(true);
        }

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
            restartText.SetActive(false);
            // 0번 씬에 Manager 오브젝트가 있다고 가정
            SceneManager.LoadSceneAsync("Scenes/Town");
            Destroy(UIObject);
            Destroy(this.gameObject);
        }
	}

    /// <summary>
    /// 씬을 전환합니다.
    /// </summary>
    /// <param name="sceneName"></param>
    public void ChangeScene(string sceneName, string mapName = null)
    {
        player.Healed(player.maxHealth);
        StartCoroutine(LoadScene(sceneName, mapName));
    }

    IEnumerator LoadScene(string sceneName, string mapName = null)
    {
        // TODO 씬 전환 중에 불투명한 패널로 화면 가리기
        isSceneLoaded = false;
        loadingPanel.SetActive(true);
        Scene currentScene = SceneManager.GetActiveScene();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Scenes/" + sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.MoveGameObjectToScene(player.gameObject, SceneManager.GetSceneByName(sceneName));
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(currentScene);

        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        Initialize(mapName);
        yield return new WaitForSeconds(0.25f);
        loadingPanel.SetActive(false);
        isSceneLoaded = true;
    }

    private void Initialize(string mapName = null)
    {
        GameObject g = GameObject.Find("MapEntityInfo");
        if (g != null)
        {
            MapEntityInfo mei = g.GetComponent<MapEntityInfo>();
            if (mei != null)
            {
                map = mei.map;
                enemies = mei.enemies;
                interactables = mei.interactables;
            }
        }
        if (mapName != null) map.mapName = mapName;
        map.Initialize();
        map.SetEntityOnTile(player, player.GetComponent<Transform>().position);

        // TODO 시작 시에 존재하는 모든 적의 충돌 판정 크기가 타일 1개 크기라고 가정
        //      또한, 다른 개체와 같은 타일에 겹쳐 있는 상태로 시작하는 적이 없다고 가정
        foreach (Character e in enemies)
        {
            map.SetEntityOnTile(e, e.GetComponent<Transform>().position);
            e.statusUI = UIObject.GetComponent<UIInfo>().enemyStatusUI;
        }
        foreach (Interactable i in interactables)
        {
            map.SetEntityOnTile(i, i.GetComponent<Transform>().position);
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

    /// <summary>
    /// 인자로 넘긴 캐릭터를 선택하여, 경계를 표시하고 상태 UI에 정보를 보여줍니다.
    /// 인자로 null을 줄 수 있습니다.
    /// </summary>
    /// <param name="c"></param>
    public void SelectCharacter(Character c)
    {
        selectedCharacter = c;
        if (mySelectedBorder != null) Destroy(mySelectedBorder);

        if (selectedCharacter != null)
            mySelectedBorder = Instantiate(selectedBorderPrefab, c.GetComponent<Transform>());
    }
}
