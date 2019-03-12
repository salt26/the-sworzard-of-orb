using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    public int maxItemNumber;   // 보관 가능한 최대 아이템 수
    
    private List<string> items = new List<string>();
    
    private int gold = 0;

    public AudioClip getItemSound;
    private AudioSource audioSource;

    [HideInInspector]
    public Text goldText;

    [HideInInspector]
    public List<Button> itemButtons;    // UI의 각 버튼에 대한 레퍼런스

    public GameObject itemImage;            // 아이템 이미지 프리팹

    private List<KeyValuePair<string, GameObject>> itemImages;  // UI의 각 버튼 위에 표시되는 아이템 이미지들을 관리

    public int Gold
    {
        get
        {
            return gold;
        }
        set
        {
            gold = value;
            goldText.text = gold.ToString();
        }
    }

    public List<string> Items
    {
        get
        {
            return items.Clone();
        }
    }

    void Start()
    {
        items = GameManager.gm.GetComponent<DataReader>().playerItems;
        Gold = GameManager.gm.GetComponent<DataReader>().playerGold;
        audioSource = GetComponent<AudioSource>();
        itemImages = new List<KeyValuePair<string, GameObject>>();
        foreach (Button b in itemButtons)
        { 
            b.onClick.AddListener(delegate { UseItem(itemButtons.IndexOf(b)); });
            b.onClick.AddListener(delegate { DedicateItem(itemButtons.IndexOf(b)); });
            b.onClick.AddListener(delegate { SellItem(itemButtons.IndexOf(b)); });
        }
        UpdateUI();
    }

    /// <summary>
    /// 가방에 들어 있던 모든 아이템을 잃습니다.
    /// </summary>
    public void RemoveAllItems()
    {
        items = new List<string>();
        UpdateUI();
    }

    /// <summary>
    /// 가방에 아이템을 넣고 true를 반환합니다.
    /// 최대 보관 가능 개수를 초과하면 아이템을 더 넣지 않고 false를 반환합니다.
    /// item이 null이거나 ItemManager에 등록되지 않은 아이템이면 넣지 않고 true를 반환합니다.
    /// </summary>
    /// <param name="item"></param>
    public bool AddItem(string item)
    {
        audioSource.clip = getItemSound;
        audioSource.Play();
        // 만약 이 메서드가 false를 반환하는 경우가 추가된다면, ItemEffect의 TransformWithNoEffect도 바꾸기!
        if (items.Count >= maxItemNumber)
        {
            // TODO 가방이 가득 찬 경우 처리하기
            Debug.Log("Inventory is full!");
            GameManager.gm.Canvas.GetComponent<UIInfo>().notiPanel.GetComponent<NotiUI>().SetNotiText("Inventory is full!");
            return false;
        }
        if (item == null)
        {
            Debug.LogWarning("Item is null.");
            return true;
        }
        if (ItemManager.im.FindItemInfo(item) == null)
        {
            Debug.LogWarning("Unregistered item.");
            return true;
        }
        items.Add(item);
        UpdateUI();
        return true;
    }

    /// <summary>
    /// 현재 items 목록을 기준으로 UI에 보여지는 아이템 이미지들을 업데이트합니다.
    /// </summary>
    private void UpdateUI()
    {
        int i;
        for (i = 0; i < items.Count; i++)
        {
            if (i == itemImages.Count)
            {
                KeyValuePair<string, GameObject> p =
                    new KeyValuePair<string, GameObject>(items[i],
                    Instantiate(itemImage, itemButtons[i].GetComponent<Transform>()));
                p.Value.GetComponent<Image>().sprite = ItemManager.im.GetItemSprite(p.Key);
                itemImages.Insert(i, p);
            }
            else if (items[i] != null && !items[i].Equals(itemImages[i].Key))
            {
                Destroy(itemImages[i].Value);
                itemImages.RemoveAt(i);
                KeyValuePair<string, GameObject> p =
                    new KeyValuePair<string, GameObject>(items[i],
                    Instantiate(itemImage, itemButtons[i].GetComponent<Transform>()));
                p.Value.GetComponent<Image>().sprite = ItemManager.im.GetItemSprite(p.Key);
                itemImages.Insert(i, p);
            }
        }
        while (i < itemImages.Count)
        {
            Destroy(itemImages[i].Value);
            itemImages.RemoveAt(i);
        }
    }

    /// <summary>
    /// 가방의 index 위치에 들어 있는 아이템을 사용합니다.
    /// 사용한 아이템은 사라지며, 플레이어의 턴이 상대에게 넘어갑니다.
    /// 플레이어의 턴일 때에만 동작합니다.
    /// </summary>
    /// <param name="index"></param>
    public void UseItem(int index)
    {
        GameManager gm = GameManager.gm;
        if (gm == null) return;
        else if (gm.Turn == 0 && gm.IsSceneLoaded && !GetComponent<Mover>().IsMoving &&
            GetComponent<Character>().Alive && index < Items.Count)
        {
            if (ItemManager.im.FindItemInfo(Items[index]).Use(index))
            {
                if ("Transform".Equals(ItemManager.im.FindItemInfo(items[index]).effectName))
                {
                    items.RemoveAt(index);
                    string newOrb = Items[Items.Count - 1];
                    items.RemoveAt(Items.Count - 1);
                    items.Insert(index, newOrb);
                    GameManager.gm.SaveGame();
                }
                else
                {
                    items.RemoveAt(index);
                }
                UpdateUI();
                gm.NextTurn();
            }
        }
    }

    /// <summary>
    /// 가방의 index 위치에 들어 있는 아이템을 플레이어 위치에 드랍하고 턴을 넘깁니다.
    /// 플레이어의 턴이고 제단과 상호작용하지 않을 때에만 작동합니다.
    /// </summary>
    /// <param name="index"></param>
    public void DropItem(int index)
    {
        GameManager gm = GameManager.gm;
        if (gm == null) return;
        else if (gm.Turn == 0 && gm.IsSceneLoaded && !GetComponent<Mover>().IsMoving &&
            GetComponent<Character>().Alive && index < Items.Count)
        {
            GameManager.gm.Canvas.GetComponent<UIInfo>().notiPanel.GetComponent<NotiUI>().SetNotiText("Item dropped.");
            GameManager.gm.map.AddItemOnTile(ItemManager.im.FindItemID(Items[index]), GetComponent<Transform>().position);
            items.RemoveAt(index);
            UpdateUI();
            gm.NextTurn();
        }
    }

    /// <summary>
    /// 가방의 itemButton이 놓인 위치에 들어 있는 아이템을 플레이어 위치에 드랍하고 턴을 넘깁니다.
    /// 플레이어의 턴이고 제단과 상호작용하지 않을 때에만 작동합니다.
    /// </summary>
    /// <param name="itemButton"></param>
    public void DropItem(Button itemButton)
    {
        int i = itemButtons.IndexOf(itemButton);
        if (i >= 0)
        {
            DropItem(i);
        }
    }

    /// <summary>
    /// indices의 각 원소를 위치로 하여, 가방의 해당 위치에 들어 있는 아이템들을 삭제합니다.
    /// </summary>
    /// <param name="indices"></param>
    public void RemoveItem(List<int> indices)
    {
        /*
        GameManager gm = GameManager.gm;
        if (gm == null) return;
        if (gm.Turn == 3 && gm.IsSceneLoaded && indices != null)
        */
        if (indices != null)
        {
            indices.Sort();
            for (int i = indices.Count - 1; i >= 0; i--)
            {
                int index = indices[i];
                if (index < Items.Count)
                {
                    items.RemoveAt(index);
                }
            }
            UpdateUI();
        }
    }

    /// <summary>
    /// 가방의 index 위치에 있는 오브 아이템을 제단에 바칩니다.
    /// 제단과 상호작용하는 중에만 동작합니다.
    /// </summary>
    /// <param name="index"></param>
    public void DedicateItem(int index)
    {
        GameManager gm = GameManager.gm;
        if (gm == null) return;
        else if (gm.Turn == 3 && gm.IsSceneLoaded && index < Items.Count)
        {
            // 아이템이 제단에 올라가서, 제단 버튼 위에 Instantiate되어야 함
            // 그리고 가방의 이 버튼이 하이라이트되며 비활성화되어야 함 (이건 ClickItemButtonUI.cs에서)
            gm.Canvas.GetComponent<UIInfo>().altarPanel.GetComponent<AltarUI>().AddOrb(Items[index], index);
        }
    }

    /// <summary>
    /// 가방의 index 위치에 있는 아이템을 상점에 판매합니다.
    /// 상점과 상호작용하는 중에만 동작합니다.
    /// </summary>
    /// <param name="index"></param>
    public void SellItem(int index)
    {
        GameManager gm = GameManager.gm;
        if (gm == null) return;
        else if (gm.Turn == 4 && gm.IsSceneLoaded && index < Items.Count)
        {
            if (gm.Canvas.GetComponent<UIInfo>().shopPanel.GetComponent<ShopUI>().SellItem(Items[index]))
                RemoveItem(new List<int> { index });
        }
    }

    /// <summary>
    /// 아이템 버튼을 인자로 받아, 해당 버튼에 들어있는 아이템 name을 반환합니다.
    /// 아이템이 들어있지 않은 버튼이면 null을 반환합니다.
    /// </summary>
    /// <param name="itemButton"></param>
    /// <returns></returns>
    public string FindItemNameInButton(Button itemButton)
    {
        int i = itemButtons.IndexOf(itemButton);
        if (i >= 0 && i < Items.Count)
        {
            return Items[i];
        }
        return null;
    }
}
