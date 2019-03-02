using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMover : Mover {

    GameManager gm;
    Transform t;
    
    private bool isMoved;       // 이동이 끝나면 true
    private const float bonusDamage = 1.5f;     // 돌진 시 곱해지는 추가 대미지

    // TODO SerializeField는 디버깅 용도로 넣은 것
    [SerializeField]
    private bool hasTaunted;    // 근처에서 플레이어를 만난 경우 true
    [SerializeField]
    private bool isTauntedPositionValid;        // 순찰 경로를 이탈해 있는 동안 true
    [SerializeField]
    private Vector3 tauntedPosition;            // 플레이어를 처음 만났을 때의 본인의 위치
    private delegate Vector3 MovePattern();
    private MovePattern movePattern;
    private Stack<Vector3> headCheckpoints;     // 순찰 시 가야 할 지점(main stack)
    private Stack<Vector3> passedCheckpoints;   // 순찰 시 지나온 지점(sub stack)
    [SerializeField]
    private GameObject myTauntedSprite;         // 도발당한 상태일 때 뜨는 !에 대한 레퍼런스
    private GameObject myDistanceSprite;        // 바닥에 범위를 표시해주는 스프라이트에 대한 레퍼런스
    private delegate int Distance(Vector3 a, Vector3 b);
    private Distance distance;
    private bool isDistanceNone = true;

    public enum DistanceType { None, Manhattan, Chebyshev };

    [Header("Public Fields")]
    public GameObject tauntedSprite;            // 도발당한 상태일 때 뜰 이미지의 프리팹
    public DistanceType distanceType = DistanceType.None;
    public int sightDistance;                   // 순찰 중 플레이어를 발견할 수 있는 최대 거리
    public int leaveDistance;                   // 순찰 경로를 이탈해서 플레이어를 쫓아갈 수 있는 최대 거리
    public List<Vector3> checkpoints;           // TODO 임시로 Inspector에서 설정 가능
    public GameObject distanceSprite;           // 바닥에 범위를 표시해 줄 스프라이트의 프리팹

    public bool Moved
    {
        get
        {
            return isMoved;
        }
    }

    // Use this for initialization
    void Start()
    {
        t = GetComponent<Transform>();
        c = GetComponent<Character>();
        gm = GameManager.gm;
        isMoving = false;
        myTauntedSprite = null;
        myDistanceSprite = null;

        InitializeCheckpoints(checkpoints);
        movePattern += PatrolCheckpoints;
        if (distanceType == DistanceType.Manhattan)
        {
            distance += VectorUtility.ManhattanDistance;
            isDistanceNone = false;
        }
        else if (distanceType == DistanceType.Chebyshev)
        {
            distance += VectorUtility.ChebyshevDistance;
            isDistanceNone = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gm == null)
        {
            gm = GameManager.gm;
            return;
        }

        if (isDistanceNone)
        {
            if (distanceType == DistanceType.None)
            {
                Debug.LogWarning("Enemy distanceType is None!");
                return;                     // Character의 Initialize에서 설정해 줄 때까지 대기
            }
            else if (distanceType == DistanceType.Manhattan)
            {
                distance += VectorUtility.ManhattanDistance;
                isDistanceNone = false;
            }
            else if (distanceType == DistanceType.Chebyshev)
            {
                distance += VectorUtility.ChebyshevDistance;
                isDistanceNone = false;
            }
        }

        if (!gm.IsSceneLoaded) return;

        if (gm.Turn == 0 && isMoved)
        {
            isMoved = false;
        }
        else if (gm.Turn == 1 && !isMoving && !isMoved)
        {
            if (c.hasStuned)
            {
                c.hasStuned = false;
                StartCoroutine(StunedAnimation());
                return;
            }
            Vector3 playerPos = VectorUtility.PositionToInt(gm.player.GetComponent<Transform>().position);
            Vector3 destination;

            Discover(playerPos, GetCurrentPosition());

            if (!AttackWithoutMove())
            {
                if (hasTaunted)
                {
                    // 한 번 도발당한 경우 처음 도발당한 위치에서 leaveDistance만큼 멀어지기 전까지 플레이어를 쫓아감
                    // 플레이어가 시야에서 벗어나도 쫓아감
                    destination = Move1Taxi(playerPos);
                    if (distance(tauntedPosition, destination) <= leaveDistance)
                    {
                        destination = Move(destination);
                    }
                    else
                    {
                        // 플레이어가 사라지면 도발 상태가 풀리고 처음 도발당한 위치로 돌아감
                        hasTaunted = false;
                        if (myTauntedSprite != null)
                        {
                            Destroy(myTauntedSprite);
                            myTauntedSprite = null;
                        }
                        if (myDistanceSprite != null)
                        {
                            myDistanceSprite.GetComponent<DistanceSprite>().Disappear();
                            myDistanceSprite = null;
                        }
                        destination = Move1Taxi(tauntedPosition);
                        destination = Move(destination);

                        // 처음 도발당한 위치로 돌아간 경우 정상 경로를 따라 순찰함
                        if (VectorUtility.IsSamePosition(destination, tauntedPosition))
                        {
                            isTauntedPositionValid = false;
                        }
                    }
                }
                else if (isTauntedPositionValid)
                {
                    // 도발 상태가 풀렸지만 아직 처음 도발당한 위치로 돌아가지 못한 경우
                    destination = Move1Taxi(tauntedPosition);
                    destination = Move(destination);

                    // 처음 도발당한 위치로 돌아간 경우 정상 경로를 따라 순찰함
                    if (VectorUtility.IsSamePosition(destination, tauntedPosition))
                    {
                        isTauntedPositionValid = false;
                    }
                }
                else
                {
                    // 시야에 플레이어가 없는 경우 정상 경로를 따라 순찰
                    destination = Move1Taxi(movePattern(), true);
                    destination = Move(destination);
                }

                Discover(playerPos, destination);
            }
        }
    }

    /// <summary>
    /// 플레이어를 발견하고, 시야 내에 플레이어가 있으면 도발당한 상태가 됩니다.
    /// </summary>
    /// <param name="playerPos"></param>
    /// <param name="enemyPos"></param>
    private void Discover(Vector3 playerPos, Vector3 enemyPos)
    {
        if (!hasTaunted && !isTauntedPositionValid
                && distance(playerPos, enemyPos) <= sightDistance)
        {
            // 도발당한 현재 위치(이동 후 위치)를 기억
            // TODO 장애물에 막혀서 못 움직인 경우 버그가 생길 수 있음
            tauntedPosition = enemyPos;
            isTauntedPositionValid = true;
            hasTaunted = true;
            if (myTauntedSprite == null) myTauntedSprite = Instantiate(tauntedSprite, t);
            if (myDistanceSprite == null) StartCoroutine(SightDistanceAnimation(enemyPos));
        }

        // 한 번 경로를 이탈하여 정상 경로로 돌아가던 중 플레이어가 다시 나타난 경우
        if (!hasTaunted && isTauntedPositionValid
            && distance(playerPos, enemyPos) <= sightDistance
            && distance(playerPos, tauntedPosition) <= leaveDistance)
        {
            hasTaunted = true;
            if (myTauntedSprite == null) myTauntedSprite = Instantiate(tauntedSprite, t);
            if (myDistanceSprite == null) StartCoroutine(SightDistanceAnimation(enemyPos));
            // TODO 도발 상태가 풀려 돌아가던 중에 플레이어가 다시 최대 공격 범위 안에 들어온 경우, 도발 상태가 되면서 공격은 하는데, 느낌표가 뜨지 않는 버그가 있다.
        }
    }

    /// <summary>
    /// 설정된 순찰 경로를 따라서, 지금 가야 할 곳을 반환하는 함수입니다.
    /// MovePattern 함수 대리자에 의해 호출됩니다.
    /// 이 함수가 호출되기 전에, 순찰 경로가 초기화되어 있어야 합니다.
    /// </summary>
    /// <returns></returns>
    private Vector3 PatrolCheckpoints()
    {
        // 경로 초기화가 되지 않은 경우
        if (headCheckpoints == null || passedCheckpoints == null)
            return GetCurrentPosition();

        // 바로 다음에 가야 할 위치가 현재 위치와 같으면, 그 위치를 지나온 경로로 취급합니다.
        while (headCheckpoints.Count > 0 && VectorUtility.IsSamePosition(GetCurrentPosition(), headCheckpoints.Peek()))
        {
            passedCheckpoints.Push(headCheckpoints.Pop());
        }

        // 더 이상 갈 곳이 없으면, 지나왔던 경로를 따라 되돌아갑니다.
        if (headCheckpoints.Count == 0)
        {
            Stack<Vector3> tempStack = headCheckpoints;
            headCheckpoints = passedCheckpoints;
            passedCheckpoints = tempStack;
        }

        // 돌아갈 경로에서 가장 먼저 가게 될 곳은 항상 현재 위치이므로,
        // 현재 위치가 아니면서 처음으로 가야 할 곳을 찾습니다.
        while (headCheckpoints.Count > 0 && VectorUtility.IsSamePosition(GetCurrentPosition(), headCheckpoints.Peek()))
        {
            passedCheckpoints.Push(headCheckpoints.Pop());
        }

        // 만약 순찰하지 않는(순찰 경로가 제자리뿐인) 경우 현재 위치를 반환합니다.
        // 일반적인 경우 지금 먼저 가야 할 곳의 위치를 반환합니다.
        if (headCheckpoints.Count == 0)
        {
            return GetCurrentPosition();
        }
        else
        {
            return headCheckpoints.Peek();
        }
    }

    /// <summary>
    /// 순찰 경로를 초기화하는 함수입니다.
    /// </summary>
    /// <param name="checkpoints">들러야 하는 지점의 목록(순서대로)</param>
    private void InitializeCheckpoints(List<Vector3> checkpoints)
    {
        headCheckpoints = new Stack<Vector3>();
        passedCheckpoints = new Stack<Vector3>();

        passedCheckpoints.Push(GetCurrentPosition());
        if (checkpoints != null)
        {
            for (int i = checkpoints.Count - 1; i >= 0; i--)
            {
                headCheckpoints.Push(VectorUtility.PositionToInt(checkpoints[i]));
            }
        }
    }

    /// <summary>
    /// 1 택시 거리만큼 움직일 때,
    /// 인자로 주어진 목적지까지 가기 위해 나아가야 할 지점을 반환합니다.
    /// 좌우 이동을 우선시할 확률과 상하 이동을 우선시할 확률이 각각 1/2씩입니다.
    /// TODO 일단은 장애물이 없다고 가정합니다.
    /// </summary>
    /// <param name="destination"></param>
    /// <returns></returns>
    private Vector3 Move1Taxi(Vector3 destination)
    {
        Vector3 direction;
        int r = Random.Range(0, 2);
        if (r == 0)
        {
            if (GetCurrentPosition().y < destination.y)
            {
                direction = new Vector3(0f, 1f, 0f);
            }
            else if (GetCurrentPosition().y > destination.y)
            {
                direction = new Vector3(0f, -1f, 0f);
            }
            else if (GetCurrentPosition().x < destination.x)
            {
                direction = new Vector3(1f, 0f, 0f);
            }
            else if (GetCurrentPosition().x > destination.x)
            {
                direction = new Vector3(-1f, 0f, 0f);
            }
            else
            {
                direction = Vector3.zero;
            }
        }
        else
        {
            if (GetCurrentPosition().x < destination.x)
            {
                direction = new Vector3(1f, 0f, 0f);
            }
            else if (GetCurrentPosition().x > destination.x)
            {
                direction = new Vector3(-1f, 0f, 0f);
            }
            else if(GetCurrentPosition().y < destination.y)
            {
                direction = new Vector3(0f, 1f, 0f);
            }
            else if (GetCurrentPosition().y > destination.y)
            {
                direction = new Vector3(0f, -1f, 0f);
            }
            else 
            {
                direction = Vector3.zero;
            }
        }
        
        return GetCurrentPosition() + direction;
    }

    /// <summary>
    /// 1 택시 거리만큼 움직일 때,
    /// 인자로 주어진 목적지까지 가기 위해 나아가야 할 지점을 반환합니다.
    /// isUpDownFirst가 true이면 좌우 이동보다 상하 이동을 항상 우선시하며,
    /// false이면 좌우 이동을 항상 우선시합니다.
    /// TODO 일단은 장애물이 없다고 가정합니다.
    /// </summary>
    /// <param name="destination"></param>
    /// <returns></returns>
    private Vector3 Move1Taxi(Vector3 destination, bool isUpDownFirst)
    {
        Vector3 direction;
        if (isUpDownFirst)
        {
            if (GetCurrentPosition().y < destination.y)
            {
                direction = new Vector3(0f, 1f, 0f);
            }
            else if (GetCurrentPosition().y > destination.y)
            {
                direction = new Vector3(0f, -1f, 0f);
            }
            else if (GetCurrentPosition().x < destination.x)
            {
                direction = new Vector3(1f, 0f, 0f);
            }
            else if (GetCurrentPosition().x > destination.x)
            {
                direction = new Vector3(-1f, 0f, 0f);
            }
            else
            {
                direction = Vector3.zero;
            }
        }
        else
        {
            if (GetCurrentPosition().x < destination.x)
            {
                direction = new Vector3(1f, 0f, 0f);
            }
            else if (GetCurrentPosition().x > destination.x)
            {
                direction = new Vector3(-1f, 0f, 0f);
            }
            else if (GetCurrentPosition().y < destination.y)
            {
                direction = new Vector3(0f, 1f, 0f);
            }
            else if (GetCurrentPosition().y > destination.y)
            {
                direction = new Vector3(0f, -1f, 0f);
            }
            else
            {
                direction = Vector3.zero;
            }
        }

        return GetCurrentPosition() + direction;
    }

    private Vector3 Move(Vector3 destination)
    {
        if (VectorUtility.IsSamePosition(destination, GetCurrentPosition()))
        {
            // 제자리에 머물러 있는 경우 움직이는 애니메이션 없이 턴 넘김
            // 제자리에서 인접한 네 칸 안에 플레이어가 있으면 공격함
            if (!AttackWithoutMove())
                isMoved = true;
            return destination;
        }
        else if (gm.map.CanMoveToTile(destination))
        {
            // TODO 한 턴에 2칸 이상을 이동하는 경우, 지나는 모든 타일에 대한 고려가 필요함
            if (destination.x < GetCurrentPosition().x) GetComponent<SpriteRenderer>().flipX = false;
            else if (destination.x > GetCurrentPosition().x) GetComponent<SpriteRenderer>().flipX = true;
            StartCoroutine(MoveAnimation(destination));
            return destination;
        }
        else if (gm.map.GetTypeOfTile(destination) == 0 && gm.map.GetEntityOnTile(destination) != null
            && gm.map.GetEntityOnTile(destination).GetType().Equals(typeof(Character))
            && ((Character)gm.map.GetEntityOnTile(destination)).type == Character.Type.Player)
        {
            // 진행 방향으로 한 칸 앞에 플레이어가 있는 경우
            if (destination.x < GetCurrentPosition().x) GetComponent<SpriteRenderer>().flipX = false;
            else if (destination.x > GetCurrentPosition().x) GetComponent<SpriteRenderer>().flipX = true;
            Attack(VectorUtility.PositionToInt((destination - GetCurrentPosition()).normalized), false);  // TODO 택시 거리 1칸이 보장되지 않음
            return GetCurrentPosition();
        }
        else
        {
            // TODO 일단은 가려고 하는 곳이 갈 수 없는 지형이면 움직이지 않고 턴 넘김
            // 제자리에서 인접한 네 칸 안에 플레이어가 있으면 공격함
            if (!AttackWithoutMove())
                isMoved = true;
            return GetCurrentPosition();
        }
    }

    /// <summary>
    /// 이동하지 않고 상하좌우로 붙어 있는 플레이어를 공격합니다.
    /// 공격에 성공하면 턴을 넘기고 true를 반환합니다.
    /// 플레이어가 주변에 없으면 턴을 넘기지 않고 false를 반환합니다.
    /// </summary>
    /// <returns></returns>
    private bool AttackWithoutMove()
    {
        bool isAttacked = false;
        List<Vector3> melee = new List<Vector3>();
        for (int i = 0; i <= c.EquippedWeapon.Range; i++) {
            melee.Add(new Vector3(-1f * i, 0f, 0f));
            melee.Add(new Vector3(1f * i, 0f, 0f));
            melee.Add(new Vector3(0f, -1f * i, 0f));
            melee.Add(new Vector3(0f, 1f * i, 0f));
        }
        foreach (Vector3 v in melee)
        {
            Vector3 destination = GetCurrentPosition() + v;
            if (gm.map.GetEntityOnTile(destination) != null
                && gm.map.GetEntityOnTile(destination).GetType().Equals(typeof(Character))
                && ((Character)gm.map.GetEntityOnTile(destination)).type == Character.Type.Player)
            {
                if (destination.x < GetCurrentPosition().x) GetComponent<SpriteRenderer>().flipX = false;
                else if (destination.x > GetCurrentPosition().x) GetComponent<SpriteRenderer>().flipX = true;
                Attack(VectorUtility.PositionToInt((destination - GetCurrentPosition()).normalized), false);
                // Debug.Log("AttackWithoutMove");
                isAttacked = true;
                break;
            }
        }
        return isAttacked;
    }

    IEnumerator MoveAnimation(Vector3 destination)
    {
        isMoving = true;
        int frame = 18;
        Vector3 origin = t.position;

        // 애니메이션이 시작하기 전에 이동 판정 완료
        gm.map.SetEntityOnTile(null, origin);
        gm.map.SetEntityOnTile(c, destination);
        for (int i = 0; i < frame; i++)
        {
            t.position = Vector3.Lerp(origin, destination, (float)i / frame);
            yield return null;
        }
        t.position = destination;
        isMoving = false;
        Attack(VectorUtility.PositionToInt((destination - origin).normalized), true); // TODO 택시 거리 1칸이 보장되지 않음
    }

    /// <summary>
    /// 진행 방향(direction)에 있는 대상이 플레이어면 공격합니다. isCharge가 true이면 돌진 공격이 적용됩니다.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="isCharge"></param>
    private void Attack(Vector3 direction, bool isCharge)
    {
        bool attacked = false;
        for (int i = 1; i <= c.EquippedWeapon.Range; i++)
        {
            // 플레이어가 진행 방향에 있는지 체크
            Entity e = gm.map.GetEntityOnTile(Mathf.RoundToInt(t.position.x + direction.x * i), Mathf.RoundToInt(t.position.y + direction.y * i));
            if (e != null && e.GetType().Equals(typeof(Character)) && ((Character)e).type == Character.Type.Player)
            {
                // 진행 방향에 플레이어가 있을 경우
                attacked = true;
                Character player = (Character)e;
                float bonus = bonusDamage;         // 돌진 시 추가 대미지 적용
                if (!isCharge) bonus = 1f;
                StartCoroutine(AttackAnimation(direction, player,
                    //Mathf.Max(0, (int)(bonus * c.EquippedWeapon.Attack()) - player.armor.Defense())));
                    player.armor.ComputeDamage(c.EquippedWeapon, bonus), (bonus > 1f)));
                break;
            }
        }
        if (!attacked)
        {
            // 진행 방향에 아무것도 없거나 플레이어가 아닌 Entity가 있을 경우
            isMoved = true;
        }
    }
    
    IEnumerator AttackAnimation(Vector3 direction, Character player, int damage, bool isCritical)
    {
        isMoving = true;
        int frame = 20;     // int이어야 동작함. float이면 동작하지 않음
        float frame2 = 6;
        Vector3 origin = t.position;

        for (int i = 0; i < frame2; i++)
        {
            yield return null;
        }

        for (int i = 0; i < frame; i++)
        {
            if (i < frame / 2)
            {
                if (!direction.Equals(new Vector3()))
                    t.position = Vector3.Lerp(origin, origin + 0.15f * direction, (float)i / frame * 2);
            }
            else
            {
                if (!direction.Equals(new Vector3()))
                    t.position = Vector3.Lerp(origin, origin + 0.15f * direction, (float)(frame - i) / frame * 2);
            }

            if (i == frame / 2)
            {
                player.Damaged(damage, direction, isCritical);
                player.DamagedWithEffects(c.EquippedWeapon.afterAttackEffect);
                isMoved = true;
            }

            yield return null;
        }
        if (!direction.Equals(new Vector3()))
            t.position = origin;
        isMoving = false;
    }

    public override IEnumerator DamagedAnimation(int oldHealth, Slider healthBar = null, StatusUI statusUI = null, Vector3 direction = new Vector3(), bool isCritical = false)
    {
        isMoving = true;
        float frame = 16, frame2 = 0;

        c.SelectThisCharacter();
        Vector3 originalPosition = t.position;
        Color originalColor = GetComponent<SpriteRenderer>().color;
        DamageNumber.DamageType dt = DamageNumber.DamageType.Normal;
        if (isCritical) dt = DamageNumber.DamageType.Critical;

        if (!direction.Equals(new Vector3()))
        {
            GameObject g = Instantiate(damageNumber, c.canvas.GetComponent<Transform>());
            g.GetComponent<DamageNumber>().Initialize(oldHealth - c.currentHealth, dt);
        }

        for (int i = 0; i < frame + frame2; i++)
        {
            if (i < frame / 2)
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(originalColor, new Color(0.7f, 0f, 0f, 0.6f), i / frame * 2);
                if (!direction.Equals(new Vector3()))
                    t.position = Vector3.Lerp(originalPosition, originalPosition + 0.2f * direction, i / frame * 2);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(originalColor, new Color(0.7f, 0f, 0f, 0.6f), (frame + frame2 - i) / (frame / 2 + frame2));
                if (!direction.Equals(new Vector3()))
                    t.position = Vector3.Lerp(originalPosition, originalPosition + 0.2f * direction, (frame + frame2 - i) / (frame / 2 + frame2));
            }

            if (i < frame)
            {
                float f = Mathf.Lerp(c.currentHealth, oldHealth, Mathf.Pow(1 - (i / frame), 2f));
                if (healthBar != null)
                    healthBar.value = f;
                if (statusUI != null)
                {
                    statusUI.UpdateAll(c, (int)f);
                }
            }

            yield return null;
        }
        if (healthBar != null)
            healthBar.value = c.currentHealth;
        if (statusUI != null)
        {
            statusUI.UpdateAll(c, c.currentHealth);
        }
        GetComponent<SpriteRenderer>().color = originalColor;
        if (!direction.Equals(new Vector3()))
            t.position = originalPosition;
        /*
        for (int i = 0; i < frame3; i++)
        {
            yield return null;
        }
        */
        isMoving = false;
    }
    
    public IEnumerator StunedAnimation()
    {
        isMoving = true;
        int frame = 25;

        if (myStunMark != null)
        {
            Destroy(myStunMark);
            myStunMark = null;
        }
        myStunMark = Instantiate(stunEffect, c.GetComponent<Transform>());

        for (int i = 0; i < frame; i++)
        {
            if (i == frame * 2 / 3)
                isMoved = true;
            yield return null;
        }
        if (myStunMark != null)
        {
            Destroy(myStunMark);
            myStunMark = null;
        }
        isMoving = false;
    }

    /// <summary>
    /// 사망을 처리합니다.
    /// 사망한 위치에 화폐와 아이템을 드랍합니다.
    /// </summary>
    public override void Death()
    {
        if (c.Equals(gm.SelectedCharacter))
        {
            gm.SelectCharacter(null);
        }
        // TODO 크기가 2 이상인 개체에 대해, 개체가 차지하고 있던 모든 타일 고려
        gm.map.SetEntityOnTile(null, t.position);

        if (myTauntedSprite != null)
        {
            Destroy(myTauntedSprite);
            myTauntedSprite = null;
        }
        if (myDistanceSprite != null)
        {
            myDistanceSprite.GetComponent<DistanceSprite>().Disappear();
            myDistanceSprite = null;
        }

        int gold = EnemyManager.em.FindEnemyInfo(c.name, c.level).gold;
        gm.map.AddGoldOnTile(gold, t.position);
        
        List<EnemyDropItemInfo> items = EnemyManager.em.FindEnemyInfo(c.name, c.level).dropItems;
        foreach (EnemyDropItemInfo di in items)
        {
            for (int i = 0; i < di.count; i++)
            {
                if (Random.Range(0f, 1f) < di.probability)
                    gm.map.AddItemOnTile(di.itemID, t.position);
            }
        }
        this.enabled = false;
    }

    /// <summary>
    /// 현재 이 오브젝트의 위치를 정수 좌표로 반환합니다.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetCurrentPosition()
    {
        return VectorUtility.PositionToInt(t.position);
    }

    /// <summary>
    /// 적의 시야 범위를 바닥에 표시합니다.
    /// 이 표시는 점점 투명해지다가 사라집니다.
    /// </summary>
    /// <param name="enemyPos"></param>
    /// <returns></returns>
    IEnumerator SightDistanceAnimation(Vector3 enemyPos)
    {
        if (myDistanceSprite != null)
        {
            myDistanceSprite.GetComponent<DistanceSprite>().Disappear();
            myDistanceSprite = null;
        }
        Vector3 v = new Vector3(enemyPos.x, enemyPos.y, -0.25f);
        myDistanceSprite = Instantiate(distanceSprite, v, Quaternion.identity);
        if (distanceType != DistanceType.None)
            myDistanceSprite.GetComponent<SpriteRenderer>().sprite =
                Resources.Load(distanceType.ToString() + sightDistance, typeof(Sprite)) as Sprite;
        float frame = 40f;
        for (int i = 0; i < frame; i++)
        {
            if (myDistanceSprite == null) break;
            myDistanceSprite.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.yellow, ColorManager.ChangeAlpha(Color.yellow, 0f), i / frame);
            yield return null;
        }
        myDistanceSprite.GetComponent<DistanceSprite>().Disappear(0);
        myDistanceSprite = null;
        StartCoroutine(LeaveDistanceAnimation());
    }

    /// <summary>
    /// 적의 최대 이동 범위를, 적이 처음 도발당한 위치를 기준으로 표시합니다.
    /// 이 표시는 점점 선명해지고 자동으로 사라지지 않습니다.
    /// </summary>
    /// <returns></returns>
    IEnumerator LeaveDistanceAnimation()
    {
        if (myDistanceSprite != null)
        {
            myDistanceSprite.GetComponent<DistanceSprite>().Disappear();
            myDistanceSprite = null;
        }
        Vector3 v = new Vector3(tauntedPosition.x, tauntedPosition.y, -0.25f);
        myDistanceSprite = Instantiate(distanceSprite, v, Quaternion.identity);
        if (distanceType != DistanceType.None)
            myDistanceSprite.GetComponent<SpriteRenderer>().sprite =
                Resources.Load(distanceType.ToString() + leaveDistance, typeof(Sprite)) as Sprite;
        float frame = 20f;
        for (int i = 0; i < frame; i++)
        {
            if (myDistanceSprite == null) break;
            myDistanceSprite.GetComponent<SpriteRenderer>().color =
                Color.Lerp(new Color(0.6f, 0f, 0f, 0f), ColorManager.ChangeAlpha(new Color(0.6f, 0f, 0f, 0f), 1f), i / frame);
            yield return null;
        }
    }
}
