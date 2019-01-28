using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AltarUI : MonoBehaviour {

    public GameObject itemImage;            // 아이템 이미지 프리팹
    public List<Button> altarButtons;
    public Button combineButton;

    /// <summary>
    /// Key는 오브가 들어있는 인벤토리의 위치, Value는 오브 이름
    /// </summary>
    private List<KeyValuePair<int, string>> orbs = new List<KeyValuePair<int, string>>();
    private List<KeyValuePair<string, GameObject>> orbImages = new List<KeyValuePair<string, GameObject>>();

    public List<KeyValuePair<int, string>> Orbs
    {
        get
        {
            return orbs.Clone();
        }
    }

    void Start()
    {
        foreach (Button b in altarButtons)
        {
            b.onClick.AddListener(delegate { RemoveOrb(altarButtons.IndexOf(b)); });
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EscapeAltar();
        }
    }

    public void EscapeAltar()
    {
        GameManager.gm.NextTurnFromAltar();
        orbs = new List<KeyValuePair<int, string>>();
        UpdateUI();
        combineButton.interactable = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 현재 orbs 목록을 기준으로 UI에 보여지는 아이템 이미지들을 업데이트합니다.
    /// </summary>
    private void UpdateUI()
    {
        int i;
        for (i = 0; i < orbs.Count; i++)
        {
            if (i == orbImages.Count)
            {
                KeyValuePair<string, GameObject> p =
                    new KeyValuePair<string, GameObject>(orbs[i].Value,
                    Instantiate(itemImage, altarButtons[i].GetComponent<Transform>()));
                p.Value.GetComponent<Image>().sprite = ItemManager.im.GetItemSprite(p.Key);
                orbImages.Insert(i, p);
            }
            else if (orbs[i].Value != null && !orbs[i].Equals(orbImages[i].Key))
            {
                Destroy(orbImages[i].Value);
                orbImages.RemoveAt(i);
                KeyValuePair<string, GameObject> p =
                    new KeyValuePair<string, GameObject>(orbs[i].Value,
                    Instantiate(itemImage, altarButtons[i].GetComponent<Transform>()));
                p.Value.GetComponent<Image>().sprite = ItemManager.im.GetItemSprite(p.Key);
                orbImages.Insert(i, p);
            }
        }
        while (i < orbImages.Count)
        {
            Destroy(orbImages[i].Value);
            orbImages.RemoveAt(i);
        }
    }

    /// <summary>
    /// 제단에 오브를 바치고 true를 반환합니다.
    /// 최대로 바칠 수 있는 개수를 초과하면 오브를 받지 않고 false를 반환합니다.
    /// orb가 null이거나 ItemManager에 등록되지 않은 오브이면 받지 않고 true를 반환합니다.
    /// </summary>
    /// <param name="orb"></param>
    public bool AddOrb(string orb, int inventoryIndex)
    {
        if (orbs.Count >= 3)
        {
            Debug.Log("Altar is full!");
            return false;
        }
        if (orb == null)
        {
            Debug.LogWarning("Orb is null.");
            return true;
        }
        if (ItemManager.im.FindItemInfo(orb) == null)
        {
            Debug.LogWarning("Unregistered item.");
            return true;
        }
        if (ItemManager.im.FindItemInfo(orb).type != ItemInfo.Type.Orb)
        {
            Debug.LogWarning("Item is not an orb.");
            return true;
        }
        if (IsContainOrbIndex(inventoryIndex))
        {
            Debug.LogWarning("This orb is already dedicated.");
            return true;
        }
        orbs.Add(new KeyValuePair<int, string>(inventoryIndex, orb));
        UpdateUI();

        if (orbs.Count == 3) CheckCombinableness();
        return true;
    }

    /// <summary>
    /// 제단의 index 위치에 바쳐진 오브를 뺍니다.
    /// </summary>
    /// <param name="index"></param>
    public void RemoveOrb(int index)
    {
        if (index < orbs.Count)
        {
            //Debug.Log("Remove orb " + index + " from altar.");
            orbs.RemoveAt(index);
            UpdateUI();
            if (combineButton.interactable) combineButton.interactable = false;
        }
    }

    /// <summary>
    /// 제단에 바쳐진 세 오브를 조합하여 새 오브를 만들 수 있는지 확인합니다.
    /// true를 반환할 때 조합하기 버튼을 활성화합니다.
    /// </summary>
    private bool CheckCombinableness()
    {
        int id = ItemManager.im.FindOrbCombResultID(orbs[0].Value, orbs[1].Value, orbs[2].Value);
        bool b = id >= 100;
        if (b)
        {
            combineButton.interactable = true;
            //Debug.Log(ItemManager.im.FindItemInfo(id).name);
        }
        return b;
    }

    /// <summary>
    /// 제단에 바친 세 오브를 조합하여 새로운 오브를 만듭니다.
    /// 재료로 쓰인 오브는 사라지고 새 오브가 인벤토리에 추가됩니다.
    /// </summary>
    public void CombineOrb()
    {
        if (!CheckCombinableness()) return;
        int id = ItemManager.im.FindOrbCombResultID(orbs[0].Value, orbs[1].Value, orbs[2].Value);
        Inventory inv = GameManager.gm.player.GetComponent<Inventory>();
        inv.RemoveItem(new List<int>() { orbs[0].Key, orbs[1].Key, orbs[2].Key });
        inv.AddItem(ItemManager.im.FindItemInfo(id).name);
        orbs = new List<KeyValuePair<int, string>>();
        UpdateUI();
        combineButton.interactable = false;

        // TODO 조합 성공 이펙트 (화면 깜빡임)
    }

    /// <summary>
    /// 인벤토리의 index 위치에 놓인 오브가 제단에 바쳐져 있으면 true를 반환합니다.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool IsContainOrbIndex(int index)
    {
        foreach (KeyValuePair<int, string> p in orbs)
        {
            if (p.Key == index) return true;
        }
        return false;
    }
}
