using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour {

    [SerializeField]
    private int type;   // 0이면 밟을 수 있는 타일, 1이면 밟을 수 없는 타일, 2이면 밟을 때 추락하는 타일
    // TODO 타일의 스프라이트도 무엇인지 가지고 있어야 하나?

    public int Type
    {
        get
        {
            return type;
        }
    }
}
