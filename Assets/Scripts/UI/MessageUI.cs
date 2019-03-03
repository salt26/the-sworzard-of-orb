using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour {

    public Button yesButton;
    public Button noButton;
    public Button okButton;
    public Text headerText;
    public Text bodyText;
    public Toggle showToggle;
    public Text showText;

    private bool isOKType;
    private string header;
    private string body;

    /// <summary>
    /// Yes 버튼과 No 버튼을 사용하는 메시지박스를 띄웁니다.
    /// header와 body에는 영어 원문을 넣어야 합니다.
    /// </summary>
    /// <param name="header"></param>
    /// <param name="body"></param>
    /// <param name="onYesClick"></param>
    /// <param name="onNoClick"></param>
    public void Initialize(string header, string body, 
        UnityEngine.Events.UnityAction onYesClick, UnityEngine.Events.UnityAction onNoClick,
        UnityEngine.Events.UnityAction<bool> onShowToggle = null)
    {
        isOKType = false;
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
        okButton.gameObject.SetActive(false);
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
        okButton.onClick.RemoveAllListeners();
        showToggle.onValueChanged.RemoveAllListeners();
        showToggle.isOn = false;

        if (onYesClick != null)
            yesButton.onClick.AddListener(onYesClick);

        yesButton.onClick.AddListener(() => {
            GameManager.gm.NextTurnFromMessage();
            gameObject.SetActive(false);
        });
        //yesButton.onClick.AddListener(delegate { UseItem(itemButtons.IndexOf(b)); });

        if (onNoClick != null)
            noButton.onClick.AddListener(onNoClick);

        noButton.onClick.AddListener(() => {
            GameManager.gm.NextTurnFromMessage();
            gameObject.SetActive(false);
        });

        if (onShowToggle != null)
        {
            showToggle.gameObject.SetActive(true);
            showToggle.onValueChanged.AddListener(onShowToggle);
        }
        else
        {
            showToggle.gameObject.SetActive(false);
        }

        this.header = header;
        this.body = body;
        RefreshMessageText();
    }

    /// <summary>
    /// OK 버튼을 사용하는 메시지박스를 띄웁니다.
    /// header와 body에는 영어 원문을 넣어야 합니다.
    /// </summary>
    /// <param name="header"></param>
    /// <param name="body"></param>
    /// <param name="onOKClick"></param>
    public void Initialize(string header, string body, UnityEngine.Events.UnityAction onOKClick = null,
        UnityEngine.Events.UnityAction<bool> onShowToggle = null)
    {
        isOKType = true;
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        okButton.gameObject.SetActive(true);
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
        okButton.onClick.RemoveAllListeners();
        showToggle.onValueChanged.RemoveAllListeners();
        showToggle.isOn = false;

        if (onOKClick != null)
            okButton.onClick.AddListener(onOKClick);

        okButton.onClick.AddListener(() => {
            GameManager.gm.NextTurnFromMessage();
            gameObject.SetActive(false);
        });
        
        if (onShowToggle != null)
        {
            showToggle.gameObject.SetActive(true);
            showToggle.onValueChanged.AddListener(onShowToggle);
        }
        else
        {
            showToggle.gameObject.SetActive(false);
        }

        this.header = header;
        this.body = body;
        RefreshMessageText();
    }

    public void RefreshMessageText()
    {
        yesButton.GetComponentInChildren<Text>().text = StringManager.sm.Translate("Yes");
        noButton.GetComponentInChildren<Text>().text = StringManager.sm.Translate("No");
        okButton.GetComponentInChildren<Text>().text = StringManager.sm.Translate("OK");
        headerText.text = StringManager.sm.Translate(header);
        bodyText.text = StringManager.sm.Translate(body);
        showText.text = StringManager.sm.Translate("Don't show this message.");
    }
}
