using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/GainBlock")]
public class GainBlock : CardAction
{
    public int blockAmount = 0;
    public double currentBlockMultiplier = 0;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) {
        pm.GainBlock((int)(pm.playerInfo.block * (currentBlockMultiplier - 1) + blockAmount));
        cae.cardPlayState[currentAction] = 1;
        yield return null;
    }
}
