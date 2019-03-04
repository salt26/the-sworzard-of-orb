using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseButtonUI : MonoBehaviour {

    public ShopUI shopUI;
    public Transform placeholder;
    public Text nameText;
    public Text costText;

    private int index = -1;
    private KeyValuePair<string, GameObject> myItem = new KeyValuePair<string, GameObject>(null, null);

    public string ItemName
    {
        get
        {
            return myItem.Key;
        }
    }

    private void Start()
    {
        index = shopUI.purchaseButtons.FindIndex(b => b.Equals(GetComponent<Button>()));
    }

    void Update () {
		if (GameManager.gm == null || index == -1)
        {
            return;
        }
        string itemName;
        if (index < shopUI.purchaseItems.Count && ItemManager.im.FindItemInfo(shopUI.purchaseItems[index]) != null)
        {
            itemName = shopUI.purchaseItems[index];
            if (myItem.Key != null && !myItem.Key.Equals(itemName))
            {
                Destroy(myItem.Value);
                myItem = new KeyValuePair<string, GameObject>(null, null);
            }
            if (myItem.Key == null && myItem.Value != null)
            {
                Destroy(myItem.Value);
                myItem = new KeyValuePair<string, GameObject>(null, null);
            }
            if (myItem.Key == null || myItem.Value == null)
            {
                myItem = new KeyValuePair<string, GameObject>(itemName, Instantiate(shopUI.itemImage, placeholder));
                myItem.Value.GetComponent<Image>().sprite = ItemManager.im.GetItemSprite(itemName);
                costText.text = ItemManager.im.FindItemInfo(shopUI.purchaseItems[index]).BuyPrice.ToString();
            }
            nameText.text = StringManager.sm.Translate(itemName);
            if (!GetComponent<Button>().interactable)
            {
                GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            itemName = null;
            nameText.text = StringManager.sm.Translate("Sold out");
            costText.text = "0";
            if (myItem.Key != null && myItem.Value != null)
            {
                Destroy(myItem.Value);
                myItem = new KeyValuePair<string, GameObject>(null, null);
            }
            if (GetComponent<Button>().interactable)
            {
                GetComponent<Button>().interactable = false;
            }
        }
	}
}
