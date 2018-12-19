using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : Item {

    public int level;       // 오브 레벨
    public Element stat;

    public override bool Use()
    {
        // TODO 지금은 아무 것도 안 함
        return false;
    }
}
