using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Treasure : Interactable {

    public GameObject canvas;

    private bool hasinteracted = false;

    public override void Interact()
    {
        if (hasinteracted) return;
        
        hasinteracted = true;
        if (Random.Range(0, 3) == 0)
        {
            canvas.SetActive(true);
            GetComponent<BoxCollider>().enabled = true;
            GetComponent<EnemyMover>().enabled = true;
            GetComponent<Character>().enabled = true;
            GetComponent<Character>().statusUI = GameManager.gm.Canvas.GetComponent<UIInfo>().enemyStatusUI;
            GetComponent<Character>().level = GameManager.gm.mapLevel[GameManager.gm.map.mapName];
            GameManager.gm.interactables.Remove(this);
            GameManager.gm.enemies.Add(GetComponent<Character>());
            GameManager.gm.map.SetEntityOnTile(GetComponent<Character>(), GetComponent<Transform>().position);
        }
        else
        {
            GameManager.gm.map.AddGoldOnTile(5, GetComponent<Transform>().position);

            // Large potion
            if (Random.Range(0f, 1f) < 0.8f)
                GameManager.gm.map.AddItemOnTile(1, GetComponent<Transform>().position);

            // Small potion
            for (int i = 0; i < 3; i++)
            {
                if (Random.Range(0f, 1f) < 0.5f)
                    GameManager.gm.map.AddItemOnTile(2, GetComponent<Transform>().position);
            }

            // An orb (lv.1 ~ lv.2)
            float r = Random.Range(0f, 1f);
            if (r < 0.2f)
            {
                GameManager.gm.map.AddItemOnTile(100, GetComponent<Transform>().position);
            }
            else if (r < 0.4f)
            {
                GameManager.gm.map.AddItemOnTile(101, GetComponent<Transform>().position);
            }
            else if(r < 0.6f)
            {
                GameManager.gm.map.AddItemOnTile(102, GetComponent<Transform>().position);
            }
            else if (r < 0.8f)
            {
                GameManager.gm.map.AddItemOnTile(103, GetComponent<Transform>().position);
            }
            else if (r < 0.82f)
            {
                GameManager.gm.map.AddItemOnTile(104, GetComponent<Transform>().position);
            }
            else if (r < 0.84f)
            {
                GameManager.gm.map.AddItemOnTile(105, GetComponent<Transform>().position);
            }
            else if (r < 0.86f)
            {
                GameManager.gm.map.AddItemOnTile(106, GetComponent<Transform>().position);
            }
            else if (r < 0.88f)
            {
                GameManager.gm.map.AddItemOnTile(107, GetComponent<Transform>().position);
            }
            else if (r < 0.9f)
            {
                GameManager.gm.map.AddItemOnTile(108, GetComponent<Transform>().position);
            }
            else if (r < 0.92f)
            {
                GameManager.gm.map.AddItemOnTile(109, GetComponent<Transform>().position);
            }
            else if (r < 0.94f)
            {
                GameManager.gm.map.AddItemOnTile(110, GetComponent<Transform>().position);
            }
            else if (r < 0.96f)
            {
                GameManager.gm.map.AddItemOnTile(111, GetComponent<Transform>().position);
            }
            else if (r < 0.98f)
            {
                GameManager.gm.map.AddItemOnTile(112, GetComponent<Transform>().position);
            }
            else
            {
                GameManager.gm.map.AddItemOnTile(113, GetComponent<Transform>().position);
            }

            foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
                sr.enabled = false;

            foreach (Image i in GetComponentsInChildren<Image>())
                i.enabled = false;
            
            GameManager.gm.map.SetEntityOnTile(null, GetComponent<Transform>().position);
        }
    }
}
