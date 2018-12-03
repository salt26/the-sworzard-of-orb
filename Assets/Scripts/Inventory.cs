using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    // TODO 나중에 private으로 바꾸기
    public List<Item> items;
    
    private int gold = 0;

    [SerializeField]
    private Text goldText;

    public int Gold
    {
        get
        {
            return gold;
        }
        set
        {
            gold = value;
            goldText.text = gold.ToString();
        }
    }
}
