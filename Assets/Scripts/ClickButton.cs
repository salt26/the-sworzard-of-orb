using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickButton : MonoBehaviour, IPointerClickHandler {

    GameManager gm;

    void Start()
    {
        gm = GameManager.gm;
    }

    void Update()
    {
        if (gm.Turn == 0 && gm.IsSceneLoaded &&
            !gm.player.GetComponent<Mover>().IsMoving)
        {
            if (!GetComponent<Button>().interactable)
                GetComponent<Button>().interactable = true;
        }
        else
        {
            if (GetComponent<Button>().interactable)
                GetComponent<Button>().interactable = false;
        }
    }


    public void OnPointerClick(PointerEventData e)
    {
        if (e.button == PointerEventData.InputButton.Right)
        {
            gm.player.GetComponent<Inventory>().RemoveItem(GetComponent<Button>());
        }
    }
}
