using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Interactable {

    public string sceneName;
    public string mapName;

    public override void Interact()
    {
        if (mapName == "") mapName = null;
        GameManager.gm.ChangeScene(sceneName, mapName);
    }
}
