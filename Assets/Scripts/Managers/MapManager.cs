using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {

    public bool autoGeneration;
    public List<MapInfo> mapInfo;
    
    public string mapName;
    //public Vector2Int size;
    private int[][] mapShape;

    public int mapIndex;
    public Color backgroundColor;

    public List<MapTile> sceneTiles;
    private List<List<MapTile>> tiles;
    
    private Text mapText;
    private Vector2 bottomLeft = new Vector2(Mathf.Infinity, Mathf.Infinity);
    private Vector2 topRight = new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity);
    private bool isReady = false;

    public bool IsReady
    {
        get
        {
            return isReady;
        }
    }
    
    public void Initialize () {

        if (mapText == null) {
            GameObject g = GameObject.Find("MapText");
            if (g != null) mapText = g.GetComponent<Text>();
        }

        if (!autoGeneration)
        {
            sceneTiles = new List<MapTile>();
            foreach (MapTile t in GetComponentsInChildren<MapTile>())
            {
                if (t.gameObject.activeInHierarchy)
                    sceneTiles.Add(t);
            }
            foreach (MapTile t in sceneTiles)
            {
                Vector3 p = t.GetComponent<Transform>().position;
                t.GetComponent<Transform>().position = new Vector3((int)(p.x), (int)(p.y), p.z);
                p = t.GetComponent<Transform>().position;
                if (p.x < bottomLeft.x) bottomLeft.x = p.x;
                if (p.y < bottomLeft.y) bottomLeft.y = p.y;
                if (p.x > topRight.x) topRight.x = p.x;
                if (p.y > topRight.y) topRight.y = p.y;
            }
            tiles = new List<List<MapTile>>();
            if (sceneTiles.Count > 0)
            {
                for (int i = 0; i < (int)topRight.y - (int)bottomLeft.y + 1; i++)
                {
                    tiles.Add(new List<MapTile>());
                    for (int j = 0; j < (int)topRight.x - (int)bottomLeft.x + 1; j++)
                    {
                        tiles[i].Add(null);
                    }
                }
            }
            foreach (MapTile t in sceneTiles)
            {
                Vector3 p = t.GetComponent<Transform>().position;
                tiles[(int)p.y - (int)bottomLeft.y][(int)p.x - (int)bottomLeft.x] = t;
            }
            if (mapText != null)
                mapText.text = mapName;
            if (Camera.main != null)
                Camera.main.backgroundColor = backgroundColor;
        }
        else
        {
            mapShape = GenerateMapShape(new Vector2Int(11, 9));
            tiles = GenerateMap(mapShape);
        }
        isReady = true;
	}

    private List<List<MapTile>> GenerateMap(int[][] mapShape)
    {
        List<List<MapTile>> tiles = new List<List<MapTile>>();

        MapInfo mi = FindMapInfo(mapName);
        if (mi == null)
        {
            Debug.LogWarning("Map doesn't exist!");
            return null;
        }

        if (mapText != null)
            mapText.text = mi.name;

        if (Camera.main != null)
            Camera.main.backgroundColor = mi.backgroundColor;

        for (int i = 0; i < mapShape.Length; i++)
        {
            // i는 y좌표
            tiles.Add(new List<MapTile>());
            for (int j = 0; j < mapShape[i].Length; j++)
            {
                // j는 x좌표
                if (mapShape[i][j] == 0)
                {
                    // 0이면 없는 타일
                    tiles[i].Add(null);
                }
                else
                {
                    // 1이면 밟을 수 있는 타일
                    MapTile t = Instantiate(mi.tilePrefab[Random.Range(0, mi.tilePrefab.Count)], new Vector3(j, i, 0f), Quaternion.identity, GetComponent<Transform>()).GetComponent<MapTile>();
                    t.Entity = null;
                    tiles[i].Add(t);
                }
            }
        }
        return tiles;
    }

    private int[][] GenerateMapShape(Vector2Int size)
    {
        // TODO 지금은 하드코딩되어 있음
        // 나중에 size를 받아, 그 안을 걸어다닐 수 있도록 0과 1로 랜덤하게 채워야 함
        int[][] mapShape = new int[size.y][];
        for (int i = 0; i < size.y; i++)
        {
            mapShape[i] = new int[size.x];
        }

        if (mapName.Equals("Red"))
        {
            for (int i = 0; i < size.y; i++)
            {
                for (int j = 0; j < size.x; j++)
                {
                    mapShape[i][j] = 1;
                }
            }
            for (int i = size.y / 2 - 2; i <= size.y / 2 + 2; i++)
            {
                for (int j = size.x / 2 - 2; j <= size.x / 2 + 2; j++)
                {
                    if (i == size.y / 2 || j == size.x / 2)
                        mapShape[i][j] = 0;
                }
            }
        }
        else if (mapName.Equals("Blue"))
        {
            for (int i = 0; i < size.y; i++)
            {
                for (int j = 0; j < size.x; j++)
                {
                    if (i % 3 == 1 && j % 4 == 1)
                    {
                        mapShape[i][j] = 0;
                    }
                    else
                        mapShape[i][j] = 1;
                }
            }
        }
        else if (mapName.Equals("Green"))
        {
            for (int i = 0; i < size.y; i++)
            {
                for (int j = 0; j < size.x; j++)
                {
                    if (i + j >= 2 && i + j < size.x + size.y - 3 && i - j <= size.y - 3 && j - i <= size.x - 3)
                    {
                        mapShape[i][j] = 1;
                    }
                    else mapShape[i][j] = 0;
                }
            }
        }
        else
        {
            for (int i = 0; i < size.y; i++)
            {
                for (int j = 0; j < size.x; j++)
                {
                    mapShape[i][j] = 1;
                }
            }
        }
        return mapShape;
    }

    public MapInfo FindMapInfo(string name)
    {
        foreach (MapInfo mi in mapInfo)
        {
            if (mi.name.Equals(name))
            {
                return mi;
            }
        }
        return null;
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
        if (!IsReady)
        {
            Debug.LogWarning("Map is not initialized!");
            return null;
        }
        else if (autoGeneration)
        {
            if (x < 0 || y < 0 || y >= tiles.Count || x >= tiles[y].Count)
                return null;
            else
                return tiles[y][x];
        }
        else
        {
            if (x < (int)bottomLeft.x || y < (int)bottomLeft.y || x > (int)topRight.x || y > (int)topRight.y)
                return null;
            else
                return tiles[y - (int)bottomLeft.y][x - (int)bottomLeft.x];
        }
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

    /// <summary>
    /// (x, y) 좌표에 위치한 MapTile 위에
    /// item 오브젝트를 생성하고 true를 반환합니다.
    /// 만약 MapTile이 존재하지 않거나 밟을 수 없는 타일이면 false를 반환합니다.
    /// item이 null이거나 ItemManager에 등록된 아이템이 아닌 경우에도 false를 반환합니다.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool AddItemOnTile(GameObject item, int x, int y)
    {
        MapTile t = GetTile(x, y);
        if (t == null || t.Type != 0 || item == null || item.GetComponent<Item>() == null ||
            !ItemManager.im.IsRegisteredItem(item.GetComponent<Item>().name))
        {
            return false;
        }
        GameObject g = Instantiate(item, t.GetComponent<Transform>().position + new Vector3(0f, 0f, -0.5f), Quaternion.identity);
        t.Items.Add(g.GetComponent<Item>());
        return true;
    }

    /// <summary>
    /// position의 (x, y) 좌표와 가장 가까운 곳에 위치한 MapTile 위에
    /// item 오브젝트를 생성하고 true를 반환합니다.
    /// 만약 MapTile이 존재하지 않으면 false를 반환합니다.
    /// item이 null이거나 ItemManager에 등록된 아이템이 아닌 경우에도 false를 반환합니다.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool AddItemOnTile(GameObject item, Vector3 position)
    {
        return AddItemOnTile(item, Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }

    /// <summary>
    /// (x, y) 좌표에 위치한 MapTile 위에
    /// gold만큼의 화폐 오브젝트를 생성하고 true를 반환합니다.
    /// 만약 gold가 0 이하이면 화폐를 생성하지 않고 true를 반환합니다.
    /// 만약 MapTile이 존재하지 않거나 밟을 수 없는 타일이면 false를 반환합니다.
    /// </summary>
    /// <param name="gold"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool AddGoldOnTile(int gold, int x, int y)
    {
        MapTile t = GetTile(x, y);
        if (t == null || t.Type != 0)
        {
            return false;
        }
        if (gold <= 0) return true;
        GameObject g = Instantiate(ItemManager.im.Gold, t.GetComponent<Transform>().position + new Vector3(0f, 0f, -0.4f), Quaternion.identity);
        g.GetComponent<Gold>().Quantity = gold;
        t.Items.Add(g.GetComponent<Item>());
        return true;
    }

    /// <summary>
    /// position의 (x, y) 좌표와 가장 가까운 곳에 위치한 MapTile 위에
    /// gold만큼의 화폐 오브젝트를 생성하고 true를 반환합니다.
    /// 만약 gold가 0 이하이면 화폐를 생성하지 않고 true를 반환합니다.
    /// 만약 MapTile이 존재하지 않으면 false를 반환합니다.
    /// </summary>
    /// <param name="gold"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool AddGoldOnTile(int gold, Vector3 position)
    {
        return AddGoldOnTile(gold, Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }

    /// <summary>
    /// (x, y) 좌표에 위치한 MapTile 위에 놓인
    /// 모든 아이템들을 inventory에 넣고 제거한 뒤 true를 반환합니다.
    /// 만약 아이템이 떨어져 있지 않으면 true를 반환합니다.
    /// 만약 MapTile이 존재하지 않거나 밟을 수 없는 타일이면 false를 반환합니다.
    /// </summary>
    /// <param name="inventory"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool PickUpItemsOnTile(Inventory inventory, int x, int y)
    {
        MapTile t = GetTile(x, y);
        if (t == null || t.Type != 0)
        {
            return false;
        }
        foreach (Item i in t.Items)
        {
            if (i.name.Equals("Gold"))
            {
                inventory.Gold += ((Gold)i).Quantity;
            }
            else
            {
                // TODO
                inventory.AddItem(i.name);
            }
        }
        for (int i = t.Items.Count - 1; i >= 0; i--)
        {
            Destroy(t.Items[i].gameObject);
        }
        t.Items = new List<Item>();
        return true;
    }

    /// <summary>
    /// position의 (x, y) 좌표와 가장 가까운 곳에 위치한 MapTile 위에 놓인
    /// 모든 아이템들을 inventory에 넣고 제거한 뒤 true를 반환합니다.
    /// 만약 아이템이 떨어져 있지 않으면 true를 반환합니다.
    /// 만약 MapTile이 존재하지 않거나 밟을 수 없는 타일이면 false를 반환합니다.
    /// </summary>
    /// <param name="inventory"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool PickUpItemsOnTile(Inventory inventory, Vector3 position)
    {
        return PickUpItemsOnTile(inventory, Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }
}

[System.Serializable]
public class MapInfo
{
    public string name;
    public List<GameObject> tilePrefab;
    public Color backgroundColor;
}