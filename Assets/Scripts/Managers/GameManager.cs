using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Map map;
    public List<Character> enemies = new List<Character>();
    public List<Interactable> interactables = new List<Interactable>();

    [HideInInspector]
    public GameObject monsterNumberMark;
    [HideInInspector]
    public Text monsterNumberText;

    [HideInInspector]
    public Image weaponMark;
    [HideInInspector]
    public GameObject restartText;
    [HideInInspector]
    public GameObject loadingPanel;

    [SerializeField]
    private Character selectedCharacter = null;
    public GameObject mySelectedBorder;

    [HideInInspector]
    public int mapLevel = 1;

    [Header("Debugging")]
    public int turnNumber = 0;

    [HideInInspector]
    public bool hasShopVisited = false;
    [HideInInspector]
    public bool hasIgnoreShopMessage = false;
    [HideInInspector]
    public bool hasIgnoreReturnMessage = false;

    private GameObject UIObject;
    private bool isSceneLoaded = false;
    
    [SerializeField]
    private int turn;   // 0이면 플레이어의 이동 턴, 1이면 적들의 이동 턴, 
                        // 2이면 턴이 넘어가는 중, 3이면 Altar에서 조합하는 중,
                        // 4이면 Shop에 있는 중, 5이면 Message가 떠 있는 중

    private int turnLimit;      // 최대 턴 제한

    public int Turn
    {
        get
        {
            return turn;
        }
    }

    /// <summary>
    /// 현재 맵에 살아있는 몬스터 수를 반환합니다.
    /// </summary>
    public int MonsterNumber
    {
        get
        {
            if (!IsSceneLoaded) return 0;
            int monsterNumber = 0;
            foreach (Character e in enemies)
            {
                if (e.type != Character.Type.Enemy || !e.Alive) continue;
                monsterNumber++;
            }
            return monsterNumber;
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

    public GameObject Canvas
    {
        get
        {
            return UIObject.gameObject;
        }
    }

    private int TurnNumber
    {
        set
        {
            turnNumber = value;
        }
    }

    public int RemainedTurn
    {
        get
        {
            // 남은 턴 제한 (-1이면 턴 제한이 없는 맵, 0이 되면 플레이어 사망)
            if (turnLimit == -1) return -1;
            else return turnLimit - turnNumber;
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
        monsterNumberMark = UIObject.GetComponent<UIInfo>().monsterNumberMark;
        monsterNumberText = UIObject.GetComponent<UIInfo>().monsterNumberText;
        weaponMark = UIObject.GetComponent<UIInfo>().weaponMark;
        restartText = UIObject.GetComponent<UIInfo>().restartText;
        loadingPanel = UIObject.GetComponent<UIInfo>().loadingPanel;
        hasIgnoreReturnMessage = false;
        hasIgnoreShopMessage = false;
        Debug.Log(player.MaxHealth);
	}

    void Start()
    {
        GameObject g = GameObject.Find("MapEntityInfo");
        if (g != null)
        {
            Shop shop = (Shop)g.GetComponent<MapEntityInfo>().interactables.Find(
                i => i != null && i.GetType().Equals(typeof(Shop)));
            if (shop != null)
            {
                g.GetComponent<MapEntityInfo>().interactables.Remove(shop);
                Destroy(shop.gameObject);
            }
        }
        mapLevel = 1;
        Initialize();
        isSceneLoaded = true;
    }

    void FixedUpdate () {
        if (!isSceneLoaded) return;
        if (isSceneLoaded) Debug.Log(gm.player);

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

        // 재시작 기능
        /*
        if (!player.Alive && Input.GetKeyDown(KeyCode.R))
        {
            restartText.SetActive(false);
            player.GetComponent<Inventory>().RemoveAllItems();   // 가지고 있던 모든 아이템을 잃음
            player.GetComponent<Inventory>().Gold /= 2;
            player.Revive();
            //mapLevel[map.mapName]--;
            mapLevel--;
            ChangeScene("Town");
        }
        */
	}

    /// <summary>
    /// 씬을 전환합니다.
    /// </summary>
    /// <param name="sceneName"></param>
    public void ChangeScene(string sceneName, string mapName = null)
    {
        player.Healed(player.MaxHealth);
        turnLimit = -1;
        if (mapName == null && sceneName.Equals("Town"))
        {
            mapLevel++;
        }
        if (sceneName.Equals("Town"))
        {
            // 마을로 이동할 때, 상점에 새 재고가 들어옴
            int orbID = Random.Range(100, 104);
            string randomOrb = ItemManager.im.FindItemInfo(orbID).name;
            Canvas.GetComponent<UIInfo>().shopPanel.GetComponent<ShopUI>().purchaseItems = new List<string>
                { "Small potion", "Large potion", "Return scroll",
                    randomOrb, "Random orb 1", "Random orb 2" };
        }
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
        MapEntityInfo mei = null;
        GameObject g = GameObject.Find("MapEntityInfo");
        if (g != null)
        {
            mei = g.GetComponent<MapEntityInfo>();
            if (mei != null)
            {
                map = mei.map;
                enemies = mei.enemies;
                interactables = mei.interactables;
            }
        }
        bool mapAutoGeneration = false;
        if (mapName != null)
        {
            map.mapName = mapName;
            mapAutoGeneration = true;
            MapInfo mi = MapManager.mm.FindMapInfo(mapName, mapLevel);
            map.GetComponent<AudioSource>().clip = Resources.Load("Audios/" + StringManager.ToPascalCase(mi.backgroundMusic), typeof(AudioClip)) as AudioClip;
            map.GetComponent<AudioSource>().Play();
            monsterNumberMark.SetActive(true);
            monsterNumberText.gameObject.SetActive(true);
            turnNumber = 0;
            turnLimit = mi.turnLimit;
            if (turnLimit >= 0)
            {
                Canvas.GetComponent<UIInfo>().turnLimitMark.SetActive(true);
                Canvas.GetComponent<UIInfo>().turnLimitText.gameObject.SetActive(true);
                Canvas.GetComponent<UIInfo>().turnLimitText.text = RemainedTurn.ToString();

                int ind = Canvas.GetComponent<UIInfo>().turnLimitSprites.Count - 1;
                if (turnLimit == 0) ind = 0;

                Canvas.GetComponent<UIInfo>().turnLimitMark.GetComponent<Image>().sprite =
                    Canvas.GetComponent<UIInfo>().turnLimitSprites[ind];
                Canvas.GetComponent<UIInfo>().turnLimitMark.GetComponent<Image>().color =
                    Canvas.GetComponent<UIInfo>().turnLimitColors[ind];
                Canvas.GetComponent<UIInfo>().turnLimitText.color =
                    Canvas.GetComponent<UIInfo>().turnLimitColors[ind];
            }
            else
            {
                Canvas.GetComponent<UIInfo>().turnLimitMark.SetActive(false);
                Canvas.GetComponent<UIInfo>().turnLimitText.gameObject.SetActive(false);
            }
        }
        else
        {
            monsterNumberMark.SetActive(false);
            monsterNumberText.gameObject.SetActive(false);
            turnNumber = 0;
            turnLimit = -1;
            Canvas.GetComponent<UIInfo>().turnLimitMark.SetActive(false);
            Canvas.GetComponent<UIInfo>().turnLimitText.gameObject.SetActive(false);
        }
        hasShopVisited = false;
        StringManager.sm.RefreshTexts();
        map.Initialize(mapAutoGeneration);

        AvailableTile availableTile = new AvailableTile(map.MapSize, map.BottomLeft);    // 맵의 각 타일 위에 새 개체를 놓을 수 있는지 확인

        for (int i = (int)map.BottomLeft.x; i <= (int)map.TopRight.x; i++)
        {
            for (int j = (int)map.BottomLeft.y; j <= (int)map.TopRight.y; j++)
            {
                // 밟을 수 없는 타일 확인
                if (map.GetTypeOfTile(i, j) != 0)
                    availableTile.Set(i, j);
            }
        }

        // 플레이어 시작 위치 지정
        int randomQuadrant = Random.Range(1, 5);
        Vector2Int v = map.GetACornerPosition(randomQuadrant);
        if (!mapAutoGeneration)
        {
            player.GetComponent<Transform>().position = new Vector3(2f, 2f, -1f);
        }
        else
        {
            player.GetComponent<Transform>().position =
                new Vector3(v.x, v.y, player.GetComponent<Transform>().position.z);
        }
        map.SetEntityOnTile(player, player.GetComponent<Transform>().position);
        availableTile.Set(v.x, v.y);

        #region MapEntityInfo에 따라 씬에 미리 배치되어 있는 개체 등록
        foreach (Interactable i in mei.interactables)
        {
            map.SetEntityOnTile(i, i.GetComponent<Transform>().position);
            availableTile.Set(i.GetComponent<Transform>().position);
        }
        // TODO 시작 시에 존재하는 모든 적의 충돌 판정 크기가 타일 1개 크기라고 가정
        //      또한, 다른 개체와 같은 타일에 겹쳐 있는 상태로 시작하는 적이 없다고 가정
        //      그리고 맵의 범위를 벗어난 위치에 있거나 벗어난 위치를 순찰 체크포인트로 갖는 적이 없다고 가정
        foreach (Character c in mei.enemies)
        {
            map.SetEntityOnTile(c, c.GetComponent<Transform>().position);
            c.statusUI = UIObject.GetComponent<UIInfo>().enemyStatusUI;
            availableTile.Set(c.GetComponent<Transform>().position);
            foreach (Vector3 cp in ((EnemyMover)c.Mover).checkpoints)
            {
                availableTile.Set(cp);
            }
        }
        #endregion

        #region 맵 정보(MapInfo)에 따라 개체 생성 후 등록
        if (mapName != null)
        {
            MapInfo mi = MapManager.mm.FindMapInfo(mapName, mapLevel);
            if (mi != null)
            {
                foreach (int id in mi.interactablesID)
                {
                    Interactable i = GetComponent<InteractableManager>().GetInteractable(id);
                    if (i != null)
                    {
                        int quadrant = 0, maxLoop = 100;
                        bool canCreate = true;
                        for (int j = 0; j < maxLoop; j++)
                        {
                            if (id == 0)
                            {
                                for (int k = 0; k < maxLoop; k++)
                                {
                                    quadrant = Random.Range(1, 5);
                                    if (quadrant != randomQuadrant) break;
                                    if (k == maxLoop - 1)
                                    {
                                        Debug.LogWarning("Exceed max loop limit!");
                                    }
                                }
                                v = map.GetACornerPosition(quadrant);
                                if (map.GetEntityOnTile(v.x, v.y) == null && map.GetTypeOfTile(v.x, v.y) == 0)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                v = new Vector2Int(Random.Range(Mathf.RoundToInt(map.BottomLeft.x), Mathf.RoundToInt(map.TopRight.x)),
                                    Random.Range(Mathf.RoundToInt(map.BottomLeft.y), Mathf.RoundToInt(map.TopRight.y)));
                                if (map.GetEntityOnTile(v.x, v.y) == null && map.GetTypeOfTile(v.x, v.y) == 0)
                                {
                                    break;
                                }
                            }
                            if (j == maxLoop - 1)
                            {
                                Debug.LogWarning("Exceed max loop limit!");
                                canCreate = false;
                            }
                        }
                        if (!canCreate) continue;

                        // TODO 생성 위치 바꾸기
                        g = Instantiate(i.gameObject, new Vector3(v.x, v.y, -1f), Quaternion.identity);
                        interactables.Add(g.GetComponent<Interactable>());
                        map.SetEntityOnTile(g.GetComponent<Interactable>(), g.GetComponent<Transform>().position);
                        availableTile.Set(v.x, v.y);
                    }
                }
                // TODO 맵 정보에 의해 생성되는 모든 적의 충돌 판정 크기가 타일 1개 크기라고 가정
                //      또한, 다른 개체와 같은 타일에 겹쳐 있는 상태로 시작하는 적이 없다고 가정
                foreach (int id in mi.enemiesID)
                {
                    EnemyInfo ei = EnemyManager.em.FindEnemyInfo(id);
                    if (ei != null)
                    {
                        // 생성 위치 정하기
                        int x = 0, y = 0, x2 = 0, y2 = 0, maxLoop = 100;
                        bool canCreate = true;
                        for (int j = 0; j < maxLoop; j++)
                        {
                            // 우선 가능한 위치 중에서 랜덤으로 생성하고, 후에 스폰 가능한 위치인지 확인
                            x = Random.Range(Mathf.RoundToInt(map.BottomLeft.x), Mathf.RoundToInt(map.TopRight.x));
                            y = Random.Range(Mathf.RoundToInt(map.BottomLeft.y), Mathf.RoundToInt(map.TopRight.y));
                            if (map.GetEntityOnTile(x, y) == null && map.GetTypeOfTile(x, y) == 0 && availableTile.Get(x, y) &&
                                VectorUtility.ChebyshevDistance(new Vector3(x, y, 0f), player.GetComponent<Transform>().position) > 3)
                            {
                                bool b = true;
                                foreach (Interactable i in interactables)
                                {
                                    if (VectorUtility.ChebyshevDistance(new Vector3(x, y, 0f), i.GetComponent<Transform>().position) <= 2)
                                    {
                                        b = false;
                                        break;
                                    }
                                }
                                if (b)
                                {
                                    // 순찰 경로 지정
                                    if (ei.type == EnemyInfo.Type.Normal)
                                    {
                                        int distance = 4;
                                        bool b2 = true;
                                        for (int k = 0; k < maxLoop; k++)
                                        {
                                            // 자신 근처의 위치 중에서, 순찰 경로 안에 스폰 불가능 위치가 하나도 존재하지 않은 위치로 결정
                                            // 순찰 경로의 체크포인트가 하나만 존재함을 가정
                                            x2 = Random.Range(Mathf.Clamp(x - distance, 0, map.MapSize.x - 1), Mathf.Clamp(x + distance, 0, map.MapSize.x - 1));
                                            y2 = Random.Range(Mathf.Clamp(y - distance, 0, map.MapSize.y - 1), Mathf.Clamp(y + distance, 0, map.MapSize.y - 1));
                                            if (map.GetEntityOnTile(x2, y2) == null && map.GetTypeOfTile(x2, y2) == 0 && !(x2 == x && y2 == y))
                                            {
                                                bool b3 = true;
                                                for (int l = Mathf.Min(x, x2); l <= Mathf.Max(x, x2); l++)
                                                {
                                                    if (!availableTile.Get(l, Mathf.Min(y, y2)) || !availableTile.Get(l, Mathf.Max(y, y2)))
                                                    {
                                                        b3 = false;
                                                        break;
                                                    }
                                                }
                                                for (int l = Mathf.Min(y, y2); l <= Mathf.Max(y, y2); l++)
                                                {
                                                    if (!availableTile.Get(Mathf.Min(x, x2), l) || !availableTile.Get(Mathf.Max(x, x2), l))
                                                    {
                                                        b3 = false;
                                                        break;
                                                    }
                                                }
                                                if (b3) break;
                                            }
                                            if (k == maxLoop - 1)
                                            {
                                                //Debug.LogWarning("Exceed max loop limit!");
                                                b2 = false;
                                            }
                                        }
                                        if (b2) break;
                                    }
                                    else break;
                                }
                            }
                            //Debug.Log(j);
                            if (j == maxLoop - 1)
                            {
                                Debug.LogWarning("Exceed max loop limit!");
                                canCreate = false;
                            }
                        }
                        if (!canCreate) continue;

                        g = Instantiate(EnemyManager.em.monsterPrefab, new Vector3(x, y, -1f), Quaternion.identity);
                        Character c = g.GetComponent<Character>();
                        c.name = ei.name;
                        c.level = mapLevel;
                        //Debug.Log("GM creates an enemy. (call first)");
                        enemies.Add(c);
                        map.SetEntityOnTile(c, g.GetComponent<Transform>().position);
                        c.statusUI = UIObject.GetComponent<UIInfo>().enemyStatusUI;
                        availableTile.Set(x, y);
                        
                        c.GetComponent<EnemyMover>().checkpoints = new List<Vector3>();

                        if (ei.type == EnemyInfo.Type.Normal)
                        {
                            c.GetComponent<EnemyMover>().checkpoints.Add(new Vector3(x2, y2, -1f));
                            availableTile.Set(x2, y2);
                            for (int l = Mathf.Min(x, x2); l <= Mathf.Max(x, x2); l++)
                            {
                                availableTile.Set(l, y);
                                availableTile.Set(l, y2);
                            }
                            for (int l = Mathf.Min(y, y2); l <= Mathf.Max(y, y2); l++)
                            {
                                availableTile.Set(x, l);
                                availableTile.Set(x2, l);
                            }
                        }
                    }
                }
            }
        }
        #endregion
        
        monsterNumberText.text = MonsterNumber.ToString();
    }

    /// <summary>
    /// 턴을 넘깁니다.
    /// 피격 애니메이션을 재생하고, 모든 애니메이션이 끝날 때까지 기다렸다가, 사망 판정 후 턴이 넘어갑니다.
    /// </summary>
    public void NextTurn()
    {
        if (turn > 1) return;
        int oldTurn = turn;
        turn = 2;   // 임시 턴 (피격 애니메이션 재생 중 키 입력으로 한 턴에 여러 번 행동하는 것을 방지)
        StartCoroutine(NextTurnWithDelay(oldTurn));
    }

    IEnumerator NextTurnWithDelay(int oldTurn)
    {
        bool ready;

        /* 페이즈 1: 일반 피격 애니메이션 재생 */
        if (oldTurn == 0)
        {
            // 플레이어의 턴이 끝남
            foreach (Character e in enemies)
            {
                e.DamagedAnimation();
            }
        }
        else if (oldTurn == 1)
        {
            // 적들의 턴이 끝남
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

        /* 페이즈 2: 무기 특수 효과(흡수) 대미지 애니메이션 처리 */
        if (oldTurn == 0)
        {
            // 흡수 효과의 회복 처리
            if (player.EquippedWeapon.drainedPercent > 0f && player.attackSum > 0)
            {
                player.Drained();
            }
            player.attackSum = 0;

        }
        else if (oldTurn == 1)
        {
            // 적들의 턴이 끝남
            foreach (Character e in enemies)
            {
                // 흡수 효과의 회복 처리
                if (e.EquippedWeapon.drainedPercent > 0f && e.attackSum > 0)
                {
                    e.Drained();
                }
                e.attackSum = 0;
            }
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

        /* 페이즈 3: 무기 특수 효과(중독, 돌풍) 대미지 애니메이션 처리 */
        if (oldTurn == 0)
        {
            // 플레이어의 턴이 끝남
            foreach (Character e in enemies)
            {
                // 돌풍 효과의 대미지 처리
                if (e.gustDamage > 0)
                {
                    e.Gusted();
                }
            }

            // 중독 효과의 대미지 처리
            if (player.poisonDamage > 0)
            {
                player.Poisoned();
            }

        }
        else if (oldTurn == 1)
        {
            // 적들의 턴이 끝남
            foreach (Character e in enemies)
            {
                // 중독 효과의 대미지 처리
                if (e.poisonDamage > 0)
                {
                    e.Poisoned();
                }
            }
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

        /* 페이즈 4: 방어구 특수 효과(반사, 재생) 대미지 애니메이션 처리 */
        if (oldTurn == 0)
        {
            foreach (Character e in enemies)
            {
                e.DefendWithEffects(e.armor.armorEffect);

                // 턴을 넘길 때의 각 적의 현재 체력을 기억
                e.oldHealth = e.currentHealth;
            }
            player.Reflected();
            foreach (Character e in enemies)
            {
                e.Regenerated();
            }
            // 턴을 넘길 때의 플레이어의 현재 체력을 기억
            player.oldHealth = player.currentHealth;
            player.trueOldHealth = player.currentHealth;
        }
        else if (oldTurn == 1)
        {
            player.DefendWithEffects(player.armor.armorEffect);
            foreach (Character e in enemies)
            {
                e.Reflected();
                // 턴을 넘길 때의 각 적의 현재 체력을 기억
                e.oldHealth = e.currentHealth;
                e.trueOldHealth = e.currentHealth;
            }
            player.Regenerated();
            player.oldHealth = player.currentHealth;
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

        /* 페이즈 5: 제한 턴 초과에 의한 대미지 애니메이션 처리 */
        if (oldTurn == 0)
        {
            if (RemainedTurn == 0)
            {
                player.Damaged(player.MaxHealth, new Vector3(0f, 0f, 0f), true);
                player.DamagedAnimation();

                // 턴을 넘길 때의 플레이어의 현재 체력을 기억
                player.oldHealth = player.currentHealth;
                player.trueOldHealth = player.currentHealth;
            }
        }
        else if (oldTurn == 1)
        {

        }

        while (true)
        {
            ready = true;
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
        
        monsterNumberText.text = MonsterNumber.ToString();

        turn = (oldTurn + 1) % 2;

        if (turn == 1)
        {
            turnNumber++;
            UIInfo ui = Canvas.GetComponent<UIInfo>();
            if (turnLimit > 0 && RemainedTurn >= 0)
            {
                ui.turnLimitText.text = RemainedTurn.ToString();
                if (RemainedTurn > 0)
                {
                    ui.turnLimitMark.GetComponent<Image>().sprite =
                        ui.turnLimitSprites[(RemainedTurn - 1) * (ui.turnLimitSprites.Count - 1) / turnLimit + 1];
                    ui.turnLimitMark.GetComponent<Image>().color =
                        ui.turnLimitColors[(RemainedTurn - 1) * (ui.turnLimitSprites.Count - 1) / turnLimit + 1];
                    ui.turnLimitText.color =
                        ui.turnLimitColors[(RemainedTurn - 1) * (ui.turnLimitSprites.Count - 1) / turnLimit + 1];
                }
                else
                {
                    ui.turnLimitMark.GetComponent<Image>().sprite = ui.turnLimitSprites[0];
                    ui.turnLimitMark.GetComponent<Image>().color = ui.turnLimitColors[0];
                    ui.turnLimitText.color = ui.turnLimitColors[0];
                }
            }
        }
    }

    /// <summary>
    /// Altar와 상호작용하여 조합하는 상태로 턴이 넘어갑니다.
    /// </summary>
    public void AltarTurn()
    {
        if (turn != 0) return;
        //Debug.Log("AltarTurn");
        turn = 3;
    }

    /// <summary>
    /// Altar에서의 조합을 마치고 턴을 넘깁니다.
    /// </summary>
    public void NextTurnFromAltar()
    {
        if (turn != 3) return;
        //Debug.Log("NextTurnFromAltar");
        turn = 1;
        turnNumber++;
    }

    /// <summary>
    /// Shop과 상호작용하여 아이템을 사고 파는 상태로 턴이 넘어갑니다.
    /// </summary>
    public void ShopTurn()
    {
        if (turn != 0) return;
        //Debug.Log("ShopTurn");
        hasShopVisited = true;
        turn = 4;
    }

    /// <summary>
    /// Shop에서의 매매를 마치고 턴을 넘깁니다.
    /// </summary>
    public void NextTurnFromShop()
    {
        if (turn != 4) return;
        //Debug.Log("NextTurnFromShop");
        turn = 1;
        turnNumber++;
    }

    /// <summary>
    /// 메시지를 띄우고, 메시지가 띄워진 상태로 턴이 넘어갑니다.
    /// OK 메시지를 띄웁니다.
    /// header와 body에는 영어 원문을 넣어야 합니다.
    /// </summary>
    public void MessageTurn(string header, string body, UnityEngine.Events.UnityAction onOKClick = null,
        UnityEngine.Events.UnityAction<bool> onShowToggle = null)
    {
        if (turn != 0) return;
        turn = 5;

        Canvas.GetComponent<UIInfo>().messagePanel.SetActive(true);
        Canvas.GetComponent<UIInfo>().messagePanel.GetComponent<MessageUI>().Initialize(
            header, body, onOKClick, onShowToggle);
    }

    /// <summary>
    /// 메시지를 띄우고, 메시지가 띄워진 상태로 턴이 넘어갑니다.
    /// 확인 또는 취소 메시지를 띄웁니다.
    /// header와 body에는 영어 원문을 넣어야 합니다.
    /// </summary>
    public void MessageTurn(string header, string body,
        UnityEngine.Events.UnityAction onYesClick, UnityEngine.Events.UnityAction onNoClick, 
        UnityEngine.Events.UnityAction<bool> onShowToggle = null)
    {
        if (turn != 0) return;
        turn = 5;

        Canvas.GetComponent<UIInfo>().messagePanel.SetActive(true);
        Canvas.GetComponent<UIInfo>().messagePanel.GetComponent<MessageUI>().Initialize(
            header, body, onYesClick, onNoClick, onShowToggle);
    }

    public void NextTurnFromMessage()
    {
        if (turn != 5) return;
        turn = 1;
        turnNumber++;
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

    public void RefreshRestartText()
    {
        restartText.GetComponent<Text>().text = StringManager.sm.Translate("Press 'R' to revive!");
    }
}

/// <summary>
/// 맵 자동 생성 시, 새로운 적 개체를 스폰할 때 사용되는 클래스입니다.
/// 맵에서 개체가 스폰될 수 없는 위치를 가지고 있습니다.
/// </summary>
public class AvailableTile
{
    private List<List<bool>> t = new List<List<bool>>();
    private Vector2Int bottomLeft;
    private Vector2Int mapSize;

    /// <summary>
    /// 맵 크기에 맞게 모든 타일을 스폰 가능 위치로 초기화합니다.
    /// </summary>
    /// <param name="bottomLeft"></param>
    /// <param name="mapSize"></param>
    public AvailableTile(Vector2Int mapSize, Vector2 bottomLeft = new Vector2())
    {
        this.bottomLeft = new Vector2Int((int)bottomLeft.x, (int)bottomLeft.y);
        this.mapSize = mapSize;
        for (int i = 0; i < mapSize.y; i++)
        {
            t.Add(new List<bool>());
            for (int j = 0; j < mapSize.x; j++)
            {
                t[i].Add(true);
            }
        }
    }

    /// <summary>
    /// (x, y) 좌표의 타일을 isSpawnable로 설정합니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="isSpawnable"></param>
    public void Set(int x, int y, bool isSpawnable = false)
    {
        if (x < bottomLeft.x || y < bottomLeft.y ||
            x - bottomLeft.x >= mapSize.x || y - bottomLeft.y >= mapSize.y)
        {
            Debug.LogWarning("Invalid position!");
            return;
        }
        t[y - bottomLeft.y][x - bottomLeft.x] = isSpawnable;
    }

    /// <summary>
    /// pos에서 가장 가까운 정수 좌표 (x, y)의 타일을 isSpawnable로 설정합니다.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="isSpawnable"></param>
    public void Set(Vector3 pos, bool isSpawnable = false)
    {
        int x = Mathf.RoundToInt(pos.x);
        int y = Mathf.RoundToInt(pos.y);
        Set(x, y, isSpawnable);
    }

    /// <summary>
    /// (x, y) 좌표의 타일에 새 개체를 스폰할 수 있는지 반환합니다.
    /// 만약 맵의 범위를 벗어나는 좌표이면 false를 반환합니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Get(int x, int y)
    {
        if (x < bottomLeft.x || y < bottomLeft.y ||
            x - bottomLeft.x >= mapSize.x || y - bottomLeft.y >= mapSize.y)
        {
            Debug.LogWarning("Invalid position!");
            return false;
        }
        return t[y - bottomLeft.y][x - bottomLeft.x];
    }
}
