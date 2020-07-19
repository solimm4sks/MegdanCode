using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/HeavyBarrage")]
public class HeavyBarrage : CardAction
{
    public int damage;
    public bool isWeaponAtk;

    private PlayerManager playerManager;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction)
    {
        playerManager = pm;
        pm.CardAttack(damage, isWeaponAtk, DamageFunction, new RangeDefined(2, true, false, false), cae, currentAction);
        yield return null;
    }

    public void DamageFunction(int dmg, IAttackable target, bool wpnAtk) ///WORKS ONLY WITH ATKDIST = 2
    {
        int dir = -1;
        Vector3Int[] adjs = GridHelper.GetAdjacentTiles(playerManager.Coords());
        for(int i = 0; i < 6; ++i) {
            if(target.Coords() == adjs[i] || target.Coords() == GridHelper.GetTileInDirection(adjs[i], i)) {
                dir = i;
                break;
            }
        }

        Vector3Int toStandOn = GridHelper.GetTileInDirection(target.Coords(), dir);
        if(GridHelper.CanStandOn(toStandOn)) 
            target.MoveTo(toStandOn);
        else
            playerManager.DamageTarget(dmg, target, wpnAtk);
    }
}
