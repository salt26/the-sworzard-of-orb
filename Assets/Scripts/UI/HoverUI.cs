using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public GameObject tooltipPanel;
    public Vector2 minPos;          // 각 원소는 0 이상 1 이하
    public Vector2 maxPos;          // 각 원소는 0 이상 1 이하, minPos보다 커야 함

    private TooltipUI myTooltip = null;
    [SerializeField]
    private bool isAltar;

    public void OnPointerEnter(PointerEventData e)
    {
        if (myTooltip == null)
        {
            int index = -1;
            if (tooltipPanel.GetComponent<ItemTooltipUI>() != null)
            {
                if (!isAltar)
                {
                    index = int.Parse(gameObject.name);
                    if (index >= GameManager.gm.player.GetComponent<Inventory>().Items.Count)
                        return;
                }
                else
                {
                    if (gameObject.name.Equals("TopButton")) index = 100;
                    else if (gameObject.name.Equals("LeftButton")) index = 101;
                    else index = 102;

                    if (index - 100 >= GameManager.gm.Canvas.GetComponent<UIInfo>().altarPanel.GetComponent<AltarUI>().Orbs.Count)
                        return;
                }
            }
            GameObject g = Instantiate(tooltipPanel, GameManager.gm.Canvas.GetComponent<Transform>());
            myTooltip = g.GetComponent<TooltipUI>();
            RectTransform r = myTooltip.GetComponent<RectTransform>();
            r.anchorMin = minPos;
            r.anchorMax = maxPos;

            if (myTooltip.GetComponent<WeaponTooltipUI>() != null)
            {
                myTooltip.GetComponent<WeaponTooltipUI>().weaponReference = GameManager.gm.player.EquippedWeapon;
            }
            if (myTooltip.GetComponent<ItemTooltipUI>() != null)
            {
                myTooltip.GetComponent<ItemTooltipUI>().ButtonIndex = index;
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
