using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Treasure : Interactable {

    public GameObject canvas;

    private bool hasinteracted = false;

    public override void Interact(bool isCharge = false)
    {
        if (hasinteracted) return;
        
        hasinteracted = true;
        if (Random.Range(0, 4) == 0)
        {
            canvas.SetActive(true);
            GetComponent<BoxCollider>().enabled = true;
            GetComponent<EnemyMover>().enabled = true;
            GetComponent<Character>().enabled = true;
            GetComponent<Character>().statusUI = GameManager.gm.Canvas.GetComponent<UIInfo>().enemyStatusUI;
            GetComponent<Character>().level = GameManager.gm.mapLevel;
            GameManager.gm.interactables.Remove(this);
            GameManager.gm.enemies.Add(GetComponent<Character>());
            GameManager.gm.map.SetEntityOnTile(GetComponent<Character>(), GetComponent<Transform>().position);
        }
        else
        {
            GameManager.gm.map.AddGoldOnTile(5, GetComponent<Transform>().position);

            // Large potion
            /*
            if (Random.Range(0f, 1f) < 0.8f)
                GameManager.gm.map.AddItemOnTile(1, GetComponent<Transform>().position);
                */

            // Small potion
            for (int i = 0; i < 2; i++)
            {
                if (Random.Range(0f, 1f) < 0.8f)
                    GameManager.gm.map.AddItemOnTile(2, GetComponent<Transform>().position);
            }

            // Random orb (lv.1 ~ lv.2)
            float r = Random.Range(0f, 1f);
            if (r < 0.8f)
            {
                GameManager.gm.map.AddItemOnTile(3, GetComponent<Transform>().position);
            }
            else
            {
                GameManager.gm.map.AddItemOnTile(4, GetComponent<Transform>().position);
            }

            foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
                sr.enabled = false;

            foreach (Image i in GetComponentsInChildren<Image>())
                i.enabled = false;
            
            GameManager.gm.map.SetEntityOnTile(null, GetComponent<Transform>().position);
        }
    }
}
