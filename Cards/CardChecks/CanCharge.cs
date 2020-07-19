using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardCheck/CanCharge")]
public class CanCharge : CardCheck
{

    public override bool Check(PlayerManager pm) {
        if(!pm.CanMove())
            return false;

        bool ok = false;
        Vector3Int[] adjTiles1 = GridHelper.GetAdjacentTiles(pm.Coords());
        for (int i = 0; i < 6; ++i) {
            bool cso = GridHelper.CanStandOn(adjTiles1[i]);
            ok |= cso;
        }
        return ok && pm.state.CanMove();
    }
}
