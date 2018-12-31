using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringUtility {

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
}
