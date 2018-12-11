using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour {

    /*
     * 현재는 Character에서 이벤트가 일어날 때마다 여기에 있는 Update 메서드를 호출하여 텍스트를 업데이트합니다.
     * 적을 클릭하여 그 적의 정보를 보여주는 시스템을 구현할 때에는, 반대로 여기서 스스로 텍스트를 업데이트해야 합니다.
     */

    //public Character character;

    private bool isFolded = false;   // 접혀 있는 동안 true가 됩니다.

    [Header("Reference")]
    public Text healthText;
    public Text attackBaseText;
    public Text attackFireText;
    public Text attackIceText;
    public Text attackNatureText;
    public Text defenseBaseText;
    public Text defenseFireText;
    public Text defenseIceText;
    public Text defenseNatureText;

    /// <summary>
    /// UI가 접혀 있는 동안 true가 됩니다.
    /// </summary>
    public bool Folded
    {
        get
        {
            return isFolded;
        }
    }

    public void UpdateHealthText(int currentHealth, int maxHealth)
    {
        if (maxHealth <= 0) return;
        if (currentHealth < 0) currentHealth = 0;

        if (healthText != null && !Folded)
        {
            healthText.text = "체: <color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.baseColor) + ">" + currentHealth + "</color> / " + maxHealth;
        }
    }

    public void UpdateAttackText(Weapon weapon)
    {
        if (weapon == null) return;
        if (Folded) return;         // TODO

        if (attackBaseText != null)
        {
            attackBaseText.text = "공: <color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.baseColor) + ">" + Padding(weapon.BaseAttack()) + "</color>";
        }

        if (attackFireText != null && weapon.ValidElement().Fire > 0)
        {
            attackFireText.text = "+<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.fireColor) + ">" + Padding(weapon.ValidElement().Fire) + "</color>";
        }
        else
        {
            attackFireText.text = "";
        }

        if (attackIceText != null && weapon.ValidElement().Ice > 0)
        {
            attackIceText.text = "+<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.iceColor) + ">" + Padding(weapon.ValidElement().Ice) + "</color>";
        }
        else
        {
            attackIceText.text = "";
        }

        if (attackNatureText != null && weapon.ValidElement().Nature > 0)
        {
            attackNatureText.text = "+<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.natureColor) + ">" + Padding(weapon.ValidElement().Nature) + "</color>";
        }
        else
        {
            attackNatureText.text = "";
        }
    }

    public void UpdateDefenseText(Armor armor)
    {
        if (armor == null) return;
        if (Folded) return;         // TODO

        if (defenseBaseText != null)
        {
            defenseBaseText.text = "방: <color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.baseColor) + ">" + Padding(armor.BaseDefense) + "</color>";
        }

        if (defenseFireText != null)
        {
            defenseFireText.text = "/ <color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.fireColor) + ">" + Padding(armor.ValidElement().Fire) + "</color>";
        }

        if (defenseIceText != null)
        {
            defenseIceText.text = "/ <color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.iceColor) + ">" + Padding(armor.ValidElement().Ice) + "</color>";
        }

        if (defenseNatureText != null)
        {
            defenseNatureText.text = "/ <color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.natureColor) + ">" + Padding(armor.ValidElement().Nature) + "</color>";
        }
    }

    /// <summary>
    /// 값이 한 자리 숫자일 경우 앞에 띄어쓰기를 붙여서 반환합니다.
    /// TODO 값이 항상 99 이하라고 가정합니다.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private string Padding(int value)
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
}
