using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/PetrifyingPotion")]
public class PetrifyingPotion1 : CardAction
{
    public int dist;
    public bool self = true;

    //private PlayerManager playerManager;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction)
    {
        //playerManager = pm;
        pm.CardAttack(0, false, DamageFunction, new RangeDefined(dist, false, false, false, self), cae, currentAction);
        yield break;
    }

    public void DamageFunction(int dmg, IAttackable target, bool wpatk)
    {
        if (target is PlayerManager) {
            PlayerManager enemy = target as PlayerManager;
            enemy.GainBlock(10);
            enemy.Stun(1);
        }
    }
}
