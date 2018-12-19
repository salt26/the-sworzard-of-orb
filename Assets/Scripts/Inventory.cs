using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    public int maxItemNumber;   // 보관 가능한 최대 아이템 수

    // TODO 나중에 inspector에서 안 보이게 하기
    [SerializeField]
    private List<string> items;
    
    private int gold = 0;

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
            return items;
        }
    }

    void Start()
    {
        itemImages = new List<KeyValuePair<string, GameObject>>();
        foreach (Button b in itemButtons)
        { 
            b.onClick.AddListener(delegate { UseItem(itemButtons.IndexOf(b)); });
        }
    }

    /// <summary>
    /// 인벤토리에 들어 있던 모든 아이템을 잃습니다.
    /// </summary>
    public void RemoveAllItems()
    {
        items = new List<string>();
        UpdateUI();
    }

    /// <summary>
    /// 인벤토리에 아이템을 넣습니다.
    /// 최대 보관 가능 개수를 초과하면 아이템을 더 넣지 않습니다.
    /// item이 null이거나 ItemManager에 등록되지 않은 아이템이면 넣지 않습니다.
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(string item)
    {
        if (items.Count >= maxItemNumber)
        {
            // TODO 인벤토리가 가득 찬 경우 처리하기
            Debug.Log("Inventory is full!");
            return;
        }
        if (item == null)
        {
            Debug.LogWarning("Item is null.");
            return;
        }
        if (!ItemManager.im.IsRegisteredItem(item))
        {
            Debug.LogWarning("Unregistered item.");
            return;
        }
        items.Add(item);
        UpdateUI();
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
                p.Value.GetComponent<Image>().sprite = ItemManager.im.GetItemPrefab(p.Key).GetComponent<Item>().inventorySprite;
                itemImages.Insert(i, p);
            }
            else if (items[i] != null && !items[i].Equals(itemImages[i].Key))
            {
                Destroy(itemImages[i].Value);
                itemImages.RemoveAt(i);
                KeyValuePair<string, GameObject> p =
                    new KeyValuePair<string, GameObject>(items[i],
                    Instantiate(itemImage, itemButtons[i].GetComponent<Transform>()));
                p.Value.GetComponent<Image>().sprite = ItemManager.im.GetItemPrefab(p.Key).GetComponent<Item>().inventorySprite;
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
    /// 인벤토리의 index 위치에 들어 있는 아이템을 사용합니다.
    /// 사용한 아이템은 사라지며, 플레이어의 턴이 상대에게 넘어갑니다.
    /// </summary>
    /// <param name="index"></param>
    public void UseItem(int index)
    {
        GameManager gm = GameManager.gm;
        if (gm == null) return;
        else if (gm.Turn == 0 && gm.IsSceneLoaded && !GetComponent<Mover>().IsMoving)
        {
            if (index < Items.Count)
            {
                if (ItemManager.im.GetItemPrefab(items[index]).GetComponent<Item>().Use())
                {
                    Debug.Log("Use item " + index + " from inventory.");
                    items.RemoveAt(index);
                    UpdateUI();
                    gm.NextTurn();
                }
            }
        }
    }

    /// <summary>
    /// 인벤토리의 index 위치에 들어 있는 아이템을 버립니다.
    /// </summary>
    /// <param name="index"></param>
    public void RemoveItem(int index)
    {
        if (index < Items.Count)
        {
            Debug.Log("Remove item " + index + " from inventory.");
            items.RemoveAt(index);
            UpdateUI();
        }
    }

    public void RemoveItem(Button itemButton)
    {
        int i = itemButtons.IndexOf(itemButton);
        if (i >= 0)
        {
            RemoveItem(i);
        }
    }
}
