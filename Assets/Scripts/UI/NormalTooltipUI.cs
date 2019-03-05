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

    // Update is called once per frame
    void Update()
    {
        text.text = StringManager.sm.Translate(content).Replace("@", param.ToString());
    }
}
