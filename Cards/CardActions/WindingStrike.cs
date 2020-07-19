using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/WindingStrike")]
public class WindingStrike : CardAction
{
    public int baseDamage;
    public double baseMultiplier;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) {
        pm.CardAttack(baseDamage + (int)(baseMultiplier * pm.playerInfo.turnDistanceTraveled), new RangeDefined(0, false, false, true), cae, currentAction);
        yield return null;
    }
}
