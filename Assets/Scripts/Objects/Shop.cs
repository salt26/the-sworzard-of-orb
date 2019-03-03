using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : Interactable {

    private GameObject shopPanel;

    void Start()
    {
        shopPanel = GameManager.gm.Canvas.GetComponent<UIInfo>().shopPanel;
    }

    public override void Interact(bool isCharge = false)
    {
        GameManager.gm.ShopTurn();
        shopPanel.SetActive(true);
    }
}
