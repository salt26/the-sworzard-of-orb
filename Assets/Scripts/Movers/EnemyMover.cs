﻿using System.Collections;
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
    private delegate int Distance(Vector3 a, Vector3 b);
    private Distance distance;
    private bool isDistanceNone = true;

    public enum DistanceType { None, Manhattan, Chebyshev };

    [Header("Public Fields")]
    public GameObject tauntedSprite;            // 도발당한 상태일 때 뜰 이미지의 오브젝트
    public DistanceType distanceType = DistanceType.None;
    public int sightDistance;                   // 순찰 중 플레이어를 발견할 수 있는 최대 택시 거리
    public int leaveDistance;                   // 순찰 경로를 이탈해서 플레이어를 쫓아갈 수 있는 최대 택시 거리
    public List<Vector3> checkpoints;           // TODO 임시로 Inspector에서 설정 가능

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

        InitializeCheckpoints(checkpoints);
        movePattern += PatrolCheckpoints;
        if (distanceType == DistanceType.Manhattan)
        {
            distance += ManhattanDistance;
            isDistanceNone = false;
        }
        else if (distanceType == DistanceType.Chebyshev)
        {
            distance += ChebyshevDistance;
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
                distance += ManhattanDistance;
                isDistanceNone = false;
            }
            else if (distanceType == DistanceType.Chebyshev)
            {
                distance += ChebyshevDistance;
                isDistanceNone = false;
            }
        }

        if (gm.Turn == 0 && isMoved)
        {
            isMoved = false;
        }
        else if (gm.Turn == 1 && !isMoving && !isMoved)
        {
            Vector3 playerPos = PositionToInt(gm.player.GetComponent<Transform>().position);
            Vector3 destination;

            Discover(playerPos, GetCurrentPosition());

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
                    destination = Move1Taxi(tauntedPosition);
                    destination = Move(destination);

                    // 처음 도발당한 위치로 돌아간 경우 정상 경로를 따라 순찰함
                    if (IsSamePosition(destination, tauntedPosition))
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
                if (IsSamePosition(destination, tauntedPosition))
                {
                    isTauntedPositionValid = false;
                }
            }
            else {
                // 시야에 플레이어가 없는 경우 정상 경로를 따라 순찰
                destination = Move1Taxi(movePattern());
                destination = Move(destination);
            }

            Discover(playerPos, destination);
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
            tauntedPosition = GetCurrentPosition();
            isTauntedPositionValid = true;
            hasTaunted = true;
            if (myTauntedSprite == null) myTauntedSprite = Instantiate(tauntedSprite, t);
        }

        // 한 번 경로를 이탈하여 정상 경로로 돌아가던 중 플레이어가 다시 나타난 경우
        if (!hasTaunted && isTauntedPositionValid
            && distance(playerPos, enemyPos) <= sightDistance
            && distance(playerPos, tauntedPosition) <= leaveDistance)
        {
            hasTaunted = true;
            if (myTauntedSprite == null) myTauntedSprite = Instantiate(tauntedSprite, t);
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
        while (headCheckpoints.Count > 0 && IsSamePosition(GetCurrentPosition(), headCheckpoints.Peek()))
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
        while (headCheckpoints.Count > 0 && IsSamePosition(GetCurrentPosition(), headCheckpoints.Peek()))
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
                headCheckpoints.Push(PositionToInt(checkpoints[i]));
            }
        }
    }

    /// <summary>
    /// 1 택시 거리만큼 움직일 때,
    /// 인자로 주어진 목적지까지 가기 위해 나아가야 할 지점을 반환합니다.
    /// 좌우 이동보다 상하 이동을 항상 우선시합니다.
    /// TODO 일단은 장애물이 없다고 가정합니다.
    /// </summary>
    /// <param name="destination"></param>
    /// <returns></returns>
    private Vector3 Move1Taxi(Vector3 destination)
    {
        Vector3 direction;
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
        
        return GetCurrentPosition() + direction;
    }

    private Vector3 Move(Vector3 destination)
    {
        if (IsSamePosition(destination, GetCurrentPosition()))
        {
            // 제자리에 머물러 있는 경우 움직이는 애니메이션 없이 턴 넘김
            // 제자리에서 인접한 네 칸 안에 플레이어가 있으면 공격함
            AttackWithoutMove();
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
            Attack(PositionToInt((destination - GetCurrentPosition()).normalized), false);  // TODO 택시 거리 1칸이 보장되지 않음
            return GetCurrentPosition();
        }
        else
        {
            // TODO 일단은 가려고 하는 곳이 갈 수 없는 지형이면 움직이지 않고 턴 넘김
            // 제자리에서 인접한 네 칸 안에 플레이어가 있으면 공격함
            AttackWithoutMove();
            isMoved = true;
            return GetCurrentPosition();
        }
    }

    private void AttackWithoutMove()
    {
        List<Vector3> melee = new List<Vector3>() { new Vector3(-1f, 0f, 0f), new Vector3(1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 1f, 0f) };
        foreach (Vector3 v in melee)
        {
            Vector3 destination = GetCurrentPosition() + v;
            if (gm.map.GetEntityOnTile(destination) != null
                && gm.map.GetEntityOnTile(destination).GetType().Equals(typeof(Character))
                && ((Character)gm.map.GetEntityOnTile(destination)).type == Character.Type.Player)
            {
                if (destination.x < GetCurrentPosition().x) GetComponent<SpriteRenderer>().flipX = false;
                else if (destination.x > GetCurrentPosition().x) GetComponent<SpriteRenderer>().flipX = true;
                Attack(PositionToInt((destination - GetCurrentPosition()).normalized), false);
            }
        }
    }

    IEnumerator MoveAnimation(Vector3 destination)
    {
        isMoving = true;
        int frame = 20;
        Vector3 origin = t.position;

        // 애니메이션이 시작하기 전에 이동 판정 완료
        gm.map.SetEntityOnTile(null, origin);
        gm.map.SetEntityOnTile(c, destination);
        for (int i = 0; i < frame; i++)
        {
            t.position = Vector3.Lerp(origin, destination, (float)i / frame);
            if (i < frame / 2)
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0.2f, 0.5f, 0.4f, 1f), (float)i / frame * 2);
            else
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0.2f, 0.5f, 0.4f, 1f), (float)(frame - i) / frame * 2);
            yield return null;
        }
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        t.position = destination;
        isMoving = false;
        Attack(PositionToInt((destination - origin).normalized), true); // TODO 택시 거리 1칸이 보장되지 않음
    }

    /// <summary>
    /// 진행 방향(direction)에 있는 대상이 플레이어면 공격합니다. isCharge가 true이면 돌진 공격이 적용됩니다.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="isCharge"></param>
    private void Attack(Vector3 direction, bool isCharge)
    {
        // 플레이어가 진행 방향에 있는지 체크
        Entity e = gm.map.GetEntityOnTile(Mathf.RoundToInt(t.position.x + direction.x), Mathf.RoundToInt(t.position.y + direction.y));
        if (e != null && e.GetType().Equals(typeof(Character)) && ((Character)e).type == Character.Type.Player)
        {
            // 진행 방향에 플레이어가 있을 경우
            Character player = (Character)e;
            float bonus = bonusDamage;         // 돌진 시 추가 대미지 적용
            if (!isCharge) bonus = 1f;
            StartCoroutine(AttackAnimation(direction, player, 
                Mathf.Max(0, (int)(bonus * c.EquippedWeapon.Damage()) - player.armor.Guard())));
        }
        else
        {
            // 진행 방향에 아무것도 없거나 플레이어가 아닌 Entity가 있을 경우
            isMoved = true;
        }
    }

    // TODO direction은 현재 사용하지 않음.
    IEnumerator AttackAnimation(Vector3 direction, Character player, int damage)
    {
        isMoving = true;
        int frame = 20;
        Vector3 origin = t.position;

        for (int i = 0; i < frame; i++)
        {
            if (i < frame / 3)
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0.9f, 1f, 0.2f, 1f), (float)i / frame * 2);
            else
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0.9f, 1f, 0.2f, 1f), (float)(frame - i) / frame * 2);

            if (i == frame / 3)
            {
                player.Damaged(damage);
                isMoved = true;
            }

            yield return null;
        }
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        isMoving = false;
    }

    public override IEnumerator DamagedAnimation(int oldHealth, Slider healthBar = null)
    {
        isMoving = true;
        int frame = 30;

        for (int i = 0; i < frame; i++)
        {
            if (i < frame / 2)
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0.7f, 0f, 0f, 0.4f), (float)i / frame * 2);
            else
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0.7f, 0f, 0f, 0.4f), (float)(frame - i) / frame * 2);

            if (healthBar != null)
                healthBar.value = Mathf.Lerp(c.currentHealth, oldHealth, Mathf.Pow(1 - ((float)i / frame), 2f));

            yield return null;
        }
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        isMoving = false;
    }

    /// <summary>
    /// 사망을 처리합니다.
    /// 사망한 위치에 화폐와 아이템을 드랍합니다.
    /// </summary>
    public override void Death()
    {
        // TODO 크기가 2 이상인 개체에 대해, 개체가 차지하고 있던 모든 타일 고려
        gm.map.SetEntityOnTile(null, t.position);

        int gold = EnemyManager.em.FindEnemyInfo(c.name, c.level).gold;
        gm.map.AddGoldOnTile(gold, t.position);

        List<string> items = EnemyManager.em.FindEnemyInfo(c.name, c.level).items;
        foreach (string itemName in items)
        {
            GameObject item = ItemManager.im.GetItemPrefab(itemName);
            gm.map.AddItemOnTile(item, t.position);
        }
        this.enabled = false;
    }

    /// <summary>
    /// 인자로 주어진 좌표의 x, y값을 정수로 반올림하여 반환합니다.
    /// z값은 그대로 반환됩니다.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private Vector3 PositionToInt(Vector3 position)
    {
        return new Vector3(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), position.z);
    }

    /// <summary>
    /// 현재 이 오브젝트의 위치를 정수 좌표로 반환합니다.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetCurrentPosition()
    {
        return PositionToInt(t.position);
    }

    /// <summary>
    /// 두 좌표가 같은 위치이면 true, 다른 위치이면 false를 반환합니다.
    /// z값은 비교하지 않고, x, y값을 반올림한 정수 값이 각각 같은지 비교합니다.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private bool IsSamePosition(Vector3 a, Vector3 b)
    {
        return (Mathf.RoundToInt(a.x) == Mathf.RoundToInt(b.x)
            && Mathf.RoundToInt(a.y) == Mathf.RoundToInt(b.y));
    }

    /// <summary>
    /// 두 위치 사이의 택시 거리를 반환합니다.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private int ManhattanDistance(Vector3 a, Vector3 b)
    {
        return Mathf.RoundToInt(Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y));
    }

    /// <summary>
    /// 두 위치 사이의 체스보드 거리를 반환합니다.
    /// 체스에서 킹이 이동할 때 걸리는 최소 이동 횟수와 같습니다.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private int ChebyshevDistance(Vector3 a, Vector3 b)
    {
        return Mathf.RoundToInt(Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y)));
    }
}
