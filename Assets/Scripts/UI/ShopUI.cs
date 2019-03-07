using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour {

    public GameObject itemImage;            // 아이템 이미지 프리팹
    public List<Button> purchaseButtons;
    public List<Button> repurchaseButtons;
    public Text repurchaseText;
    public Image hostImage;
    public List<Sprite> hostSprites;
    public GameObject bubbleImage;
    public Text bubbleText;

    /// <summary>
    /// 상점에서 판매하는 아이템 목록. index는 구입 버튼 위치.
    /// </summary>
    [HideInInspector]
    public List<string> purchaseItems = new List<string>();     // 아이템 목록은 GM의 ChangeScene()에서 초기화됨

    /// <summary>
    /// 재구매하는 아이템 목록. index는 재구매 버튼 위치.
    /// </summary>
    private List<string> repurchaseItems = new List<string>();

    /// <summary>
    /// 재구매하는 아이템의 이미지 목록. Key는 아이템 이름, Value는 이미지 오브젝트.
    /// </summary>
    private List<KeyValuePair<string, GameObject>> repurchaseItemImages = new List<KeyValuePair<string, GameObject>>();

    //private int currentHostFace = 0;    // 0: 무표정, 1: 말하는 중, 2: 웃음
    private bool stopSpeaking = false;
    private bool isSpeaking = false;
    private string originalBubbleString = "";

    public List<string> RepurchaseItems
    {
        get
        {
            return repurchaseItems.Clone();
        }
    }

    // Use this for initialization
    void Start () {
		
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EscapeShop();
        }
    }

    public void EnterShop()
    {
        StartCoroutine(Speech("Welcome!", 2));
    }

    public void EscapeShop()
    {
        GameManager.gm.NextTurnFromShop();
        repurchaseItems = new List<string>();
        originalBubbleString = "";
        bubbleText.text = "";
        hostImage.sprite = hostSprites[1];
        bubbleImage.SetActive(false);
        if (isSpeaking) stopSpeaking = true;
        else stopSpeaking = false;
        isSpeaking = false;
        UpdateRepurchaseUI();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 현재 repurchaseItems 목록을 기준으로 UI에 보여지는 재구매 아이템 이미지들을 업데이트합니다.
    /// </summary>
    private void UpdateRepurchaseUI()
    {
        int i;
        for (i = 0; i < repurchaseItems.Count; i++)
        {
            if (i == repurchaseItemImages.Count)
            {
                KeyValuePair<string, GameObject> p =
                    new KeyValuePair<string, GameObject>(repurchaseItems[i],
                    Instantiate(itemImage, repurchaseButtons[i].GetComponent<Transform>()));
                p.Value.GetComponent<Image>().sprite = ItemManager.im.GetItemSprite(p.Key);
                repurchaseItemImages.Insert(i, p);
            }
            else if (repurchaseItems[i] != null && !repurchaseItems[i].Equals(repurchaseItemImages[i].Key))
            {
                Destroy(repurchaseItemImages[i].Value);
                repurchaseItemImages.RemoveAt(i);
                KeyValuePair<string, GameObject> p =
                    new KeyValuePair<string, GameObject>(repurchaseItems[i],
                    Instantiate(itemImage, repurchaseButtons[i].GetComponent<Transform>()));
                p.Value.GetComponent<Image>().sprite = ItemManager.im.GetItemSprite(p.Key);
                repurchaseItemImages.Insert(i, p);
            }
        }
        while (i < repurchaseItemImages.Count)
        {
            Destroy(repurchaseItemImages[i].Value);
            repurchaseItemImages.RemoveAt(i);
        }
    }

    /// <summary>
    /// 가방에서 아이템을 상점에 판매하고 true를 반환합니다.
    /// 판매한 아이템은 재구매 목록에 추가됩니다.
    /// 만약 item이 null이거나 ItemManager에 등록되지 않았으면 판매하지 않고 false를 반환합니다.
    /// </summary>
    /// <param name="item"></param>
    public bool SellItem(string item)
    {
        // 만약 재구매 목록이 꽉 차 있으면 추가하기 전에 하나 비우고
        // 재구매 목록에 추가하고
        // 판매 금액만큼 받고
        if (item == null)
        {
            Debug.LogWarning("Item is null.");
            return false;
        }
        if (ItemManager.im.FindItemInfo(item) == null)
        {
            Debug.LogWarning("Unregistered item.");
            return false;
        }
        if (repurchaseItems.Count == repurchaseButtons.Count)
        {
            repurchaseItems.RemoveAt(0);
        }
        repurchaseItems.Add(item);
        GameManager.gm.player.GetComponent<Inventory>().Gold += ItemManager.im.FindItemInfo(item).SellPrice;
        UpdateRepurchaseUI();
        StartCoroutine(Speech("Thank you. I'll take it.", 2));
        return true;
    }

    public void RepurchaseItem(int index)
    {
        if (index >= repurchaseItems.Count)
        {
            return;
        }
        Inventory inv = GameManager.gm.player.GetComponent<Inventory>();
        if (inv.Items.Count >= inv.maxItemNumber)
        {
            Debug.Log("Inventory is full!");
            StartCoroutine(Speech("Please check if your inventory is full.", 0));
            return;
        }
        if (repurchaseItems[index] == null)
        {
            Debug.LogWarning("Item is null.");
            return;
        }
        ItemInfo ii = ItemManager.im.FindItemInfo(repurchaseItems[index]);
        if (ii == null)
        {
            Debug.LogWarning("Unregistered item.");
            return;
        }
        if (inv.Gold < ii.SellPrice)
        {
            Debug.Log("Not enough gold!");
            StartCoroutine(Speech("Not enough gold...", 0));
            return;
        }
        inv.Gold -= ii.SellPrice;
        inv.AddItem(ii.name);
        repurchaseItems.RemoveAt(index);
        StartCoroutine(Speech("I'll return it to you.", 1));
        UpdateRepurchaseUI();
    }

    public void PurchaseItem(int index)
    {
        if (index >= purchaseItems.Count)
        {
            return;
        }
        Inventory inv = GameManager.gm.player.GetComponent<Inventory>();

        if (inv.Items.Count >= inv.maxItemNumber)
        {
            Debug.Log("Inventory is full!");
            StartCoroutine(Speech("Please check if your inventory is full.", 0));
            return;
        }
        if (purchaseItems[index] == null)
        {
            Debug.LogWarning("Item is null.");
            return;
        }
        ItemInfo ii = ItemManager.im.FindItemInfo(purchaseItems[index]);
        if (ii == null)
        {
            Debug.LogWarning("Unregistered item.");
            return;
        }
        if (inv.Gold < ii.BuyPrice)
        {
            Debug.Log("Not enough gold!");
            StartCoroutine(Speech("Not enough gold...", 0));
            return;
        }
        inv.Gold -= ii.BuyPrice;
        inv.AddItem(ii.name);
        purchaseItems.RemoveAt(index);
        StartCoroutine(Speech("That would be useful for you.", 2));
        purchaseItems.Insert(index, null);
    }

    public void RefreshShopText()
    {
        repurchaseText.text = StringManager.sm.Translate("Repurchase");
        bubbleText.text = StringManager.sm.Translate(originalBubbleString);
    }

    IEnumerator Speech(string word, int startHostFace, int endHostFace = 1)
    {
        if (startHostFace < 0 || startHostFace >= hostSprites.Count || endHostFace < 0 || endHostFace >= hostSprites.Count)
            yield break;
        if (isSpeaking)
            stopSpeaking = true;
        yield return null;
        isSpeaking = true;

        originalBubbleString = word;
        bubbleText.text = StringManager.sm.Translate(originalBubbleString);
        if (!bubbleImage.activeInHierarchy)
        {
            bubbleImage.SetActive(true);
        }
        hostImage.sprite = hostSprites[startHostFace];
        float frame = 150f;

        for (int i = 0; i < frame; i++)
        {
            if (stopSpeaking)
            {
                stopSpeaking = false;
                break;
            }

            yield return null;
        }
        originalBubbleString = "";
        bubbleText.text = "";
        hostImage.sprite = hostSprites[endHostFace];
        bubbleImage.SetActive(false);
        isSpeaking = false;
    }
}
