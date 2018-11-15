using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour {

    GameManager gm;
    Transform t;

    private bool isMoving;

	// Use this for initialization
	void Start () {
        t = GetComponent<Transform>();
        gm = GameManager.gm;
        isMoving = false;
	}
	
	void Update () {
        if (gm == null)
        {
            gm = GameManager.gm;
            return;
        }
		if (gm.Turn == 0 && !isMoving)
        {
            // TODO 맵을 보고 갈 수 있는 지형인지 확인할 것
            if (Input.GetKeyDown(KeyCode.LeftArrow) &&
                gm.map.CanMoveToTile(Mathf.RoundToInt(t.position.x - 1f), Mathf.RoundToInt(t.position.y)))  // TODO true 붙여서 추락사 가능하게
            {
                GetComponent<SpriteRenderer>().flipX = false;
                StartCoroutine(MoveAnimation(new Vector3(-1f, 0f, 0f)));
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) &&
                gm.map.CanMoveToTile(Mathf.RoundToInt(t.position.x + 1f), Mathf.RoundToInt(t.position.y)))  // TODO true 붙여서 추락사 가능하게
            {
                GetComponent<SpriteRenderer>().flipX = true;
                StartCoroutine(MoveAnimation(new Vector3(1f, 0f, 0f)));
            }
            else if(Input.GetKeyDown(KeyCode.UpArrow) &&
                gm.map.CanMoveToTile(Mathf.RoundToInt(t.position.x), Mathf.RoundToInt(t.position.y + 1f)))  // TODO true 붙여서 추락사 가능하게
            {
                StartCoroutine(MoveAnimation(new Vector3(0f, 1f, 0f)));
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) &&
                gm.map.CanMoveToTile(Mathf.RoundToInt(t.position.x), Mathf.RoundToInt(t.position.y - 1f)))  // TODO true 붙여서 추락사 가능하게
            {
                StartCoroutine(MoveAnimation(new Vector3(0f, -1f, 0f)));
            }
        }
	}

    IEnumerator MoveAnimation(Vector3 direction)
    {
        isMoving = true;
        int frame = 30;
        Vector3 origin = t.position;
        Vector3 destination = t.position + direction;

        // 애니메이션이 시작하기 전에 이동 판정 완료
        gm.map.SetObjectOnTile(null, origin);
        gm.map.SetObjectOnTile(gameObject, destination);
        for (int i = 0; i < frame; i++)
        {
            t.position = Vector3.Lerp(origin, destination, Mathf.Sqrt((float)i / frame));
            if (i < frame / 2)
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(1f, 0.4f, 0.8f, 1f), (float)i / frame * 2);
            else
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(1f, 0.4f, 0.8f, 1f), (float)(frame - i) / frame * 2);
            yield return null;
        }
        t.position = destination;
        isMoving = false;
        gm.NextTurn();
    }
}
