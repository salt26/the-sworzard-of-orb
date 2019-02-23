using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltipUI : TooltipUI {

    [HideInInspector]
    public int ButtonIndex;
    public Text itemNameText;
    public Text itemTooltipText;

    private AltarUI altarUI;

    void Start()
    {
        altarUI = GameManager.gm.Canvas.GetComponent<UIInfo>().altarPanel.GetComponent<AltarUI>();
    }

    // Update is called once per frame
    void Update () {
		if (ButtonIndex >= 0 && ButtonIndex < 100 &&
                ButtonIndex < GameManager.gm.player.GetComponent<Inventory>().Items.Count)
        {
            ItemInfo ii = ItemManager.im.FindItemInfo(GameManager.gm.player.GetComponent<Inventory>().Items[ButtonIndex]);
            itemNameText.text = StringManager.sm.Translate(ii.name);
            if (ii.type == ItemInfo.Type.Orb) {
                SetTooltipText(ii);
            }
            else
            {
                itemTooltipText.resizeTextMaxSize = 36;
                itemTooltipText.text = StringManager.sm.Translate(ii.tooltip);
            }
        }
        else if (ButtonIndex >= 100 && ButtonIndex != 103 &&
            ButtonIndex - 100 < altarUI.Orbs.Count)
        {
            ItemInfo ii = ItemManager.im.FindItemInfo(altarUI.Orbs[ButtonIndex - 100].Value);
            itemNameText.text = StringManager.sm.Translate(ii.name);
            itemNameText.color = new Color(0.35f, 0.27f, 0.2f);

            SetTooltipText(ii);
        }
        else if (ButtonIndex == 103 && altarUI.combineButton.IsInteractable() && !altarUI.whitePanel.activeInHierarchy)
        {
            ItemInfo ii = ItemManager.im.FindItemInfo(
                ItemManager.im.FindOrbCombResultID(altarUI.Orbs[0].Value, altarUI.Orbs[1].Value, altarUI.Orbs[2].Value));
            itemNameText.text = StringManager.sm.Translate(ii.name);
            itemNameText.color = new Color(0.35f, 0.15f, 0.35f);

            SetTooltipText(ii);
        }
        else
        {
            Disappear();
        }
	}

    private void SetTooltipText(ItemInfo ii)
    {
        itemTooltipText.resizeTextMaxSize = 28;

        itemTooltipText.text = StringManager.sm.Translate("Lv.") + ii.level + " " + ii.stat + "\n";
        if (ii.usage == ItemInfo.Usage.None)
        {
            itemTooltipText.text += StringManager.sm.Translate("Ingredient") + "\n";
        }
        else if (ii.usage == ItemInfo.Usage.Weapon)
        {
            itemTooltipText.text += StringManager.sm.Translate("Weapon only") + "\n";
        }
        else if (ii.usage == ItemInfo.Usage.Armor)
        {
            itemTooltipText.text += StringManager.sm.Translate("Armor only") + "\n";
        }
        //itemTooltipText.text += StringManager.sm.Translate(ii.effectName) + " " + ii.effectParam + "\n";   // TODO 예쁘게 보이게
        itemTooltipText.text += "\n" + StringManager.sm.Translate(ii.tooltip);
    }
}
