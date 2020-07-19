using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/Blink")]
public class Blink : CardAction
{
    public int blinkRange;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) {
        List<Vector3Int> blinkTiles = GridHelper.TilesInDistance(pm.Coords(), blinkRange);
        foreach (var tile in blinkTiles) {
            if (GridHelper.CanStandOn(tile)) {
                GridHelper.groundTilemap.SetTileFlags(tile, UnityEngine.Tilemaps.TileFlags.None);
                GridHelper.groundTilemap.SetColor(tile, GridHelper.standableTileColor);
            }
        }

        Vector3Int newPosition = new Vector3Int();
        bool moved = false;
        while (true) {
            if (Input.GetMouseButtonDown(0)) {
                Vector3Int mouseCoords = pm.MouseCoords();

                for(int i = 0; i < blinkTiles.Count; ++i) {
                    var tile = blinkTiles[i];
                    GridHelper.groundTilemap.SetColor(tile, Color.white);
                    if (GridHelper.CanStandOn(tile) && tile == mouseCoords) {
                        newPosition = tile;
                        moved = true;
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

        yield return null;
    }
}
