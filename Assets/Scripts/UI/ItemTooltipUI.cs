using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltipUI : TooltipUI {

    [HideInInspector]
    public int ButtonIndex;
    public Text itemNameText;
    public Text itemTooltipText;
	
	// Update is called once per frame
	void Update () {
		if (ButtonIndex >= 0 && ButtonIndex < 100 &&
                ButtonIndex < GameManager.gm.player.GetComponent<Inventory>().Items.Count)
        {
            ItemInfo ii = ItemManager.im.FindItemInfo(GameManager.gm.player.GetComponent<Inventory>().Items[ButtonIndex]);
            itemNameText.text = ii.name;
            if (ii.type == ItemInfo.Type.Orb) {
                itemTooltipText.resizeTextMaxSize = 28;

                itemTooltipText.text = "Lv." + ii.level + " " + ii.stat + "\n";
                if (ii.usage == ItemInfo.Usage.None)
                {
                    itemTooltipText.text += "재료\n";
                }
                else if (ii.usage == ItemInfo.Usage.Weapon)
                {
                    itemTooltipText.text += "무기 전용\n";
                }
                else if (ii.usage == ItemInfo.Usage.Armor)
                {
                    itemTooltipText.text += "방어구 전용\n";
                }
                //itemTooltipText.text += ii.effectName + " " + ii.effectParam + "\n";   // TODO 예쁘게 보이게
                itemTooltipText.text += "\n" + ii.tooltip;
            }
            else
            {
                itemTooltipText.resizeTextMaxSize = 36;
                itemTooltipText.text = ii.tooltip;
            }
        }
        else if (ButtonIndex >= 100 &&
            ButtonIndex - 100 < GameManager.gm.Canvas.GetComponent<UIInfo>().altarPanel.GetComponent<AltarUI>().Orbs.Count)
        {
            ItemInfo ii = ItemManager.im.FindItemInfo(
                GameManager.gm.Canvas.GetComponent<UIInfo>().altarPanel.GetComponent<AltarUI>().Orbs[ButtonIndex - 100].Value);
            itemNameText.text = ii.name;

            itemTooltipText.resizeTextMaxSize = 28;

            itemTooltipText.text = "Lv." + ii.level + " " + ii.stat + "\n";
            if (ii.usage == ItemInfo.Usage.None)
            {
                itemTooltipText.text += "재료\n";
            }
            else if (ii.usage == ItemInfo.Usage.Weapon)
            {
                itemTooltipText.text += "무기 전용\n";
            }
            else if (ii.usage == ItemInfo.Usage.Armor)
            {
                itemTooltipText.text += "방어구 전용\n";
            }
            //itemTooltipText.text += ii.effectName + " " + ii.effectParam + "\n";   // TODO 예쁘게 보이게
            itemTooltipText.text += "\n" + ii.tooltip;
        }
        else
        {
            Disappear();
        }
	}
}
