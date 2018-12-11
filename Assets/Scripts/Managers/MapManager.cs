using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {
    
    public List<MapInfo> mapInfo;
    
    [SerializeField]
    private string mapName;
    public Vector2 size;

    private List<List<MapTile>> tiles = new List<List<MapTile>>();

    [SerializeField]
    private Text mapText;

    // Use this for initialization
    void Awake () {
        MapInfo mi = FindMapInfo(mapName);
        if (mi == null)
        {
            Debug.LogWarning("Map doesn't exist!");
            return;
        }
        mapText.text = mi.name;
		for (int i = 0; i < size.y; i++)
        {
            tiles.Add(new List<MapTile>());
            for (int j = 0; j < size.x; j++)
            {
                MapTile t = Instantiate(mi.tilePrefab[Random.Range(0, mi.tilePrefab.Count)], new Vector3(j, i, 0f), Quaternion.identity, GetComponent<Transform>()).GetComponent<MapTile>();
                t.Entity = null;
                tiles[i].Add(t);
            }
        }
        Camera.main.backgroundColor = mi.backgroundColor;
	}
	
	// Update is called once per frame
	void Update () {
		
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

    /// <summary>
    /// (x, y) 좌표에 위치한 MapTile 위에
    /// item 오브젝트를 생성하고 true를 반환합니다.
    /// 만약 MapTile이 존재하지 않거나 밟을 수 없는 타일이면 false를 반환합니다.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool AddItemOnTile(GameObject item, int x, int y)
    {
        MapTile t = GetTile(x, y);
        if (t == null || t.Type != 0)
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
                inventory.items.Add(i.name);
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