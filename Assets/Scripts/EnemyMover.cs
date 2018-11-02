using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour {

    GameManager gm;
    Transform t;

    private bool isMoving;  // 이동 중에 true
    private bool isMoved;   // 이동이 끝나면 true
    private Vector3 direction = new Vector3(-1f, 0f, 0f);   // Patrol 함수에서 사용
    private delegate Vector3 MovePattern();
    private MovePattern movePattern;

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

        movePattern += PatrolLR;
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
            Vector3 destination = movePattern();
            if (gm.map.GetTypeOfTile(Mathf.RoundToInt(destination.x), Mathf.RoundToInt(destination.y)) == 0)
            {
                if (destination.x < t.position.x) GetComponent<SpriteRenderer>().flipX = false;
                else GetComponent<SpriteRenderer>().flipX = true;
                StartCoroutine(MoveAnimation(destination));
            }
            else
            {
                // 일단은 가려고 하는 곳이 갈 수 없는 지형이면 움직이지 않고 턴 넘김
                isMoved = true;
            }
        }
    }

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
}
