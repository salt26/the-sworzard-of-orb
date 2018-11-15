using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour {

    GameManager gm;
    Transform t;
    
    private bool isMoving;      // 이동 중에 true
    private bool isMoved;       // 이동이 끝나면 true

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

    [Header("Public Fields")]
    public int sightDistance;   // 순찰 중 플레이어를 발견할 수 있는 최대 택시 거리
    public int leaveDistance;   // 순찰 경로를 이탈해서 플레이어를 쫓아갈 수 있는 최대 택시 거리
    public List<Vector3> checkpoints;           // TODO: 임시로 Inspector에서 설정 가능

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
        gm = GameManager.gm;
        isMoving = false;

        InitializeCheckpoints(checkpoints);
        movePattern += PatrolCheckpoints;
    }

    // Update is called once per frame
    void Update()
    {
        if (gm == null)
        {
            gm = GameManager.gm;
            return;
        }
        if (gm.Turn == 0 && isMoved)
        {
            isMoved = false;
        }
        else if (gm.Turn == 1 && !isMoving && !isMoved)
        {
            Vector3 playerPos = PositionToInt(gm.character.GetComponent<Transform>().position);
            Vector3 destination;

            if (hasTaunted)
            {
                // 한 번 도발당한 경우 처음 도발당한 위치에서 leaveDistance만큼 멀어지기 전까지 플레이어를 쫓아감
                // 플레이어가 시야에서 벗어나도 쫓아감
                destination = Move1Taxi(playerPos);
                if (Distance(tauntedPosition, destination) <= leaveDistance)
                {
                    Move(destination);
                }
                else
                {
                    // 플레이어가 사라지면 도발 상태가 풀리고 처음 도발당한 위치로 돌아감
                    hasTaunted = false;
                    destination = Move1Taxi(tauntedPosition);
                    Move(destination);

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
                Move(destination);

                // 처음 도발당한 위치로 돌아간 경우 정상 경로를 따라 순찰함
                if (IsSamePosition(destination, tauntedPosition))
                {
                    isTauntedPositionValid = false;
                }
            }
            else {
                // 시야에 플레이어가 없는 경우 정상 경로를 따라 순찰
                destination = Move1Taxi(movePattern());
                Move(destination);
            }
            
            // 정상 경로를 순찰하던 중 플레이어를 발견한 경우
            if (!hasTaunted && !isTauntedPositionValid
                && Distance(playerPos, destination) <= sightDistance)
            {
                // 도발당한 현재 위치(이동 후 위치)를 기억
                // TODO 장애물에 막혀서 못 움직인 경우 버그가 생길 수 있음
                tauntedPosition = destination;
                isTauntedPositionValid = true;
                hasTaunted = true;
            }

            // 한 번 경로를 이탈하여 정상 경로로 돌아가던 중 플레이어가 다시 나타난 경우
            if (!hasTaunted && isTauntedPositionValid
                && Distance(playerPos, tauntedPosition) <= leaveDistance)
            {
                hasTaunted = true;
            }
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
            return CurrentPosition();

        // 바로 다음에 가야 할 위치가 현재 위치와 같으면, 그 위치를 지나온 경로로 취급합니다.
        while (headCheckpoints.Count > 0 && IsSamePosition(CurrentPosition(), headCheckpoints.Peek()))
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
        while (headCheckpoints.Count > 0 && IsSamePosition(CurrentPosition(), headCheckpoints.Peek()))
        {
            passedCheckpoints.Push(headCheckpoints.Pop());
        }

        // 만약 순찰하지 않는(순찰 경로가 제자리뿐인) 경우 현재 위치를 반환합니다.
        // 일반적인 경우 지금 먼저 가야 할 곳의 위치를 반환합니다.
        if (headCheckpoints.Count == 0)
        {
            return CurrentPosition();
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

        passedCheckpoints.Push(CurrentPosition());
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
        if (CurrentPosition().y < destination.y)
        {
            direction = new Vector3(0f, 1f, 0f);
        }
        else if (CurrentPosition().y > destination.y)
        {
            direction = new Vector3(0f, -1f, 0f);
        }
        else if (CurrentPosition().x < destination.x)
        {
            direction = new Vector3(1f, 0f, 0f);
        }
        else if (CurrentPosition().x > destination.x)
        {
            direction = new Vector3(-1f, 0f, 0f);
        }
        else
        {
            direction = Vector3.zero;
        }

        return CurrentPosition() + direction;
    }

    private void Move(Vector3 destination)
    {
        if (IsSamePosition(destination, CurrentPosition()))
        {
            // 제자리에 머물러 있는 경우 움직이는 애니메이션 없이 턴 넘김
            isMoved = true;
        }
        else if (gm.map.CanMoveToTile(destination))
        {
            // TODO 한 턴에 2칸 이상을 이동하는 경우, 지나는 모든 타일에 대한 고려가 필요함
            if (destination.x < CurrentPosition().x) GetComponent<SpriteRenderer>().flipX = false;
            else if (destination.x > CurrentPosition().x) GetComponent<SpriteRenderer>().flipX = true;
            StartCoroutine(MoveAnimation(destination));
        }
        else
        {
            // TODO 일단은 가려고 하는 곳이 갈 수 없는 지형이면 움직이지 않고 턴 넘김
            isMoved = true;
        }
    }

    IEnumerator MoveAnimation(Vector3 destination)
    {
        isMoving = true;
        int frame = 30;
        Vector3 origin = t.position;

        // 애니메이션이 시작하기 전에 이동 판정 완료
        gm.map.SetObjectOnTile(null, origin);
        gm.map.SetObjectOnTile(gameObject, destination);
        for (int i = 0; i < frame; i++)
        {
            t.position = Vector3.Lerp(origin, destination, (float)i / frame);
            if (i < frame / 2)
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(1f, 0.4f, 0.8f, 1f), (float)i / frame * 2);
            else
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(1f, 0.4f, 0.8f, 1f), (float)(frame - i) / frame * 2);
            yield return null;
        }
        t.position = destination;
        isMoving = false;
        isMoved = true;
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
    private Vector3 CurrentPosition()
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

    private int Distance(Vector3 a, Vector3 b)
    {
        return Mathf.RoundToInt(Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y));
    }
}
