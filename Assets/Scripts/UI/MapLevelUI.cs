using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLevelUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
        string map = GetComponentInParent<Portal>().mapName;
        if (GameManager.gm != null)
        {
            if (GameManager.gm.mapLevel.ContainsKey(map))
                GetComponent<Text>().text = (GameManager.gm.mapLevel[map] + 1).ToString();
            else
                GetComponent<Text>().text = "1";
        }
	}
}
