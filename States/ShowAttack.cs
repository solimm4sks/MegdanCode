using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAttack : State
{
    public ShowAttack(PlayerManager pm) : base(pm) { }

    public override IEnumerator Start()
    {
        List<Vector3Int> tiles = playerManager.TilesInBasic();
        for (int i = 0; i < tiles.Count; ++i) 
            GridHelper.groundTilemap.SetColor(tiles[i], playerManager.atkRangeColor);
        yield break;
    }

    public override IEnumerator HideAttack()
    {
        List<Vector3Int> tiles = playerManager.TilesInBasic();
        for (int i = 0; i < tiles.Count; ++i) 
            GridHelper.groundTilemap.SetColor(tiles[i], Color.white);

        playerManager.SetState(new MyTurn(playerManager));
        yield break;
    }

}
