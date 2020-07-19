using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardCheck/CanBlink")]
public class CanBlink : CardCheck {

    public int range;
    private List<Vector3Int> blinkTiles;

    public override bool Check(PlayerManager pm) {
        if (!pm.CanMove())
            return false;

        blinkTiles = GridHelper.TilesInDistance(pm.Coords(), range);

        foreach (Vector3Int tile in blinkTiles) {
            if (GridHelper.CanStandOn(tile)) {
                return true;
            }
        }
        return false;
    }
}
