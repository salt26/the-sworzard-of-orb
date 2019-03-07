using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public enum UIType { Inventory, Altar, Shop, Repurchase, Turn, MonsterNumber, Recipe }
    public GameObject tooltipPanel;
    public Vector2 minPos;          // 각 원소는 0 이상 1 이하
    public Vector2 maxPos;          // 각 원소는 0 이상 1 이하, minPos보다 커야 함
    public int itemIDInRecipe;      // type이 Recipe인 경우에만 작동

    private TooltipUI myTooltip = null;
    [SerializeField]
    private UIType type;

    public void OnPointerEnter(PointerEventData e)
    {
        if (myTooltip != null)
        {
            myTooltip.Disappear();
            myTooltip = null;
        }
        if (myTooltip == null)
        {
            int index = -1;
            if (tooltipPanel.GetComponent<ItemTooltipUI>() != null)
            {
                if (type == UIType.Inventory)
                {
                    index = int.Parse(gameObject.name);
                    if (index >= GameManager.gm.player.GetComponent<Inventory>().Items.Count)
                        return;
                }
                else if (type == UIType.Altar)
                {
                    if (gameObject.name.Equals("TopButton")) index = 100;
                    else if (gameObject.name.Equals("LeftButton")) index = 101;
                    else if (gameObject.name.Equals("RightButton")) index = 102;
                    else index = 103;

                    if (!(index == 103 && GameManager.gm.Canvas.GetComponent<UIInfo>().altarPanel.GetComponent<AltarUI>().combineButton.IsInteractable()) && 
                        index - 100 >= GameManager.gm.Canvas.GetComponent<UIInfo>().altarPanel.GetComponent<AltarUI>().Orbs.Count)
                        return;
                }
                else if (type == UIType.Shop)
                {
                    index = int.Parse(gameObject.name.Substring(gameObject.name.Length - 1)) - 1 + 200;
                    if (GetComponent<PurchaseButtonUI>().ItemName == null) return;
                }
                else if (type == UIType.Repurchase)
                {
                    index = int.Parse(gameObject.name.Substring(0, 1)) + 300;
                    if (GameManager.gm.Canvas.GetComponent<UIInfo>().shopPanel.GetComponent<ShopUI>().RepurchaseItems.Count <= index - 300 || 
                        GameManager.gm.Canvas.GetComponent<UIInfo>().shopPanel.GetComponent<ShopUI>().RepurchaseItems[index - 300] == null)
                        return;
                }
                else if (type == UIType.Recipe)
                {
                    if (ItemManager.im.FindItemInfo(itemIDInRecipe) == null) return;
                    index = itemIDInRecipe + 400;
                }
            }
            GameObject g = Instantiate(tooltipPanel, GameManager.gm.Canvas.GetComponent<Transform>());
            myTooltip = g.GetComponent<TooltipUI>();
            RectTransform r = myTooltip.GetComponent<RectTransform>();
            r.anchorMin = minPos;
            r.anchorMax = maxPos;
            if (type == UIType.Altar && index != 103) g.GetComponent<Image>().color = new Color(0.8f, 0.7f, 0.55f);
            else if (type == UIType.Altar || type == UIType.Recipe) g.GetComponent<Image>().color = new Color(0.8f, 0.45f, 0.7f);
            else if (type == UIType.Shop) g.GetComponent<Image>().color = new Color(0.8f, 0.45f, 0.7f);
            else if (type == UIType.Repurchase) g.GetComponent<Image>().color = new Color(0.8f, 0.7f, 0.55f);

            if (myTooltip.GetComponent<WeaponTooltipUI>() != null)
            {
                myTooltip.GetComponent<WeaponTooltipUI>().weaponReference = GameManager.gm.player.EquippedWeapon;
            }
            if (myTooltip.GetComponent<ItemTooltipUI>() != null)
            {
                myTooltip.GetComponent<ItemTooltipUI>().ButtonIndex = index;
                if (type == UIType.Inventory)
                {
                    myTooltip.GetComponent<RectTransform>().pivot = new Vector2(0f, 0f);
                }
                else if (type == UIType.Altar || type == UIType.Recipe)
                {
                    myTooltip.GetComponent<RectTransform>().pivot = new Vector2(myTooltip.GetComponent<RectTransform>().pivot.x, 1f);
                }
                else if (type == UIType.Shop)
                {
                    myTooltip.GetComponent<RectTransform>().pivot = new Vector2(myTooltip.GetComponent<RectTransform>().pivot.x, 1f);
                }
                else if (type == UIType.Repurchase)
                {
                    myTooltip.GetComponent<RectTransform>().pivot = new Vector2(myTooltip.GetComponent<RectTransform>().pivot.x, 1f);
                }
            }
            if (myTooltip.GetComponent<NormalTooltipUI>() != null)
            {
                myTooltip.GetComponent<RectTransform>().pivot = new Vector2(myTooltip.GetComponent<RectTransform>().pivot.x, 1f);
                if (type == UIType.MonsterNumber)
                {
                    myTooltip.GetComponent<NormalTooltipUI>().content = "The number of live monsters";
                }
                else if (type == UIType.Turn)
                {
                    myTooltip.GetComponent<NormalTooltipUI>().content = "@ turns left before you die";
                    myTooltip.GetComponent<NormalTooltipUI>().param = GameManager.gm.RemainedTurn;
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (myTooltip != null)
        {
            myTooltip.Disappear();
        }
    }

    
}
