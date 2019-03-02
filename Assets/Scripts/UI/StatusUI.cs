using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour {
    
    private bool isFolded = false;   // 접혀 있는 동안 true가 됩니다.

    [Header("Reference")]
    public Text nameText;
    public Text healthText;
    public Text healthPointText;
    public Text attackText;
    public Text attackBaseText;
    public Text attackFireText;
    public Text attackIceText;
    public Text attackNatureText;
    public Text defenseText;
    public Text defenseBaseText;
    public Text defenseFireText;
    public Text defenseIceText;
    public Text defenseNatureText;

    private string characterName = null;

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
            UpdateNameText(character.name);
            UpdateAttackText(character.EquippedWeapon);
            UpdateDefenseText(character.armor);
            UpdateHealthText(currentHealth, character.MaxHealth);
        }
    }

    public void UpdateNameText(string name)
    {
        if (name == null) return;
        if (nameText != null && !Folded)
        {
            nameText.text = StringManager.sm.Translate(name);
            characterName = name;
        }
    }

    public void UpdateHealthText(int currentHealth, int maxHealth)
    {
        if (maxHealth <= 0) return;
        if (currentHealth < 0) currentHealth = 0;

        if (healthPointText != null && !Folded)
        {
            healthPointText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.baseColor) + ">" + currentHealth + "</color> / " + maxHealth;
        }
    }

    public void UpdateAttackText(Weapon weapon)
    {
        if (weapon == null) return;
        if (Folded) return;         // TODO

        if (attackBaseText != null)
        {
            attackBaseText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.baseColor) + ">" + StringManager.Padding(weapon.BaseAttack()) + "</color>";
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
            defenseBaseText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.baseColor) + ">" + StringManager.Padding(armor.BaseDefense()) + "</color>";
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
        if (characterName != null)
            nameText.text = StringManager.sm.Translate(characterName);
        healthText.text = "" + StringManager.sm.Translate("HP :");
        attackText.text = "" + StringManager.sm.Translate("Atk:");
        defenseText.text = "" + StringManager.sm.Translate("Def:");
    }
}
