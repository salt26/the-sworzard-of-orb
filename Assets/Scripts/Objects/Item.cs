using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    //public enum Type { Gold, Orb, Consumable };
    //public Type type;

    public new string name; // 아이템 이름(Primary key)

    /*
    public bool Use()
    {
        ItemInfo ii = ItemManager.im.FindItemInfo(name);
        if (ii != null)
            return ii.Use();
        else return false;
    }
    */

    public virtual void Initialize(string itemName)
    {
        ItemInfo ii = ItemManager.im.FindItemInfo(itemName);
        if (ii == null)
        {
            Debug.LogWarning("Failed to initialize item.");
            return;
        }
        name = itemName;
        GetComponent<SpriteRenderer>().sprite = ItemManager.im.GetItemSprite(itemName);
    }
}
