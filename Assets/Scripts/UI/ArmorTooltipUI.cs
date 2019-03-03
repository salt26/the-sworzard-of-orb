using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmorTooltipUI : TooltipUI
{

    [HideInInspector]
    public Armor armorReference;
    public Text nameText;
    public Text statText;
    public Text armorEffectText;

    void Update()
    {
        if (armorReference == null || !armorReference.Equals(GameManager.gm.player.EquippedWeapon))
        {
            armorReference = GameManager.gm.player.armor;
        }
        nameText.text = StringManager.sm.Translate("Armor");
        statText.text = "(<color=#F13E00>" + StringManager.Padding(armorReference.element.Fire) +
            "</color>/<color=#007CF1>" + StringManager.Padding(armorReference.element.Ice) +
            "</color>/<color=#18B300>" + StringManager.Padding(armorReference.element.Nature) + "</color>)";
        string effectText = "";

        foreach (KeyValuePair<string, int> p in armorReference.effects)
        {
            if (p.Key.Equals("Reflect"))
            {
                effectText += StringManager.sm.Translate(p.Key) + ": " + p.Value + "%\n";
            }
            else
            {
                effectText += StringManager.sm.Translate(p.Key) + ": " + p.Value + "\n";
            }
        }
        if (effectText.Length > 0)
        {
            armorEffectText.text = effectText.Substring(0, effectText.Length - 1);
            armorEffectText.gameObject.SetActive(true);
        }
        else
        {
            armorEffectText.text = "";
            armorEffectText.gameObject.SetActive(false);
        }
    }
}
