    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(menuName = "CardAction/WhirlwindTechnique")]
public class WhirlwindTechnique : CardAction
{
    public int blockAmount1;
    public int blockAmount2;

    public int damage1;
    public int damage2;

    private PlayerManager playerManager;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) {
        playerManager = pm;
        pm.CardAttack(0, true, DamageFunction, new RangeDefined(0, false, false, true), cae, currentAction);
        yield return null;
    }

    public void DamageFunction(int dmg, IAttackable target, bool wpnAtk) {
        int rand = Random.Range(0, 2);
        if (rand == 0) {
            playerManager.GainBlock(blockAmount1);
            playerManager.DamageTarget(damage1, target);
        }
        else {
            playerManager.GainBlock(blockAmount2);
            playerManager.DamageTarget(damage2, target, wpnAtk);
        }
    }
}
