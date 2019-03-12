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

    void Awake()
    {
        isDisappearing = false;
        time = Time.time;
    }

    void Update()
    {
        if (time > 0f && Time.time >= time + 8f)
        {
            Disappear();
        }
        if (armorReference == null || !armorReference.Equals(GameManager.gm.player.EquippedWeapon))
        {
            armorReference = GameManager.gm.player.armor;
        }
        nameText.text = StringManager.sm.Translate("Armor");
        statText.text = "(<color=#F13E00>" + StringManager.Padding(armorReference.element.Fire) +
            "</color>/<color=#007CF1>" + StringManager.Padding(armorReference.element.Ice) +
            "</color>/<color=#18B300>" + StringManager.Padding(armorReference.element.Nature) + "</color>)";
        string effectText = "";

        effectText += StringManager.sm.Translate("Defense:") + " <color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.baseColor) + ">" +
            StringManager.Padding(armorReference.BaseDefense()) +
            "</color> / <color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.fireColor) + ">" +
            StringManager.Padding(armorReference.ValidElement.Fire) +
            "</color> / <color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.iceColor) + ">" +
            StringManager.Padding(armorReference.ValidElement.Ice) +
            "</color> / <color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.natureColor) + ">" +
            StringManager.Padding(armorReference.ValidElement.Nature) + "</color>\n\n";

        foreach (KeyValuePair<string, int> p in armorReference.Effects)
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
            armorEffectText.text = effectText.TrimEnd(new char[] { '\n' });
            armorEffectText.gameObject.SetActive(true);
        }
        else
        {
            armorEffectText.text = "";
            armorEffectText.gameObject.SetActive(false);
        }
    }
}
