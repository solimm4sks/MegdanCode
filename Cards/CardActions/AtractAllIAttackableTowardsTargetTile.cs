using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/AtractAllIAttackableTowardsTargetTile")]
public class AtractAllIAttackableTowardsTargetTile : CardAction
{
    public int dist = -1;
    public bool straight;
    public bool piercing;
    public bool basicRange;

    private CardActionExecutor _cardActionExecutor;
    private int _currentAction;
    private PlayerManager _playerManager;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction)
    {
        pm.CardSelectTile(new RangeDefined(dist, straight, piercing, basicRange), SelectedTile);
        _cardActionExecutor = cae;
        _currentAction = currentAction;
        _playerManager = pm;

        yield break;
    }

    public void SelectedTile(Vector3Int? tile) {
        if (tile == null) {
            _cardActionExecutor.cardPlayState[_currentAction] = 2;
            return;
        }

        _playerManager.StartCoroutine(PullTargetsTowards(tile.Value));

        _cardActionExecutor.cardPlayState[_currentAction] = 1;
    }

    private IEnumerator PullTargetsTowards(Vector3Int targetTile) {
        List<IAttackable> attackables = GridHelper.IAttackablesInTiles(GridHelper.TilesInDistance(_playerManager.Coords(), -1));
        attackables.Remove(_playerManager);
        
        List<List<IAttackable>> attackablesByDist = new List<List<IAttackable>>();
        List<List<List<Vector3Int>>> availableByDist = new List<List<List<Vector3Int>>>();
        
        for (int i = 0; i <= GridHelper.MAX_TILEDIST_ON_MAP; ++i) { //populate attackablesByDist
            attackablesByDist.Add(new List<IAttackable>());

            foreach (var atk in attackables)
                if (GridHelper.TileDistance(targetTile, atk.Coords()) == i)
                    attackablesByDist[i].Add(atk);
        }

        for (int dist = 0; dist <= GridHelper.MAX_TILEDIST_ON_MAP; ++dist) { //populate availableByDist
            availableByDist.Add(new List<List<Vector3Int>>());
            
            foreach (IAttackable attackable in attackablesByDist[dist]) {
                availableByDist[dist].Add(new List<Vector3Int>());
                Vector3Int[] atkAdjs = GridHelper.GetAdjacentTiles(attackable.Coords());
                for (int i = 0; i < 6; ++i) {
                    if (GridHelper.groundTilemap.HasTile(atkAdjs[i]) && GridHelper.TileDistance(targetTile, atkAdjs[i]) == dist - 1)
                        availableByDist[dist][availableByDist[dist].Count - 1].Add(atkAdjs[i]);
                }
            }
        }

        //move non players
        for (int dist = 0; dist <= GridHelper.MAX_TILEDIST_ON_MAP; ++dist) {
            for (int attackableId = 0; attackableId < availableByDist[dist].Count; ++attackableId) {
                IAttackable attackable = attackablesByDist[dist][attackableId];
                if (attackable is PlayerManager) //
                    continue;

                foreach (var avalibleTile in availableByDist[dist][attackableId]) {
                    if (GridHelper.CanStandOn(avalibleTile)) {
                        attackable.MoveTo(avalibleTile);
                        break;
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        //move players, same code as before
        for (int dist = 0; dist <= GridHelper.MAX_TILEDIST_ON_MAP; ++dist) {
            for (int attackableId = 0; attackableId < availableByDist[dist].Count; ++attackableId) {
                IAttackable attackable = attackablesByDist[dist][attackableId];
                if (!(attackable is PlayerManager)) //only dif here
                    continue;

                foreach (var avalibleTile in availableByDist[dist][attackableId]) {
                    if (GridHelper.CanStandOn(avalibleTile)) {
                        attackable.MoveTo(avalibleTile);
                        break;
                    }
                }


                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private bool AttackableInList(IAttackable atk, List<IAttackable> atks) {
        foreach (var atkk in atks)
            if (atk == atkk)
                return true;
        return false;
    }
}
