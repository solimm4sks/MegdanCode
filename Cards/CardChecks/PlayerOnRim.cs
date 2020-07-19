using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardCheck/PlayerOnRim")]
public class PlayerOnRim : CardCheck
{
    public bool onRim;

    public override bool Check(PlayerManager pm) {
        return GridHelper.IsTileOnRim(pm.Coords()) == onRim;
    }
}
