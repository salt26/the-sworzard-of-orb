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
    public DamagedScreen DamagedPanel;

    [Header("GM")]
    public GameObject monsterNumberMark;
    public Text monsterNumberText;
    public Image weaponMark;
    public GameObject restartText;
    public GameObject loadingPanel;
    public GameObject enemyStatusUIGroup;

    [Header("Enemy")]
    public StatusUI enemyStatusUI;

    [Header("ETC")]
    public List<GameObject> menuUI;
    public GameObject altarPanel;
    public GameObject shopPanel;
    public Text menuText;
    public Text languageText;
    public Text goodbyeText;

    public void MenuUI()
    {
        foreach (GameObject g in menuUI)
        {
            g.SetActive(!g.activeInHierarchy);
        }
    }

    public void ToggleLanguage()
    {
        StringManager.Language l = StringManager.sm.LanguageSetting;
        switch (l)
        {
            case StringManager.Language.English:
                StringManager.sm.LanguageSetting = StringManager.Language.Korean;
                languageText.text = "English";
                break;
            case StringManager.Language.Korean:
                StringManager.sm.LanguageSetting = StringManager.Language.English;
                languageText.text = "한국어";
                break;
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void RefreshMenuTexts()
    {
        menuText.text = StringManager.sm.Translate("Menu");
        goodbyeText.text = StringManager.sm.Translate("Goodbye~");
    }
}
