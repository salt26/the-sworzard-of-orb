using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mover : MonoBehaviour {

    protected bool isMoving;   // 이동 중에 true

    protected Character c;

    public bool IsMoving
    {
        get
        {
            return isMoving;
        }
    }

    public virtual IEnumerator DamagedAnimation(int oldHealth, Slider healthBar = null)
    {
        yield return null;
    }

    public virtual void Death()
    {

    }
}
