using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponTooltipUI : TooltipUI {

    [HideInInspector]
    public Weapon weaponReference;
    public Text weaponNameText;
    public Text fireText, iceText, natureText;

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
        fireText.text = StringManager.Padding(weaponReference.element.Fire);
        iceText.text = StringManager.Padding(weaponReference.element.Ice);
        natureText.text = StringManager.Padding(weaponReference.element.Nature);
    }
}
