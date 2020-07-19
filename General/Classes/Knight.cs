using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Knight : PlayerManager {

    public int lastBasicMoveDir = 7;
    private List<GameObject> arrows = new List<GameObject>();

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < 6; ++i)
            arrows.Add(GameObject.Find(gameObject.name + "/FMArrows/Arrow" + i));
    }

    protected override void SetClassProperties() {
        playerInfo.basicRange = 1;
        playerInfo.basicIsPiercing = false;
        playerInfo.attack = 2;
    }

    public override void _BasicMove(Vector3Int destination) {
        Vector3Int[] adjTiles = GridHelper.GetAdjacentTiles(Coords());
        List<Vector3Int> tilesInMove = TilesInMove();
        if (!MovesAreSame(adjTiles, tilesInMove)) {
            base._BasicMove(destination);
            return;
        }

        for (int i = 0; i < 6; ++i) {
            if (GridHelper.CanStandOn(adjTiles[i])) {
                GridHelper.groundTilemap.SetColor(adjTiles[i], Color.white);
                if (adjTiles[i] == destination && playerInfo.resolve >= MoveCost() - (i == lastBasicMoveDir ? 1 : 0)) {
                    transform.position = GridHelper.grid.CellToWorld(destination) + cellOffset;
                    UpdateResolve(playerInfo.resolve - Mathf.Max(MoveCost() - (i == lastBasicMoveDir ? 1 : 0), 0));
                    playerInfo.moved = true;
                    playerInfo.turnDistanceTraveled++;
                    lastBasicMoveDir = i;
                }
            }
        }

        UpdateFreeMoveArrows();
    }

    private void UpdateFreeMoveArrows() {
        for (int i = 0; i < 6; ++i) {
            arrows[i].SetActive(i == lastBasicMoveDir);
        }
    }

    private bool MovesAreSame(Vector3Int[] adjTiles, List<Vector3Int> tilesInMove) {
        bool ok = true;
        int acSize = 0;
        for (int i = 0; i < 6; ++i) {
            if (GridHelper.CanStandOn(adjTiles[i])) {
                acSize++;
                ok &= GridHelper.TileInList(adjTiles[i], tilesInMove);
            }
            if (!ok)
                break;
        }

        return ok && acSize == tilesInMove.Count;
    }
}
