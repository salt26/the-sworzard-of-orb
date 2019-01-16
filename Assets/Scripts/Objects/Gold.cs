using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gold : Item {

    [SerializeField]
    private int quantity;

    public int Quantity
    {
        get
        {
            return quantity;
        }
        set
        {
            if (value >= 2)
            {
                GetComponent<SpriteRenderer>().sprite = Resources.Load("NewGolds2", typeof(Sprite)) as Sprite;
                quantity = value;
            }
            else if (value == 1)
            {
                GetComponent<SpriteRenderer>().sprite = Resources.Load("NewGolds", typeof(Sprite)) as Sprite;
                quantity = value;
            }
        }
    }

    void Start()
    {
        Quantity = quantity;
    }
}
