using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    //public enum Type { Gold, Orb, Consumable };
    //public Type type;

    public new string name; // 아이템 이름(Primary key)

    public Sprite inventorySprite;  // 인벤토리에서 보여질 이미지

    [SerializeField]
    private string effectName;
    [SerializeField]
    private int effectParam;

    public virtual bool Use()
    {
        //Debug.Log("Use item: " + name);
        return ItemManager.im.InvokeEffect(effectName, effectParam);
    }
}
