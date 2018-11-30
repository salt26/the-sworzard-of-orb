using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mover : MonoBehaviour {

    public bool isMoving;   // 이동 중에 true

    public virtual IEnumerator DamagedAnimation(int oldHealth, Slider healthBar = null)
    {
        yield return null;
    }

    public virtual void Death()
    {

    }
}
