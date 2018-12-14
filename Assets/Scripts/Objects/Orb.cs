using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : Item {

    public int level;
    public Element stat;
    public delegate void Effect();
    public Effect effect;
    public string effectName;

}
