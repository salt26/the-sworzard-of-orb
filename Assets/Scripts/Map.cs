using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour {

    [HideInInspector]
    public string mapName;
    //public Vector2Int size;
    private int[][] mapShape;

    private bool autoGeneration;
    private Vector2 bottomLeft = new Vector2(Mathf.Infinity, Mathf.Infinity);
    private Vector2 topRight = new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity);
    
    private List<MapTile> sceneTiles;
    private List<List<MapTile>> tiles;

    private bool isReady = false;

    public bool IsReady
    {
        get
        {
            return isReady;
        }
    }

    public Vector2 BottomLeft
    {
        get
        {
            return bottomLeft;
        }
    }

    public Vector2 TopRight
    {
        get
        {
            return topRight;
        }
    }

    /// <summary>
    /// 맵 크기를 반환합니다.
    /// 자동 생성된 맵이 아닌 경우 (0, 0)을 반환합니다.
    /// </summary>
    public Vector2Int MapSize
    {
        get
        {
            if (!IsReady)
                return new Vector2Int(0, 0);
            else if (!autoGeneration)
                return new Vector2Int((int)TopRight.x - (int)BottomLeft.x + 1, (int)TopRight.y - (int)BottomLeft.y + 1);
            else
                return new Vector2Int(mapShape[0].Length, mapShape.Length);
        }
    }

    public void Initialize(bool autoGen)
    {
        autoGeneration = autoGen;

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
            if (MapManager.mm.mapText != null)
                MapManager.mm.mapText.text = StringManager.sm.Translate(SceneManager.GetActiveScene().name);    // TODO
            if (Camera.main != null)
                Camera.main.backgroundColor = MapManager.mm.townBackgroundColor;
        }
        else
        {
            MapInfo mi = MapManager.mm.FindMapInfo(mapName, GameManager.gm.mapLevel[mapName]);
            mapShape = GenerateMapShape(new Vector2Int(mi.width, mi.height));
            tiles = GenerateMap(mapShape);
        }
        isReady = true;
    }

    private int[][] GenerateMapShape(Vector2Int size)
    {
        int[][] mapShape = new int[size.y][];
        for (int i = 0; i < size.y; i++)
        {
            mapShape[i] = new int[size.x];
        }

        int maxLoop = 100;
        for (int i = 0; i < maxLoop; i++)
        {
            mapShape = _MapGenerate(mapShape, 6);
            if (_IslandChecker(mapShape) && _CornerChecker(mapShape)) break;
            else
            {
                //Debug.Log("Retry map generating...");
                if (i == maxLoop - 1) Debug.LogError("Exceed max loop limit!");
            }
        }

        return mapShape;
    }

    private List<List<MapTile>> GenerateMap(int[][] mapShape)
    {
        if (mapShape == null || mapShape.Length <= 0 || mapShape[0].Length <= 0)
            return null;
        List<List<MapTile>> tiles = new List<List<MapTile>>();

        MapInfo mi = MapManager.mm.FindMapInfo(mapName, GameManager.gm.mapLevel[mapName]);
        if (mi == null)
        {
            Debug.LogWarning("Map doesn't exist!");
            return null;
        }

        if (MapManager.mm.mapText != null)
            MapManager.mm.mapText.text = StringManager.sm.Translate(mi.name);

        if (Camera.main != null)
            Camera.main.backgroundColor = mi.backgroundColor;

        bottomLeft = new Vector2(0f, 0f);
        topRight = new Vector2(mapShape[0].Length - 1, mapShape.Length - 1);

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

    #region 대륙 생성 및 무결성 검사 메서드
    private int[][] _MapGenerate(int[][] mapShape, int step)
    {
        int maxR = Mathf.Min(mapShape.Length / 2, mapShape[0].Length / 2);
        for (int i = 1; i < step + 1; i++)
        {
            float r = maxR * (0.5f * Mathf.Exp(-0.8f * Mathf.Sqrt(i)));
            for (int j = 0; j < i * i / 2; j++)
            {
                float x = Random.Range(0f, 1f) * (mapShape.Length - r * 2) + r;
                float y = Random.Range(0f, 1f) * (mapShape[0].Length - r * 2) + r;
                mapShape = _CircleGenerate(mapShape, r, x, y);
            }
        }
        return mapShape;
    }

    private int[][] _CircleGenerate(int[][] mapShape, float r, float x, float y)
    {
        for (int i = 0; i < mapShape.Length; i++)
        {
            for (int j = 0; j < mapShape[0].Length; j++)
            {
                if (Mathf.Sqrt(Mathf.Pow(i - x, 2) + Mathf.Pow(j - y, 2)) <= r)
                {
                    mapShape[i][j] = 1;
                }
            }
        }
        return mapShape;
    }
    
    /// <summary>
    /// 생성한 대륙이 걸어서 이동할 수 있는 하나의 섬인지 확인합니다.
    /// </summary>
    /// <param name="mapShape"></param>
    /// <returns></returns>
    private bool _IslandChecker(int[][] mapShape)
    {
        bool[][] check = new bool[mapShape.Length][];
        for (int i = 0; i < mapShape.Length; i++)
        {
            check[i] = new bool[mapShape[i].Length];
            for (int j = 0; j < mapShape[i].Length; j++)
            {
                check[i][j] = false;
            }
        }
        bool flag = false;
        for (int i = 0; i < mapShape.Length; i++)
        {
            for (int j = 0; j < mapShape[0].Length; j++)
            {
                if (mapShape[i][j] == 1)
                {
                    if (!check[i][j])
                    {
                        if (flag) return false;
                        check = _BFS(check, mapShape, j, i);
                        flag = true;
                    }
                }
            }
        }
        return true;
    }

    private bool[][] _BFS(bool[][] check, int[][] mapShape, int x, int y)
    {
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        q.Enqueue(new Vector2Int(x, y));
        check[y][x] = true;
        while (q.Count > 0)
        {
            Vector2Int v = q.Dequeue();
            if (v.y > 0 && mapShape[v.y - 1][v.x] == 1 && !check[v.y - 1][v.x])
            {
                check[v.y - 1][v.x] = true;
                q.Enqueue(new Vector2Int(v.x, v.y - 1));
            }
            if (v.y < mapShape.Length - 1 && mapShape[v.y + 1][v.x] == 1 && !check[v.y + 1][v.x])
            {
                check[v.y + 1][v.x] = true;
                q.Enqueue(new Vector2Int(v.x, v.y + 1));
            }
            if (v.x > 0 && mapShape[v.y][v.x - 1] == 1 && !check[v.y][v.x - 1])
            {
                check[v.y][v.x - 1] = true;
                q.Enqueue(new Vector2Int(v.x - 1, v.y));
            }
            if (v.x < mapShape[0].Length - 1 && mapShape[v.y][v.x + 1] == 1 && !check[v.y][v.x + 1])
            {
                check[v.y][v.x + 1] = true;
                q.Enqueue(new Vector2Int(v.x + 1, v.y));
            }
        }
        return check;
    }

    /// <summary>
    /// 생성한 대륙을 (4 * 4)등분했을 때,
    /// 모서리에 해당하는 네 영역에 모두 타일이 하나 이상 존재하는지 확인합니다.
    /// </summary>
    /// <param name="mapShape"></param>
    /// <returns></returns>
    private bool _CornerChecker(int[][] mapShape)
    {
        int checkCount = 0;
        for (int i = mapShape.Length * 3 / 4; i < mapShape.Length; i++)
        {
            for (int j = mapShape[0].Length * 3 / 4; j < mapShape[0].Length; j++)
            {
                if (mapShape[i][j] == 1)
                {
                    checkCount++;
                    break;
                }
            }
            if (checkCount == 1) break;
        }
        for (int i = mapShape.Length * 3 / 4; i < mapShape.Length; i++)
        {
            for (int j = 0; j < mapShape[0].Length / 4; j++)
            {
                if (mapShape[i][j] == 0)
                {
                    checkCount++;
                    break;
                }
            }
            if (checkCount == 2) break;
        }
        for (int i = 0; i < mapShape.Length / 4; i++)
        {
            for (int j = 0; j < mapShape[0].Length / 4; j++)
            {
                if (mapShape[i][j] == 0)
                {
                    checkCount++;
                    break;
                }
            }
            if (checkCount == 3) break;
        }
        for (int i = 0; i < mapShape.Length / 4; i++)
        {
            for (int j = mapShape[0].Length * 3 / 4; j < mapShape[0].Length; j++)
            {
                if (mapShape[i][j] == 0)
                {
                    checkCount++;
                    break;
                }
            }
            if (checkCount == 4) break;
        }
        if (checkCount == 4) return true;
        else return false;
    }
    #endregion

    /// <summary>
    /// 대륙을 (4 * 4)등분할 때, 모서리에 해당하는 영역에 있는 타일 하나의 위치를 반환합니다.
    /// 자동 생성된 맵에서만 사용할 수 있습니다.
    /// </summary>
    /// <param name="quadrant">사분면 (1 이상 4 이하)</param>
    /// <returns></returns>
    public Vector2Int GetACornerPosition(int quadrant)
    {
        if (!IsReady || !autoGeneration || quadrant < 1 || quadrant > 4)
            return new Vector2Int(0, 0);

        Vector2Int v = new Vector2Int(0, 0);
        int maxLoop = 100;
        bool isMaxLoop = false;

        // Stochastic한 방법으로 고르기
        switch (quadrant)
        {
            case 1:
                for (int i = 0; i < maxLoop; i++)
                {
                    v = new Vector2Int(Random.Range(mapShape[0].Length * 3 / 4, mapShape[0].Length),
                        Random.Range(mapShape.Length * 3 / 4, mapShape.Length));
                    if (GetTile(v.x, v.y) != null) break;
                    if (i == maxLoop - 1) isMaxLoop = true;
                }
                break;
            case 2:
                for (int i = 0; i < maxLoop; i++)
                {
                    v = new Vector2Int(Random.Range(0, mapShape[0].Length / 4),
                        Random.Range(mapShape.Length * 3 / 4, mapShape.Length));
                    if (GetTile(v.x, v.y) != null) break;
                    if (i == maxLoop - 1) isMaxLoop = true;
                }
                break;
            case 3:
                for (int i = 0; i < maxLoop; i++)
                {
                    v = new Vector2Int(Random.Range(0, mapShape[0].Length / 4),
                        Random.Range(0, mapShape.Length / 4));
                    if (GetTile(v.x, v.y) != null) break;
                    if (i == maxLoop - 1) isMaxLoop = true;
                }
                break;
            default:
                for (int i = 0; i < maxLoop; i++)
                {
                    v = new Vector2Int(Random.Range(mapShape[0].Length * 3 / 4, mapShape[0].Length),
                        Random.Range(0, mapShape.Length / 4));
                    if (GetTile(v.x, v.y) != null) break;
                    if (i == maxLoop - 1) isMaxLoop = true;
                }
                break;
        }

        // Stochastic한 방법이 실패한 경우
        if (isMaxLoop)
        {
            Debug.LogWarning("Exceed max loop limit!");
            v = new Vector2Int(0, 0);

            // Deterministic한 방법으로 고르기
            switch (quadrant)
            {
                case 1:
                    for (int i = mapShape.Length * 3 / 4; i < mapShape.Length; i++)
                    {
                        for (int j = mapShape[0].Length * 3 / 4; j < mapShape[0].Length; j++)
                        {
                            if (GetTile(j, i) != null)
                            {
                                v = new Vector2Int(j, i);
                                break;
                            }
                        }
                    }
                    break;
                case 2:
                    for (int i = mapShape.Length * 3 / 4; i < mapShape.Length; i++)
                    {
                        for (int j = 0; j < mapShape[0].Length / 4; j++)
                        {
                            if (GetTile(j, i) != null)
                            {
                                v = new Vector2Int(j, i);
                                break;
                            }
                        }
                    }
                    break;
                case 3:
                    for (int i = 0; i < mapShape.Length / 4; i++)
                    {
                        for (int j = 0; j < mapShape[0].Length / 4; j++)
                        {
                            if (GetTile(j, i) != null)
                            {
                                v = new Vector2Int(j, i);
                                break;
                            }
                        }
                    }
                    break;
                default:
                    for (int i = 0; i < mapShape.Length / 4; i++)
                    {
                        for (int j = mapShape[0].Length * 3 / 4; j < mapShape[0].Length; j++)
                        {
                            if (GetTile(j, i) != null)
                            {
                                v = new Vector2Int(j, i);
                                break;
                            }
                        }
                    }
                    break;
            }
        }

        return v;
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
        else
        {
            if (x < (int)bottomLeft.x || y < (int)bottomLeft.y || x > (int)topRight.x || y > (int)topRight.y)
                return null;
            else
                return tiles[y - (int)bottomLeft.y][x - (int)bottomLeft.x];
        }
    }

    /// <summary>
    /// (x, y) 좌표에 위치한 MapTile의 타입을 반환합니다.
    /// 만약 타일이 존재하지 않으면 1을 반환합니다.
    /// 0이면 밟을 수 있는 타일, 1이면 밟을 수 없는 타일, 2이면 밟을 때 추락하는 타일입니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
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

    /// <summary>
    /// position의 (x, y) 좌표와 가장 가까운 곳에 위치한 MapTile의 타입을 반환합니다.
    /// 만약 타일이 존재하지 않으면 1을 반환합니다.
    /// 0이면 밟을 수 있는 타일, 1이면 밟을 수 없는 타일, 2이면 밟을 때 추락하는 타일입니다.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
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
    /// 아이템 id를 갖는 아이템 오브젝트를 생성하고 true를 반환합니다.
    /// 만약 MapTile이 존재하지 않거나 밟을 수 없는 타일이면 false를 반환합니다.
    /// ItemManager에 등록된 아이템이 아닌 경우에도 false를 반환합니다.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool AddItemOnTile(int itemID, int x, int y)
    {
        MapTile t = GetTile(x, y);
        if (t == null || t.Type != 0 ||
            ItemManager.im.FindItemInfo(itemID) == null)
        {
            return false;
        }
        GameObject g = ItemManager.im.CreateItem(itemID, t.GetComponent<Transform>().position + new Vector3(0f, 0f, -0.5f));
        t.Items.Add(g.GetComponent<Item>());
        return true;
    }

    /// <summary>
    /// position의 (x, y) 좌표와 가장 가까운 곳에 위치한 MapTile 위에
    /// 아이템 id를 갖는 아이템 오브젝트를 생성하고 true를 반환합니다.
    /// 만약 MapTile이 존재하지 않으면 false를 반환합니다.
    /// ItemManager에 등록된 아이템이 아닌 경우에도 false를 반환합니다.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool AddItemOnTile(int itemID, Vector3 position)
    {
        return AddItemOnTile(itemID, Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
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
        bool b;
        MapTile t = GetTile(x, y);
        if (t == null || t.Type != 0)
        {
            return false;
        }
        for (int i = t.Items.Count - 1; i >= 0; i--)
        {
            if (t.Items[i] == null) continue;
            if (t.Items[i].name.Equals("Gold"))
            {
                inventory.Gold += ((Gold)t.Items[i]).Quantity;
                Destroy(t.Items[i].gameObject);
                t.Items[i] = null;
            }
            else
            {
                // TODO
                b = inventory.AddItem(t.Items[i].name);
                if (b)
                {
                    Destroy(t.Items[i].gameObject);
                    t.Items[i] = null;
                }
            }
        }
        List<Item> l = new List<Item>();
        for (int i = 0; i < t.Items.Count; i++)
        {
            if (t.Items[i] != null) l.Add(t.Items[i]);
        }
        t.Items = l;
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
