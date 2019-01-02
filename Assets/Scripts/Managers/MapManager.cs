using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {

    public static MapManager mm;

    public Dictionary<int, MapInfo> mapInfo;
    
    public Color townBackgroundColor;
    
    public Text mapText;

    void Awake()
    {
        mm = this;
        if (mapText == null)
        {
            GameObject g = GameObject.Find("MapText");
            if (g != null) mapText = g.GetComponent<Text>();
        }
    }

    /// <summary>
    /// 맵 이름으로 맵 정보를 찾습니다.
    /// 같은 이름의 맵이 여러 개이면 가장 먼저 찾은 맵 정보를 반환합니다.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public MapInfo FindMapInfo(string name)
    {
        foreach (KeyValuePair<int, MapInfo> mi in mapInfo)
        {
            if (mi.Value == null) continue;
            if (mi.Value.name.Equals(name))
            {
                return mi.Value;
            }
        }
        return null;
    }

    /// <summary>
    /// 맵 ID로 맵 정보를 찾습니다.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public MapInfo FindMapInfo(int id)
    {
        if (mapInfo.ContainsKey(id)) return mapInfo[id];
        return null;
    }

    
}

[System.Serializable]
public class MapInfo
{
    public string name;
    public List<GameObject> tilePrefab;
    public Color backgroundColor;
    public List<int> enemiesID = new List<int>();
    public List<int> interactablesID = new List<int>();
}