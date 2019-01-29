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
            itemNameText.text = ii.name;
            if (ii.type == ItemInfo.Type.Orb) {
                SetTooltipText(ii);
            }
            else
            {
                itemTooltipText.resizeTextMaxSize = 36;
                itemTooltipText.text = ii.tooltip;
            }
        }
        else if (ButtonIndex >= 100 && ButtonIndex != 103 &&
            ButtonIndex - 100 < altarUI.Orbs.Count)
        {
            ItemInfo ii = ItemManager.im.FindItemInfo(altarUI.Orbs[ButtonIndex - 100].Value);
            itemNameText.text = ii.name;
            itemNameText.color = new Color(0.35f, 0.27f, 0.2f);

            SetTooltipText(ii);
        }
        else if (ButtonIndex == 103 && altarUI.combineButton.IsInteractable())
        {
            ItemInfo ii = ItemManager.im.FindItemInfo(
                ItemManager.im.FindOrbCombResultID(altarUI.Orbs[0].Value, altarUI.Orbs[1].Value, altarUI.Orbs[2].Value));
            itemNameText.text = ii.name;
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
}
