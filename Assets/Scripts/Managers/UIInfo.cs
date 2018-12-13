using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInfo : MonoBehaviour {

    [Header("Player")]
    public Slider healthBar;
    public StatusUI playerStatusUI;
    public Text goldText;

    [Header("GM")]
    public Image turnMark;
    public Image weaponMark;
    public GameObject restartText;
}
