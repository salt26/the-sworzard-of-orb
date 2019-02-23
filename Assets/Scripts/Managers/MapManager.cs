using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    /// 맵 이름과 레벨로 맵 정보를 찾습니다.
    /// 맵 이름이 존재하지만 레벨이 존재하지 않는 경우에는
    /// 인자로 주어진 레벨보다 작은, 가장 높은 레벨의 맵 정보를 반환합니다.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public MapInfo FindMapInfo(string name, int level)
    {
        if (level < 1) return null;
        SortedList<int, MapInfo> l = new SortedList<int, MapInfo>();
        foreach (KeyValuePair<int, MapInfo> mi in mapInfo)
        {
            if (mi.Value == null) continue;
            if (mi.Value.name.Equals(name))
            {
                l.Add(mi.Value.level, mi.Value);
            }
        }
        for (int i = level; i >= 1; i--)
        {
            if (l.ContainsKey(i))
            {
                return l[i];
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

    public void RefreshMapText()
    {
        string mapName;
        if (GameManager.gm.map == null || GameManager.gm.map.mapName == null || GameManager.gm.map.mapName == "")
        {
            mapName = StringManager.sm.Translate(SceneManager.GetActiveScene().name);
        }
        else
        {
            mapName = GameManager.gm.map.mapName;
        }
        if (mapText != null)
            mapText.text = StringManager.sm.Translate(mapName);
    }
}

[System.Serializable]
public class MapInfo
{
    public string name;
    public int level;
    public int width;
    public int height;
    public List<GameObject> tilePrefab;
    public Color backgroundColor;
    public string backgroundMusic;
    public List<int> enemiesID = new List<int>();
    public List<int> interactablesID = new List<int>();
}