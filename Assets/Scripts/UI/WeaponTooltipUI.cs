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
    
    void Awake()
    {
        isDisappearing = false;
        time = Time.time;
    }

    void Start()
    {
        fireText.color = ColorManager.cm.fireColor;
        iceText.color = ColorManager.cm.iceColor;
        natureText.color = ColorManager.cm.natureColor;
    }

    void Update () {
        if (time > 0f && Time.time >= time + 8f)
        {
            Disappear();
        }
        if (weaponReference == null || !weaponReference.Equals(GameManager.gm.player.EquippedWeapon))
        {
            weaponReference = GameManager.gm.player.EquippedWeapon;
        }
        weaponNameText.text = StringManager.sm.Translate(weaponReference.name);
        statText.text = "(<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.fireColor) + ">" +
            StringManager.Padding(weaponReference.element.Fire) +
            "</color>/<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.iceColor) + ">" +
            StringManager.Padding(weaponReference.element.Ice) +
            "</color>/<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.natureColor) + ">" +
            StringManager.Padding(weaponReference.element.Nature) + "</color>)";
        string effectText = "";
        effectText += StringManager.sm.Translate("Range:") + " " + weaponReference.Range + '\n';
        if (weaponReference.Range > 1)
            effectText += StringManager.sm.Translate("Element for calc:") + " (" +
                StringManager.Padding(weaponReference.OriginalElement.Fire / weaponReference.Range) + "/" +
                StringManager.Padding(weaponReference.OriginalElement.Ice / weaponReference.Range) + "/" +
                StringManager.Padding(weaponReference.OriginalElement.Nature / weaponReference.Range) + ")\n";
        if (weaponReference.PureAmpElement.Fire != 0)
            effectText += StringManager.sm.Translate("FireAmp:\t\t\t") + " 0 + " + StringManager.Padding(weaponReference.PureAmpElement.Fire) + " +  0 +  0\n";
        if (weaponReference.PureAmpElement.Ice != 0)
            effectText += StringManager.sm.Translate("IceAmp:\t\t\t") + " 0 +  0 + " + StringManager.Padding(weaponReference.PureAmpElement.Ice) + " +  0\n";
        if (weaponReference.PureAmpElement.Nature != 0)
            effectText += StringManager.sm.Translate("NatureAmp:\t\t") + " 0 +  0 +  0 + " + StringManager.Padding(weaponReference.PureAmpElement.Nature) + "\n";
        effectText += StringManager.sm.Translate("Normal attack:\t") + "<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.baseColor) + ">" +
            StringManager.Padding(weaponReference.BaseAttack()) +
            "</color> + <color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.fireColor) + ">" +
            StringManager.Padding(weaponReference.ValidElement.Fire) +
            "</color> + <color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.iceColor) + ">" +
            StringManager.Padding(weaponReference.ValidElement.Ice) +
            "</color> + <color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.natureColor) + ">" +
            StringManager.Padding(weaponReference.ValidElement.Nature) + "</color>\n\n";
        effectText += StringManager.sm.Translate("Charge bonus:") + " " + Mathf.RoundToInt(weaponReference.chargeBonus * 100) + "%\n";
        effectText += StringManager.sm.Translate("Charge attack:\t") + "<color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.baseColor) + ">" +
            StringManager.Padding((int)(weaponReference.BaseAttack() * weaponReference.chargeBonus)) +
            "</color> + <color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.fireColor) + ">" +
            StringManager.Padding(weaponReference.ValidElement.Fire) +
            "</color> + <color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.iceColor) + ">" +
            StringManager.Padding(weaponReference.ValidElement.Ice) +
            "</color> + <color=#" + ColorUtility.ToHtmlStringRGB(ColorManager.cm.natureColor) + ">" +
            StringManager.Padding(weaponReference.ValidElement.Nature) + "</color>\n\n";
        if (weaponReference.stunEffectInTooltip > 0f)
            effectText += StringManager.sm.Translate("Stun") + ": " + Mathf.RoundToInt(weaponReference.stunEffectInTooltip * 10000) / 100f + "%\n";

        foreach (KeyValuePair<string, int> p in weaponReference.Effects)
        {
            if (p.Key.Equals("Drain"))
            {
                effectText += StringManager.sm.Translate(p.Key) + ": " + p.Value + "%\n";
            }
            else if (p.Key.EndsWith("Amp") || p.Key.Equals("Sharpen") || p.Key.Equals("Stun"))
            {

            }
            else
            {
                effectText += StringManager.sm.Translate(p.Key) + ": " + p.Value + "\n";
            }
        }
        weaponEffectText.text = effectText.TrimEnd(new char[] { '\n' });
    }
}
