using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/Charge")]
public class Charge : CardAction
{
    public int chargeRange;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) {
        Vector3Int[] adjTiles1 = GridHelper.GetAdjacentTiles(pm.Coords());
        for (int i = 0; i < 6; ++i) {
            if (GridHelper.CanStandOn(adjTiles1[i])) {
                Vector3Int cTile = GridHelper.GetChargeTile(pm.Coords(), i, chargeRange);
                GridHelper.groundTilemap.SetTileFlags(cTile, UnityEngine.Tilemaps.TileFlags.None);
                GridHelper.groundTilemap.SetColor(cTile, GridHelper.standableTileColor);
            }
        }

        bool moved = false;
        int acDist = 0;
        Vector3Int newPosition = new Vector3Int();
        while (true) {
            if (Input.GetMouseButtonDown(0)) {
                Vector3Int mouseCoords = pm.MouseCoords();

                Vector3Int[] adjTiles = GridHelper.GetAdjacentTiles(pm.Coords());
                for (int i = 0; i < 6; ++i) {
                    if (GridHelper.CanStandOn(adjTiles[i])) {
                        Vector3Int cTile = GridHelper.GetChargeTile(pm.Coords(), i, chargeRange, ref acDist);
                        GridHelper.groundTilemap.SetColor(cTile, Color.white);
                        if (GridHelper.CanStandOn(cTile) && cTile == mouseCoords) {
                            newPosition = cTile;
                            moved = true;
                        }
                    }
                }            
                break;
            }
            yield return null;
        }
        if (moved) {
            pm.MoveTo(newPosition);
            cae.cardPlayState[currentAction] = 1;
        }
        else {
            cae.cardPlayState[currentAction] = 2;
        }
    }   
}
