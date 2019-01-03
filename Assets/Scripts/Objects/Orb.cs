using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : Item {

    public int level;       // 오브 레벨

    public override void Initialize(string itemName)
    {
        ItemInfo ii = ItemManager.im.FindItemInfo(itemName);
        if (ii == null)
        {
            Debug.LogWarning("Failed to initialize item.");
            return;
        }
        name = itemName;
        level = ii.level;
        GetComponent<SpriteRenderer>().sprite = ItemManager.im.GetItemSprite(itemName);
    }
}
