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
    private ShopUI shopUI;

    void Awake()
    {
        isDisappearing = false;
        time = Time.time;
    }

    void Start()
    {
        altarUI = GameManager.gm.Canvas.GetComponent<UIInfo>().altarPanel.GetComponent<AltarUI>();
        shopUI = GameManager.gm.Canvas.GetComponent<UIInfo>().shopPanel.GetComponent<ShopUI>();
    }

    // Update is called once per frame
    void Update () {
        if (time > 0f && Time.time >= time + 8f)
        {
            Disappear();
        }
        if (ButtonIndex >= 0 && ButtonIndex < 100 &&
                ButtonIndex < GameManager.gm.player.GetComponent<Inventory>().Items.Count)
        {
            ItemInfo ii = ItemManager.im.FindItemInfo(GameManager.gm.player.GetComponent<Inventory>().Items[ButtonIndex]);
            itemNameText.text = StringManager.sm.Translate(ii.name);
            SetTooltipText(ii);
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
        else if (ButtonIndex >= 200 && ButtonIndex < 300 && shopUI.gameObject.activeInHierarchy)
        {
            ItemInfo ii = ItemManager.im.FindItemInfo(shopUI.purchaseButtons[ButtonIndex - 200].GetComponent<PurchaseButtonUI>().ItemName);
            if (ii != null)
            {
                itemNameText.text = StringManager.sm.Translate(ii.name);
                itemNameText.color = new Color(0.35f, 0.15f, 0.35f);

                SetTooltipText(ii);
            }
        }
        else if (ButtonIndex >= 300 && ButtonIndex < 400 && shopUI.RepurchaseItems.Count > ButtonIndex - 300)
        {
            ItemInfo ii = ItemManager.im.FindItemInfo(shopUI.RepurchaseItems[ButtonIndex - 300]);
            if (ii != null)
            {
                itemNameText.text = StringManager.sm.Translate(ii.name);
                itemNameText.color = new Color(0.35f, 0.27f, 0.2f);

                SetTooltipText(ii);
            }
        }
        else if (ButtonIndex >= 400)
        {
            ItemInfo ii = ItemManager.im.FindItemInfo(ButtonIndex - 400);
            if (ii != null && ii.level - 1 <= altarUI.recipePanels.Count && ii.level >= 1 && 
                altarUI.recipePanels[ii.level - 2].activeInHierarchy)
            {
                itemNameText.text = StringManager.sm.Translate(ii.name);
                itemNameText.color = new Color(0.35f, 0.15f, 0.35f);

                SetTooltipText(ii);
            }
            else
            {
                Disappear();
            }
        }
        else
        {
            Disappear();
        }
	}

    private void SetTooltipText(ItemInfo ii)
    {
        //itemTooltipText.resizeTextMaxSize = 28;

        itemTooltipText.text = StringManager.sm.Translate(ii.type.ToString()) + "\n";
        if (ii.type == ItemInfo.Type.Orb)
        {
            itemTooltipText.text = StringManager.sm.Translate("Lv.@").Replace("@", ii.level.ToString()) + " " +
               StringManager.sm.Translate(ii.type.ToString()) + "\n";
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
            itemTooltipText.text += ii.stat + "\n";
        }
        if (ii.effectName != null && !ii.effectName.Equals("") && !ii.effectName.Equals("None"))
        {
            itemTooltipText.text += StringManager.sm.Translate("Effect:") + " ";
            if (ii.effectName.Equals("Drain") || ii.effectName.Equals("Stun") || ii.effectName.Equals("Reflect") || ii.effectName.Equals("Sharpen"))
            {
                itemTooltipText.text += StringManager.sm.Translate(ii.effectName) + "(" + ii.effectParam + "%)\n";
            }
            else if (ii.effectName.EndsWith("Amp"))
            {
                itemTooltipText.text += StringManager.sm.Translate(ii.effectName) + "(" + ii.effectParam + "%)\n";
            }
            else if (ii.effectName.Equals("Return"))
            {
                itemTooltipText.text += StringManager.sm.Translate(ii.effectName) + "\n";
            }
            else
            {
                itemTooltipText.text += StringManager.sm.Translate(ii.effectName) + "(" + ii.effectParam + ")\n";
            }
        }
        itemTooltipText.text += "\n" + StringManager.sm.Translate(ii.tooltip).Replace("@", ii.effectParam.ToString()) + "\n";
        if (ButtonIndex >= 100 && ButtonIndex < 103)
        {
            itemTooltipText.text += StringManager.sm.Translate("Sell price:") + " " + ii.SellPrice;
            itemTooltipText.text += "\n\n" + StringManager.sm.Translate("[Left click to get back]");
        }
        else if (ButtonIndex == 103)
        {
            itemTooltipText.text += StringManager.sm.Translate("Sell price:") + " " + ii.SellPrice;
            itemTooltipText.text += "\n\n" + StringManager.sm.Translate("[Left click to compound]");
        }
        else if (ButtonIndex >= 200 && ButtonIndex < 300)
        {
            itemTooltipText.text += StringManager.sm.Translate("Buy price:") + " " + ii.BuyPrice;
            itemTooltipText.text += "\n\n" + StringManager.sm.Translate("[Left click to buy]");
        }
        else if (ButtonIndex >= 300 && ButtonIndex < 400)
        {
            itemTooltipText.text += StringManager.sm.Translate("Repurchase price:") + " " + ii.SellPrice;
            itemTooltipText.text += "\n\n" + StringManager.sm.Translate("[Left click to repurchase]");
        }
        else
            itemTooltipText.text += StringManager.sm.Translate("Sell price:") + " " + ii.SellPrice;

        if (ButtonIndex < 100 && GameManager.gm.IsSceneLoaded &&
            ButtonIndex < GameManager.gm.player.GetComponent<Inventory>().itemButtons.Count )
        {
            if (GameManager.gm.Turn == 0 &&
                GameManager.gm.player.GetComponent<Inventory>().itemButtons[ButtonIndex].interactable)
            {
                itemTooltipText.text += "\n\n" + StringManager.sm.Translate("[Left click to use]");
                itemTooltipText.text += "\n" + StringManager.sm.Translate("[Right click to drop]");
            }
            else if (GameManager.gm.Turn == 0)
            {
                itemTooltipText.text += "\n\n" + StringManager.sm.Translate("[Right click to drop]");
            }
            else if (GameManager.gm.Turn == 3 &&
                GameManager.gm.player.GetComponent<Inventory>().itemButtons[ButtonIndex].interactable)
            {
                if (GameManager.gm.Canvas.GetComponent<UIInfo>().altarPanel.GetComponent<AltarUI>().IsContainOrbIndex(ButtonIndex))
                {
                    itemTooltipText.text += "\n\n" + StringManager.sm.Translate("[Left click to get back]");
                }
                else
                {
                    itemTooltipText.text += "\n\n" + StringManager.sm.Translate("[Left click to dedicate]");
                }
            }
            else if (GameManager.gm.Turn == 4 &&
                GameManager.gm.player.GetComponent<Inventory>().itemButtons[ButtonIndex].interactable)
            {
                itemTooltipText.text += "\n\n" + StringManager.sm.Translate("[Left click to sell]");
            }
        }
    }
}
