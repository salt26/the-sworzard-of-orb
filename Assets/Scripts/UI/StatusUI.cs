using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour {
    
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

    public void UpdateAll(Character character, int currentHealth)
    {
        if (GameManager.gm.player.Equals(character) ||
            (GameManager.gm.SelectedCharacter != null && GameManager.gm.SelectedCharacter.Equals(character)))
        {
            UpdateAttackText(character.EquippedWeapon);
            UpdateDefenseText(character.armor);
            UpdateHealthText(currentHealth, character.maxHealth);
        }
    }

    public void UpdateHealthText(int currentHealth, int maxHealth)
    {
        if (maxHealth <= 0) return;
        if (currentHealth < 0) currentHealth = 0;

        if (healthText != null && !Folded)
        {
            healthText.text = "" + StringManager.sm.Translate("HP :") + "<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.baseColor) + ">" + currentHealth + "</color> / " + maxHealth;
        }
    }

    public void UpdateAttackText(Weapon weapon)
    {
        if (weapon == null) return;
        if (Folded) return;         // TODO

        if (attackBaseText != null)
        {
            attackBaseText.text = "" + StringManager.sm.Translate("Atk:") + "<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.baseColor) + ">" + StringManager.Padding(weapon.BaseAttack()) + "</color>";
        }

        if (attackFireText != null/* && weapon.ValidElement.Fire > 0*/)
        {
            attackFireText.text = "+<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.fireColor) + ">" + StringManager.Padding(weapon.ValidElement.Fire) + "</color>";
        }
        else
        {
            attackFireText.text = "";
        }

        if (attackIceText != null/* && weapon.ValidElement.Ice > 0*/)
        {
            attackIceText.text = "+<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.iceColor) + ">" + StringManager.Padding(weapon.ValidElement.Ice) + "</color>";
        }
        else
        {
            attackIceText.text = "";
        }

        if (attackNatureText != null/* && weapon.ValidElement.Nature > 0*/)
        {
            attackNatureText.text = "+<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.natureColor) + ">" + StringManager.Padding(weapon.ValidElement.Nature) + "</color>";
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
            defenseBaseText.text = "" + StringManager.sm.Translate("Def:") + "<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.baseColor) + ">" + StringManager.Padding(armor.BaseDefense()) + "</color>";
        }

        if (defenseFireText != null)
        {
            defenseFireText.text = "/<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.fireColor) + ">" + StringManager.Padding(armor.ValidElement.Fire) + "</color>";
        }

        if (defenseIceText != null)
        {
            defenseIceText.text = "/<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.iceColor) + ">" + StringManager.Padding(armor.ValidElement.Ice) + "</color>";
        }

        if (defenseNatureText != null)
        {
            defenseNatureText.text = "/<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.natureColor) + ">" + StringManager.Padding(armor.ValidElement.Nature) + "</color>";
        }
    }

    public void RefreshText()
    {
        healthText.text = "" + StringManager.sm.Translate("HP :") + healthText.text.Substring(healthText.text.IndexOf(':') + 1);
        attackBaseText.text = "" + StringManager.sm.Translate("Atk:") + attackBaseText.text.Substring(attackBaseText.text.IndexOf(':') + 1);
        defenseBaseText.text = "" + StringManager.sm.Translate("Def:") + defenseBaseText.text.Substring(defenseBaseText.text.IndexOf(':') + 1);
    }
}
