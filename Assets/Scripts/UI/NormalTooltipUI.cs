using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NormalTooltipUI : TooltipUI
{

    public Text text;

    [HideInInspector]
    public string content = "";
    public int param = 0;

    void Awake()
    {
        isDisappearing = false;
        time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (time > 0f && Time.time >= time + 8f)
        {
            Disappear();
        }
        text.text = StringManager.sm.Translate(content).Replace("@", param.ToString());
    }
}
