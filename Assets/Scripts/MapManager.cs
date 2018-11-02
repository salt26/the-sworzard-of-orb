using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    public GameObject tile;
    public Vector2 size;

    private List<List<MapTile>> tiles = new List<List<MapTile>>();

    // Use this for initialization
    void Awake () {
		for (int i = 0; i < size.y; i++)
        {
            tiles.Add(new List<MapTile>());
            for (int j = 0; j < size.x; j++)
            {
                MapTile t = Instantiate(tile, new Vector3(j, i, 0f), Quaternion.identity, GetComponent<Transform>()).GetComponent<MapTile>();
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
            return 2;  // 밟으면 추락
        }
        else
            return GetTile(x, y).Type;
    }
}
