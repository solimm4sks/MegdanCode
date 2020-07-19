using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : State
{
    public Attack(PlayerManager pm) : base(pm) { }

    public override IEnumerator Start() {
        List<Vector3Int> tiles = playerManager.TilesInBasic();
        List<RaycastHit2D> hits = GridHelper.RaycastTiles(tiles);
        for (int i = 0; i < tiles.Count; ++i) {
            GridHelper.groundTilemap.SetColor(tiles[i], playerManager.atkRangeColor);
            if (hits[i]) {
                IAttackable target = hits[i].collider.gameObject.transform.parent.GetComponent<IAttackable>();
                if (target != null) {
                    target.SetAttackableOutline(!playerManager.playerInfo.attacked);
                }
            }
        }
        yield break;
    }

    public override IEnumerator LandAttack(IAttackable target) { //doesnt check if I have enought resolve?
        if (playerManager.playerInfo.attacked)
            yield break;

        PlayerInfo pi = playerManager.playerInfo;
        if(!pi.basicIsStraight || !pi.basicIsPiercing) {
            playerManager.DamageTarget(playerManager.BasicDamage(), target, true);
            target.SetAttackableOutline(false); //false = !attacked
        }
        else { ///this only for piercing atks (so ranger)
            int targetDir = -1;
            int basicDist = playerManager.BasicRange();
            List<Vector3Int>[] tilesInDir = new List<Vector3Int>[6];

            for(int i = 0; i < 6; ++i) {
                tilesInDir[i] = GridHelper.TilesInDirection(playerManager.Coords(), i, basicDist, true, GridHelper.CanAttackThrough);
                if(GridHelper.TileInList(target.Coords(), tilesInDir[i])) {
                    targetDir = i;
                    break;
                }
            }

            List<RaycastHit2D> hits = GridHelper.RaycastTilesNoEmptyHits(tilesInDir[targetDir]);
            foreach(var hit in hits) {
                IAttackable piercedTarget = hit.transform.parent.GetComponent<IAttackable>();
                if(piercedTarget != null) {
                    playerManager.DamageTarget(playerManager.BasicDamage(), piercedTarget);
                    target.SetAttackableOutline(false);
                }
            }
        }

        playerManager.UpdateResolve(playerManager.playerInfo.resolve - playerManager.BasicCost());
        playerManager.playerInfo.attacked = true;

        playerManager.HideAttack();
        yield break;
    }

    public override IEnumerator HideAttack() {
        List<Vector3Int> tiles = playerManager.TilesInBasic();
        List<RaycastHit2D> hits = GridHelper.RaycastTiles(tiles);
        for (int i = 0; i < tiles.Count; ++i) {
            GridHelper.groundTilemap.SetColor(tiles[i], Color.white);
            if (hits[i]) {
                IAttackable target = hits[i].transform.parent.GetComponent<IAttackable>();
                if (target != null) {
                    target.SetAttackableOutline(false);
                }
            }
        }

        playerManager.SetState(new MyTurn(playerManager));
        yield break;
    }

    /*
    public override IEnumerator HideAttackRange() {
        List<Vector3Int> tiles = new List<Vector3Int>();
        tiles = playerManager.TilesInBasic();

        foreach (Vector3Int tile in tiles)
            GridHelper.groundTilemap.SetColor(tile, Color.white);

        playerManager.SetState(new MyTurn(playerManager));
        yield break;
    }
    */
}
