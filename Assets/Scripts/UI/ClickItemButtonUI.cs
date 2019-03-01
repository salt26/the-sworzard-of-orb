using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickItemButtonUI : MonoBehaviour, IPointerClickHandler {

    GameManager gm;

    void Start()
    {
        gm = GameManager.gm;
        GetComponent<Image>().color = Color.white;
    }

    void Update()
    {
        if (gm.Turn == 0 && gm.IsSceneLoaded &&
            !gm.player.GetComponent<Mover>().IsMoving)
        {
            string itemName = gm.player.GetComponent<Inventory>().FindItemNameInButton(GetComponent<Button>());
            if (itemName != null && ItemManager.im.FindItemInfo(itemName).type == ItemInfo.Type.Orb &&
                ItemManager.im.FindItemInfo(itemName).usage == ItemInfo.Usage.None)
            {
                if (GetComponent<Button>().interactable)
                    GetComponent<Button>().interactable = false;
            }
            else if (itemName != null && ItemManager.im.FindItemInfo(itemName).type == ItemInfo.Type.Consumable &&
                !ItemManager.im.InvokeItemUsage(ItemManager.im.FindItemInfo(itemName).effectName, ItemManager.im.FindItemInfo(itemName).effectParam))
            {
                if (GetComponent<Button>().interactable)
                    GetComponent<Button>().interactable = false;
            }
            else if (!GetComponent<Button>().interactable)
                GetComponent<Button>().interactable = true;
        }
        else if (gm.Turn == 3 && gm.IsSceneLoaded)
        {
            string itemName = gm.player.GetComponent<Inventory>().FindItemNameInButton(GetComponent<Button>());
            if (itemName != null && ItemManager.im.FindItemInfo(itemName).type == ItemInfo.Type.Orb)
            {
                if (!GetComponent<Button>().interactable)
                    GetComponent<Button>().interactable = true;
            }
            else if(GetComponent<Button>().interactable)
                GetComponent<Button>().interactable = false;
        }
        else if (gm.Turn == 4 && gm.IsSceneLoaded)
        {
            if (!GetComponent<Button>().interactable)
                GetComponent<Button>().interactable = true;
        }
        else
        {
            if (GetComponent<Button>().interactable)
                GetComponent<Button>().interactable = false;
        }

        // 제단에 올라간 오브 자리의 버튼 색 하이라이트
        if (gm.Turn == 3 && gm.IsSceneLoaded &&
            gm.Canvas.GetComponent<UIInfo>().altarPanel.GetComponent<AltarUI>().IsContainOrbIndex(int.Parse(gameObject.name)))
        {
            if (GetComponent<Image>().color == Color.white)
                GetComponent<Image>().color = new Color(0.3f, 0.4f, 1f, 1f);
        }
        else
        {
            if (GetComponent<Image>().color != Color.white)
                GetComponent<Image>().color = Color.white;
        }
    }


    public void OnPointerClick(PointerEventData e)
    {
        if (e.button == PointerEventData.InputButton.Right)
        {
            gm.player.GetComponent<Inventory>().DropItem(GetComponent<Button>());
        }
    }
}
