using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Interactable {

    public string sceneName;
    public Vector2Int startPosition;

    public override void Interact()
    {
        GameManager.gm.player.GetComponent<Transform>().position = new Vector3(startPosition.x, startPosition.y, GameManager.gm.player.GetComponent<Transform>().position.z);
        GameManager.gm.ChangeScene(sceneName);
    }
}
