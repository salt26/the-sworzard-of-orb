using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : Item {

    public int level;       // 오브 레벨
    public Element stat;

    public override bool Use()
    {
        // TODO 지금은 현재 장착한 무기에 스탯이 추가됨
        GameManager.gm.player.EquippedWeapon.element += stat;
        GameManager.gm.player.statusUI.UpdateAttackText(GameManager.gm.player.EquippedWeapon);
        return true;
    }
}
