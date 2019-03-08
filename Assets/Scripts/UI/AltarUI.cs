using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AltarUI : MonoBehaviour {

    public GameObject itemImage;            // 아이템 이미지 프리팹
    public List<Button> altarButtons;
    public Button combineButton;
    public List<GameObject> circlePartImages;
    public GameObject whitePanel;
    public List<GameObject> recipePanels;
    public AudioClip orbMakingSound;


    /// <summary>
    /// Key는 오브가 들어있는 가방의 위치, Value는 오브 이름
    /// </summary>
    private List<KeyValuePair<int, string>> orbs = new List<KeyValuePair<int, string>>();
    private List<KeyValuePair<string, GameObject>> orbImages = new List<KeyValuePair<string, GameObject>>();
    private AudioSource audioSource;

    public List<KeyValuePair<int, string>> Orbs
    {
        get
        {
            return orbs.Clone();
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        foreach (Button b in altarButtons)
        {
            b.onClick.AddListener(delegate { RemoveOrb(altarButtons.IndexOf(b)); });
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !whitePanel.activeInHierarchy)
        {
            EscapeAltar();
        }
    }

    public void EscapeAltar()
    {
        if (whitePanel.activeInHierarchy) return;
        GameManager.gm.NextTurnFromAltar();
        orbs = new List<KeyValuePair<int, string>>();
        UpdateUI();
        for (int i = 0; i < 3; i++)
        {
            circlePartImages[i].GetComponent<Image>().color = ColorManager.ChangeAlpha(Color.black, 0f);
        }
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
        for (i = 0; i < 3; i++)
        {
            if (i < orbs.Count)
            {
                StartCoroutine(CircleOnAnimation(i));
            }
            else if (i >= orbs.Count)
            {
                StartCoroutine(CircleOffAnimation(i));
            }
        }

        int orbLevel = 0;
        for (i = 0; i < orbs.Count; i++)
        {
            int l = ItemManager.im.FindItemInfo(Orbs[i].Value).level;
            if (orbLevel == 0) {
                orbLevel = l;
            }
            else if (orbLevel != l)
            {
                orbLevel = -1;
                break;
            }
        }
        if (orbLevel == 1)
        {
            recipePanels[0].SetActive(true);
            recipePanels[1].SetActive(false);
        }
        else if (orbLevel == 2)
        {
            recipePanels[0].SetActive(false);
            recipePanels[1].SetActive(true);
        }
        else
        {
            recipePanels[0].SetActive(false);
            recipePanels[1].SetActive(false);
        }
    }

    IEnumerator CircleOnAnimation(int index)
    {
        circlePartImages[index].SetActive(true);
        Color end = ColorManager.ExtractRepresentative(orbImages[index].Value.GetComponent<Image>().mainTexture as Texture2D);
        float frame = 40f;
        for (int i = 0; i < frame; i++)
        {
            circlePartImages[index].GetComponent<Image>().color = Color.Lerp(circlePartImages[index].GetComponent<Image>().color, end, i / frame);
            yield return null;
            if (index >= orbs.Count) yield break;
        }
        circlePartImages[index].GetComponent<Image>().color = end;
    }

    IEnumerator CircleOffAnimation(int index)
    {
        Color end = ColorManager.ChangeAlpha(Color.black, 0f);
        Color start = circlePartImages[index].GetComponent<Image>().color;
        float frame = 10f;
        for (int i = (int)(circlePartImages[index].GetComponent<Image>().color.a * frame); i >= 0; i--)
        {
            circlePartImages[index].GetComponent<Image>().color = Color.Lerp(end, start, i / frame);
            yield return null;
            if (index < orbs.Count) yield break;
        }
        circlePartImages[index].GetComponent<Image>().color = end;
        circlePartImages[index].SetActive(false);
    }

    /// <summary>
    /// 제단에 오브를 바치고 true를 반환합니다.
    /// 이미 바친 오브이면 제단에서 제거하고 true를 반환합니다.
    /// 최대로 바칠 수 있는 개수를 초과하면 오브를 받지 않고 false를 반환합니다.
    /// orb가 null이거나 ItemManager에 등록되지 않은 오브이면 받지 않고 true를 반환합니다.
    /// </summary>
    /// <param name="orb"></param>
    public bool AddOrb(string orb, int inventoryIndex)
    {
        if (whitePanel.activeInHierarchy) return true;
        if (IsContainOrbIndex(inventoryIndex))
        {
            RemoveOrb(orbs.FindIndex(p => p.Key == inventoryIndex));
            return true;
        }
        if (orbs.Count >= 3)
        {
            Debug.Log("Altar is full!");
            GameManager.gm.Canvas.GetComponent<UIInfo>().notiPanel.GetComponent<NotiUI>().SetNotiText("Altar is full!");
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
        if (whitePanel.activeInHierarchy) return;
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
            combineButton.GetComponent<Image>().sprite = ItemManager.im.GetItemSprite(id);
            combineButton.interactable = true;
        }
        return b;
    }

    /// <summary>
    /// 제단에 바친 세 오브를 조합하여 새로운 오브를 만듭니다.
    /// 재료로 쓰인 오브는 사라지고 새 오브가 가방에 추가됩니다.
    /// </summary>
    public void CombineOrb()
    {
        if (whitePanel.activeInHierarchy) return;
        if (!CheckCombinableness()) return;
        int id = ItemManager.im.FindOrbCombResultID(orbs[0].Value, orbs[1].Value, orbs[2].Value);
        Inventory inv = GameManager.gm.player.GetComponent<Inventory>();
        inv.RemoveItem(new List<int>() { orbs[0].Key, orbs[1].Key, orbs[2].Key });
        inv.AddItem(ItemManager.im.FindItemInfo(id).name);
        orbs = new List<KeyValuePair<int, string>>();
        UpdateUI();

        // TODO 조합 성공 이펙트 (화면 깜빡임)
        audioSource.clip = orbMakingSound;
        audioSource.Play();
        StartCoroutine(WhiteAnimation());
    }

    IEnumerator WhiteAnimation()
    {
        whitePanel.SetActive(true);
        whitePanel.GetComponent<Image>().color = ColorManager.ChangeAlpha(whitePanel.GetComponent<Image>().color, 0f);
        float frame = 12f;
        for (int i = 0; i < frame; i++)
        {
            whitePanel.GetComponent<Image>().color = Color.Lerp(ColorManager.ChangeAlpha(whitePanel.GetComponent<Image>().color, 0f),
                ColorManager.ChangeAlpha(whitePanel.GetComponent<Image>().color, 0.5f), i / frame);
            yield return null;
        }
        for (int i = 0; i < frame / 4f; i++)
        {
            whitePanel.GetComponent<Image>().color = Color.Lerp(ColorManager.ChangeAlpha(whitePanel.GetComponent<Image>().color, 0.5f),
                ColorManager.ChangeAlpha(whitePanel.GetComponent<Image>().color, 0f), i * 4 / frame);
            yield return null;
        }

        whitePanel.GetComponent<Image>().color = ColorManager.ChangeAlpha(whitePanel.GetComponent<Image>().color, 0f);
        whitePanel.SetActive(false);

        combineButton.interactable = false;
    }

    /// <summary>
    /// 가방의 index 위치에 놓인 오브가 제단에 바쳐져 있으면 true를 반환합니다.
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
