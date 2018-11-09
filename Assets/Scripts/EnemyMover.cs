using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour {

    GameManager gm;
    Transform t;

    private bool isMoving;  // 이동 중에 true
    private bool isMoved;   // 이동이 끝나면 true
    //private Vector3 direction = new Vector3(-1f, 0f, 0f);   // PatrolLR 함수에서 사용
    private delegate Vector3 MovePattern();
    private MovePattern movePattern;

    private Stack<Vector3> headCheckpoints;     // 가야 할 지점(main stack)
    private Stack<Vector3> passedCheckpoints;   // 지나온 지점(sub stack)

    public List<Vector3> checkpoints;        // TODO: 임시로 Inspector에서 설정 가능

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
            Vector3 destination = Move1Taxi(movePattern());
            if (gm.map.GetTypeOfTile(Mathf.RoundToInt(destination.x), Mathf.RoundToInt(destination.y)) == 0)
            {
                if (destination.x < t.position.x) GetComponent<SpriteRenderer>().flipX = false;
                else if (destination.x > t.position.x) GetComponent<SpriteRenderer>().flipX = true;
                StartCoroutine(MoveAnimation(destination));
            }
            else
            {
                // TODO 일단은 가려고 하는 곳이 갈 수 없는 지형이면 움직이지 않고 턴 넘김
                isMoved = true;
            }
        }
    }

    /*
    private Vector3 PatrolLR()
    {
        // 하드코딩
        // 언제나 x좌표가 2, 3, 4 중 하나라고 가정
        int left = 2;
        int right = 4;

        if (Mathf.RoundToInt(t.position.x) <= left)
        {
            direction = new Vector3(1f, 0f, 0f);
        }
        else if (Mathf.RoundToInt(t.position.x) >= right)
        {
            direction = new Vector3(-1f, 0f, 0f);
        }

        return t.position + direction;
    }
    */

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
        /*
        // 체크포인트 목록 첫 번째 원소가 현재 위치일 경우, 제거
        while (checkpoints != null && checkpoints.Count > 0
            && IsSamePosition(PositionToInt(checkpoints[0]), CurrentPosition()))
        {
            checkpoints.RemoveAt(0);
        }
        */
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

    IEnumerator MoveAnimation(Vector3 destination)
    {
        isMoving = true;
        int frame = 40;
        Vector3 origin = t.position;
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
}
