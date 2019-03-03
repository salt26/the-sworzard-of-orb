using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : Interactable {

    public string sceneName;
    public string mapName;

    public override void Interact(bool isCharge = false)
    {
        if (mapName != null && mapName.Equals("")) mapName = null;

        if (mapName == null && GameManager.gm.MonsterNumber > 0 && isCharge)
        {
            // 전장에서 포탈에 돌진 상호작용한 경우
            GameManager.gm.MessageTurn("Charge interaction",
                "Do you intend to escape to Town? If so, click 'Yes'. If you click 'No', you can stay here.",
                delegate { GameManager.gm.ChangeScene(sceneName, mapName); }, null, null);
        }
        else if (mapName == null && GameManager.gm.MonsterNumber > 0 && !GameManager.gm.hasIgnoreReturnMessage)
        {
            // 
            GameManager.gm.MessageTurn("Escape to Town?",
                "You had better hunt monsters as many as possible. If you click 'Yes', you'll move to Town. If you click 'No', you can stay here.",
                delegate { GameManager.gm.ChangeScene(sceneName, mapName); }, null,
                (value) => GameManager.gm.hasIgnoreReturnMessage = value);
        }
        else if (SceneManager.GetActiveScene().name.Equals("Town") && GameManager.gm.mapLevel > 1 &&
            !GameManager.gm.hasShopVisited && !GameManager.gm.hasIgnoreShopMessage)
        {
            GameManager.gm.MessageTurn("Visit our shop!",
                "You can drop in at our shop and buy something new. If you want to leave without visiting our shop, click 'No'. (From Shop Host)",
                null, delegate { GameManager.gm.ChangeScene(sceneName, mapName); },
                (value) => GameManager.gm.hasIgnoreShopMessage = value);
        }
        else
        {
            GameManager.gm.ChangeScene(sceneName, mapName);
        }
    }
}
