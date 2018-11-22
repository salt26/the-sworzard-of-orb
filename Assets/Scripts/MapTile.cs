using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour {

    [SerializeField]
    private int type;   // 0이면 밟을 수 있는 타일, 1이면 밟을 수 없는 타일, 2이면 밟을 때 추락하는 타일
    private Entity entity;  // 이 타일을 밟고 있는 개체(Character 또는 Interactable)
    private Item item;      // 이 타일 위에 놓인 아이템
    // TODO 타일의 스프라이트도 무엇인지 가지고 있어야 하나?

    public int Type
    {
        get
        {
            return type;
        }
    }

    public Entity Entity
    {
        get
        {
            return entity;
        }
        set
        {
            entity = value;
        }
    }

    public Item Item
    {
        get
        {
            return item;
        }
        set
        {
            item = value;
        }
    }
}
