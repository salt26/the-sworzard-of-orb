using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Altar : Interactable {

    private GameObject altarPanel;

    void Start()
    {
        altarPanel = GameManager.gm.Canvas.GetComponent<UIInfo>().altarPanel;
    }

    public override void Interact(bool isCharge = false)
    {
        GameManager.gm.AltarTurn();
        altarPanel.SetActive(true);
    }
}
