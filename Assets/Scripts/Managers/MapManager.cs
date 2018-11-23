﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    public List<GameObject> tilePrefab;
    public Vector2 size;

    private List<List<MapTile>> tiles = new List<List<MapTile>>();

    // Use this for initialization
    void Awake () {
		for (int i = 0; i < size.y; i++)
        {
            tiles.Add(new List<MapTile>());
            for (int j = 0; j < size.x; j++)
            {
                MapTile t = Instantiate(tilePrefab[Random.Range(0, 20) / 17], new Vector3(j, i, 0f), Quaternion.identity, GetComponent<Transform>()).GetComponent<MapTile>();
                t.Entity = null;
                tiles[i].Add(t);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// (x, y) 좌표에 위치한 MapTile을 반환합니다.
    /// 만약 존재하지 않는 경우 null을 반환합니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private MapTile GetTile(int x, int y)
    {
        if (x < 0 || y < 0 || y >= tiles.Count || x >= tiles[y].Count)
            return null;
        else
            return tiles[y][x];
    }

    public int GetTypeOfTile(int x, int y)
    {
        if (GetTile(x, y) == null)
        {
            //return 2;  // 밟으면 추락
            return 1;   // TODO 위의 코드로 교체할 것!
        }
        else
            return GetTile(x, y).Type;
    }

    public int GetTypeOfTile(Vector3 position)
    {
        return GetTypeOfTile(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }

    /// <summary>
    /// (x, y) 좌표에 위치한 MapTile 위에 있는 개체를 반환합니다.
    /// 만약 MapTile이나 개체가 존재하지 않으면 null을 반환합니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Entity GetEntityOnTile(int x, int y)
    {
        MapTile t = GetTile(x, y);
        if (t == null)
        {
            return null;
        }
        return t.Entity;
    }

    /// <summary>
    /// position의 (x, y) 좌표와 가장 가까운 곳에 위치한 MapTile 위에 있는 개체를 반환합니다.
    /// 만약 MapTile이나 개체가 존재하지 않으면 null을 반환합니다.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Entity GetEntityOnTile(Vector3 position)
    {
        return GetEntityOnTile(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }

    /// <summary>
    /// (x, y) 좌표에 위치한 MapTile 위에 있는 개체를
    /// obj로 설정하고 true를 반환합니다.
    /// 만약 MapTile이 존재하지 않거나 밟을 수 없는 타일이면 false를 반환합니다.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool SetEntityOnTile(Entity entity, int x, int y)
    {
        MapTile t = GetTile(x, y);
        if (t == null || t.Type != 0)
        {
            return false;
        }
        t.Entity = entity;
        return true;
    }

    /// <summary>
    /// position의 (x, y) 좌표와 가장 가까운 곳에 위치한 MapTile 위에 있는 개체를
    /// obj로 설정하고 true를 반환합니다.
    /// 만약 MapTile이 존재하지 않으면 false를 반환합니다.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool SetEntityOnTile(Entity entity, Vector3 position)
    {
        return SetEntityOnTile(entity, Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }

    /// <summary>
    /// (x, y) 좌표의 MapTile로 이동할 수 있으면 true를,
    /// 이동할 수 없으면 false를 반환합니다.
    /// 추락사를 허용하는 경우 canFall에 true를 넣어주세요.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="canFall"></param>
    /// <returns></returns>
    public bool CanMoveToTile(int x, int y, bool canFall = false)
    {
        return (canFall && GetTypeOfTile(x, y) == 2) ||
            (GetTypeOfTile(x, y) == 0 && GetEntityOnTile(x, y) == null);
    }

    /// <summary>
    /// destination의 (x, y) 좌표와 가장 가까운 곳에 위치한
    /// MapTile로 이동할 수 있으면 true를,
    /// 이동할 수 없으면 false를 반환합니다.
    /// 추락사를 허용하는 경우 canFall에 true를 넣어주세요.
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="canFall"></param>
    /// <returns></returns>
    public bool CanMoveToTile(Vector3 destination, bool canFall = false)
    {
        return CanMoveToTile(Mathf.RoundToInt(destination.x), Mathf.RoundToInt(destination.y), canFall);
    }
}