using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIInfo : MonoBehaviour {

    [Header("Player")]
    public Slider healthBar;
    public StatusUI playerStatusUI;
    public Text goldText;
    public List<Button> itemButtons;

    [Header("GM")]
    public Image turnMark;
    public Image weaponMark;
    public GameObject restartText;
    public GameObject loadingPanel;
    public GameObject enemyStatusUIGroup;

    [Header("Enemy")]
    public StatusUI enemyStatusUI;

    [Header("ETC")]
    public GameObject menuUI;
    public GameObject altarPanel;

    public void MenuUI()
    {
        menuUI.SetActive(!menuUI.activeInHierarchy);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
