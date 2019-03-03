using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : Interactable {

    public string sceneName;
    public string mapName;

    public override void Interact()
    {
        if (mapName != null && mapName.Equals("")) mapName = null;

        if (mapName == null && GameManager.gm.MonsterNumber > 0 && !GameManager.gm.hasIgnoreReturnMessage)
        {
            GameManager.gm.MessageTurn("Escape to Town?",
                "If click 'Yes', you'll move to Town. If click 'No', you can stay here.",
                delegate { GameManager.gm.ChangeScene(sceneName, mapName); }, null,
                (value) => GameManager.gm.hasIgnoreReturnMessage = value);
        }
        else if (SceneManager.GetActiveScene().name.Equals("Town") && GameManager.gm.mapLevel > 1 &&
            !GameManager.gm.hasShopVisited && !GameManager.gm.hasIgnoreShopMessage)
        {
            GameManager.gm.MessageTurn("Visit our shop!",
                "You can drop in at our shop and buy something new. If you leave without visiting our shop, click 'No'. -From Shop Host-",
                null, delegate { GameManager.gm.ChangeScene(sceneName, mapName); },
                (value) => GameManager.gm.hasIgnoreShopMessage = value);
        }
        else
        {
            GameManager.gm.ChangeScene(sceneName, mapName);
        }
    }
}
