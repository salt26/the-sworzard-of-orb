using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponTooltipUI : TooltipUI {

    [HideInInspector]
    public Weapon weaponReference;
    public Text weaponNameText;
    public Text statText;
    public Text fireText, iceText, natureText;
    public Text weaponEffectText;

    void Start()
    {
        fireText.color = ColorManager.cm.fireColor;
        iceText.color = ColorManager.cm.iceColor;
        natureText.color = ColorManager.cm.natureColor;
    }

    void Update () {
		if (weaponReference == null || !weaponReference.Equals(GameManager.gm.player.EquippedWeapon))
        {
            weaponReference = GameManager.gm.player.EquippedWeapon;
        }
        weaponNameText.text = StringManager.sm.Translate(weaponReference.name);
        statText.text = "(<color=#F13E00>" + StringManager.Padding(weaponReference.element.Fire) +
            "</color>/<color=#007CF1>" + StringManager.Padding(weaponReference.element.Ice) +
            "</color>/<color=#18B300>" + StringManager.Padding(weaponReference.element.Nature) + "</color>)";
        string effectText = "";
        effectText += StringManager.sm.Translate("Range:") + " " + weaponReference.Range + '\n';
        effectText += StringManager.sm.Translate("Charge bonus:") + " " + (int)(weaponReference.chargeBonus * 100) + "%\n";
        if (weaponReference.PureAmpElement.Fire != 0)
            effectText += StringManager.sm.Translate("FireAmp") + ": 0 + " + weaponReference.PureAmpElement.Fire + " + 0 + 0\n";
        if (weaponReference.PureAmpElement.Ice != 0)
            effectText += StringManager.sm.Translate("IceAmp") + ": 0 + 0 + " + weaponReference.PureAmpElement.Ice + " + 0\n";
        if (weaponReference.PureAmpElement.Nature != 0)
            effectText += StringManager.sm.Translate("NatureAmp") + ": 0 + 0 + 0 + " + weaponReference.PureAmpElement.Nature + "\n";

        foreach (KeyValuePair<string, int> p in weaponReference.effects)
        {
            if (p.Key.Equals("Drain") || p.Key.Equals("Stun"))
            {
                effectText += StringManager.sm.Translate(p.Key) + ": " + p.Value + "%\n";
            }
            else if (p.Key.EndsWith("Amp") || p.Key.Equals("Sharpen"))
            {

            }
            else
            {
                effectText += StringManager.sm.Translate(p.Key) + ": " + p.Value + "\n";
            }
        }
        weaponEffectText.text = effectText.Substring(0, effectText.Length - 1);
    }
}
