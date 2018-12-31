﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataReader : MonoBehaviour {

	// Read Data and parse it to initialize managers.
	void Awake ()
    {
        ItemManager im = GetComponent<ItemManager>();
        EnemyManager em = GetComponent<EnemyManager>();
        MapManager mm = GetComponent<MapManager>();
        im.itemInfo = new Dictionary<int, ItemInfo>();
        em.enemyInfo = new Dictionary<int, EnemyInfo>();
        mm.mapInfo = new Dictionary<int, MapInfo>();

        TextAsset itemData = Resources.Load("Data/Item") as TextAsset;
        TextAsset orbData = Resources.Load("Data/Orb") as TextAsset;
        TextAsset monsterData = Resources.Load("Data/Monster") as TextAsset;
        TextAsset mapData = Resources.Load("Data/Map") as TextAsset;
        
        string[] itemLine = itemData.text.Split('\n');
        string[] orbLine = orbData.text.Split('\n');
        string[] monsterLine = monsterData.text.Split('\n');
        string[] mapLine = mapData.text.Split('\n');
        
        #region Parse Item.txt
        foreach (string l in itemLine)
        {
            if (l.StartsWith("#")) continue;
            string[] token = l.Split(' ');

            if (token.Length != 4 && token.Length != 6) Error("Item");

            ItemInfo ii = new ItemInfo();

            // token[0] : id
            ii.name = StringUtility.ReplaceUnderbar(token[1]);
            ii.type = ItemInfo.Type.Consumable;
            ii.tooltip = StringUtility.ReplaceUnderbar(token[2]);
            ii.price = int.Parse(token[3]);

            if (token.Length == 6)
            {
                ii.effectName = StringUtility.ToPascalCase(token[4]);
                ii.effectParam = int.Parse(token[5]);
            }
            im.itemInfo.Add(int.Parse(token[0]), ii);
        }
        #endregion

        #region Parse Orb.txt
        foreach (string l in orbLine)
        {
            if (l.StartsWith("#")) continue;
            string[] token = l.Split(' ');

            if (token.Length != 8 && token.Length != 10) Error("Orb");

            ItemInfo ii = new ItemInfo();

            // token[0] : id
            ii.name = StringUtility.ReplaceUnderbar(token[1]);
            ii.type = ItemInfo.Type.Orb;
            ii.tooltip = StringUtility.ReplaceUnderbar(token[2]);
            ii.level = int.Parse(token[3]);
            ii.stat = new Element(int.Parse(token[4]), int.Parse(token[5]), int.Parse(token[6]));
            ii.price = int.Parse(token[7]);

            if (token.Length == 10)
            {
                ii.effectName = StringUtility.ToPascalCase(token[8]);
                ii.effectParam = int.Parse(token[9]);
            }
            im.itemInfo.Add(int.Parse(token[0]), ii);
        }
        #endregion

        #region Parse Monster.txt
        int multiLine = -1;
        int enemyID = 0;
        EnemyInfo ei = new EnemyInfo();
        foreach (string l in monsterLine)
        {
            if (l.StartsWith("#")) continue;
            else if (l.StartsWith("|")) multiLine++;
            else if (multiLine == -1) multiLine = 0;
            else Error("Monster");

            string[] token = l.Split(' ');

            if (multiLine == 0)
            {
                if (token.Length != 5) Error("Monster");

                ei = new EnemyInfo();

                enemyID = int.Parse(token[0]);
                ei.name = StringUtility.ReplaceUnderbar(token[1]);
                switch (int.Parse(token[2]))
                {
                    case 0:
                        ei.type = EnemyInfo.Type.Normal;
                        break;
                    case 1:
                        ei.type = EnemyInfo.Type.Elite;
                        break;
                    case 2:
                        ei.type = EnemyInfo.Type.Boss;
                        break;
                    default:
                        ei.type = EnemyInfo.Type.Normal;
                        break;
                }
                ei.size = int.Parse(token[3]);
                ei.maxHealth = int.Parse(token[4]);
            }
            else if (multiLine == 1)
            {
                if (token.Length != 9) Error("Monster");

                // token[0] : '|'
                Weapon w = new Weapon();
                w.name = StringUtility.ReplaceUnderbar(token[1]);
                w.element = new Element(int.Parse(token[2]), int.Parse(token[3]), int.Parse(token[4]));
                ei.weaponDelta = new Element(int.Parse(token[5]), int.Parse(token[6]), int.Parse(token[7]));
                w.range = int.Parse(token[8]);

                ei.weapon = w;
            }
            else if (multiLine == 2)
            {
                if (token.Length != 7) Error("Monster");

                // token[0] : '|'
                Armor a = new Armor();
                a.element = new Element(int.Parse(token[1]), int.Parse(token[2]), int.Parse(token[3]));
                ei.armorDelta = new Element(int.Parse(token[4]), int.Parse(token[5]), int.Parse(token[6]));

                ei.armor = a;
            }
            else if (multiLine == 3)
            {
                if (token.Length != 5) Error("Monster");

                // token[0] : '|'
                switch (int.Parse(token[1]))
                {
                    case 0:
                        ei.distanceType = EnemyMover.DistanceType.Manhattan;
                        break;
                    case 1:
                        ei.distanceType = EnemyMover.DistanceType.Chebyshev;
                        break;
                    default:
                        ei.distanceType = EnemyMover.DistanceType.None;
                        break;
                }
                ei.sightDistance = int.Parse(token[2]);
                ei.leaveDistance = int.Parse(token[3]);
                ei.gold = int.Parse(token[4]);
            }
            else if (multiLine == 4)
            {
                if (token.Length % 4 != 0) Error("Monster");

                // token[0] : '|'

                ei.dropItems = new List<EnemyDropItemInfo>();
                EnemyDropItemInfo di = new EnemyDropItemInfo();
                for (int i = 0; i < token.Length; i++)
                {
                    switch (i % 4)
                    {
                        case 1:
                            di = new EnemyDropItemInfo();
                            di.itemID = int.Parse(token[i]);
                            break;
                        case 2:
                            di.count = int.Parse(token[i]);
                            break;
                        case 3:
                            di.probability = float.Parse(token[i]);

                            // di를 deep copy하여 ei.dropItems에 추가 (C# 3.0 문법)
                            ei.dropItems.Add(di.Clone());
                            break;
                    }
                }

                // ei를 deep copy하여 em.enemyInfo에 추가 (C# 3.0 문법)
                em.enemyInfo.Add(enemyID, ei.Clone());
                multiLine = -1;
            }
            
        }
        #endregion

        #region Parse Map.txt
        foreach (string l in mapLine)
        {
            if (l.StartsWith("#")) continue;
            string[] token = l.Split(' ');

            MapInfo mi = new MapInfo();

            // token[0] : id
            mi.name = StringUtility.ReplaceUnderbar(token[1]);
            // token[2] : width
            // token[3] : height
            mi.backgroundColor = new Color(float.Parse(token[4]), float.Parse(token[5]), float.Parse(token[6]));

            if (!token[7].Equals("|")) Error("Map");

            int i = 8;
            for (i = 8; i < token.Length; i++)
            {
                if (token[i].Equals("|")) break;
                mi.tilePrefab = new List<GameObject>();
                MapTile mt = GetComponent<TileManager>().tiles[int.Parse(token[i])];
                if (mt != null)
                    mi.tilePrefab.Add(mt.gameObject);
            }

            for (i = i + 1; i < token.Length; i++)
            {
                if (token[i].Equals("|")) break;
                mi.enemiesID.Add(int.Parse(token[i]));
            }

            for (i = i + 1; i < token.Length; i++)
            {
                mi.interactablesID.Add(int.Parse(token[i]));
            }

            mm.mapInfo.Add(int.Parse(token[0]), mi);
        }
        #endregion
        
    }

    private void Error(string data)
    {
        Debug.LogError("Data " + data + ".txt cannot be read!");
    }
}