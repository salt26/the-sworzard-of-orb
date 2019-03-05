using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringManager : MonoBehaviour {
    
    public static StringManager sm;

    /// <summary>
    /// 지원되는 언어 종류
    /// </summary>
    // 주의: Translation.txt에서의 언어 순서와 일치해야 함
    public enum Language { English = 0, Korean = 1 };
    
    private Language languageSetting = Language.Korean;
    public delegate void AfterSetLanguage();
    private AfterSetLanguage afterSetLanguage;
    private bool ready = false;     // Start가 호출된 후에 afterSetLanguage를 호출할 수 있음
    
    /// <summary>
    /// Key는 영어로 된 원래 문자열, Value는 언어 번호를 인덱스로 하는 번역 문자열 목록입니다.
    /// 언어 번호가 0이면 영어, 1이면 한국어입니다.
    /// </summary>
    public Dictionary<string, List<string>> dictionary;

    /// <summary>
    /// 설정된 언어
    /// </summary>
    public Language LanguageSetting
    {
        get
        {
            return languageSetting;
        }
        set
        {
            languageSetting = value;
            RefreshTexts();
        }
    }
    
    void Awake()
    {
        if (sm != null)
        {
            return;
        }
        sm = this;
    }

    void Start()
    {
        afterSetLanguage += GameManager.gm.Canvas.GetComponent<UIInfo>().playerStatusUI.RefreshText;
        afterSetLanguage += GameManager.gm.Canvas.GetComponent<UIInfo>().enemyStatusUI.RefreshText;
        afterSetLanguage += GameManager.gm.Canvas.GetComponent<UIInfo>().RefreshMenuTexts;
        afterSetLanguage += GameManager.gm.Canvas.GetComponent<UIInfo>().shopPanel.GetComponent<ShopUI>().RefreshRepurchaseText;
        afterSetLanguage += GameManager.gm.Canvas.GetComponent<UIInfo>().messagePanel.GetComponent<MessageUI>().RefreshMessageText;
        afterSetLanguage += MapManager.mm.RefreshMapText;
        afterSetLanguage += GameManager.gm.RefreshRestartText;
        ready = true;
        afterSetLanguage();
    }

    /// <summary>
    /// original 문자열에서 '_'(Underbar)를 모두 ' '(Space)로 바꾼 문자열을 반환합니다.
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    public static string ReplaceUnderbar(string original)
    {
        string r = original.Replace('_', ' ');
        //Debug.Log("ReplaceUnderbar " + original + " -> " + r);
        return r;
    }

    /// <summary>
    /// original 문자열을 PascalCase로 바꾼 문자열을 반환합니다.
    /// ' '(Space) 외의 공백 문자는 처리하지 않습니다.
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    public static string ToPascalCase(string original)
    {
        string[] token = original.Split(' ');
        string r = "";
        foreach (string s in token)
        {
            r += s.Substring(0, 1).ToUpper() + s.Substring(1);
        }
        //Debug.Log("ToPascalCase " + original + " -> " + r);
        return r;
    }

    /// <summary>
    /// 값이 한 자리 숫자일 경우 앞에 띄어쓰기를 붙여서 반환합니다.
    /// TODO 값이 항상 99 이하라고 가정합니다.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string Padding(int value)
    {
        if (value < 10)
        {
            return " " + value;
        }
        else
        {
            return "" + value;
        }
    }

    /// <summary>
    /// 영어로 된 original 문자열을 설정된 언어로 번역합니다.
    /// 만약 번역에 실패하면 original을 그대로 반환합니다.
    /// </summary>
    /// <param name="original"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    public string Translate(string original)
    {
        Language language = languageSetting;
        if (original == null)
        {
            return "";
        }
        if (dictionary.ContainsKey(original) &&
            dictionary[original] != null && dictionary[original].Count > (int)language)
        {
            return dictionary[original][(int)language];
        }
        else
        {
            return original;
        }
    }

    /// <summary>
    /// 각종 UI 텍스트를 현재 언어 설정에 맞게 새로고침합니다.
    /// </summary>
    public void RefreshTexts()
    {
        if (ready)
        {
            afterSetLanguage();
        }
    }
}
