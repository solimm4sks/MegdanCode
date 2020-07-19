using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/Plunge")]
public class Plunge : CardAction
{
    public int dir;
    public int damage;
    //use basic range by default

    private PlayerManager playerManager;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) 
    {
        playerManager = pm;
        if (dir == -1)
            pm.CardAttack(damage, true, DamageFunction, new RangeDefined(1, true, false, true), cae, currentAction);
        else
            pm.CardAttack(damage, true, new RangeDefined(1, true, false, true, new List<int>() { dir }, null), cae, currentAction);

        yield break;
    }

    public void DamageFunction(int dmg, IAttackable target, bool wpatk) 
    {
        int dmgDir = GridHelper.GetAdjDirection(playerManager.Coords(), target.Coords());
        playerManager.cardManager.AddCardToBottom(Resources.Load("Data/Cards/Upgrades/Knight/Plunge " + dmgDir) as Card);
        playerManager.DamageTarget(dmg, target, wpatk);
    }
}
