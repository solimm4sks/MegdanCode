using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "CardCheck/MoreThan_HealthBlock")]
public class MoreThan_HealthBlock : CardCheck
{
    public int health = -1;
    public int block = -1;

    public override bool Check(PlayerManager pm) {
        return pm.playerInfo.health > health && pm.playerInfo.block > block;
    }
}
