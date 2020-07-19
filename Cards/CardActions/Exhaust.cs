using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/Exhaust")]
public class Exhaust : CardAction
{
    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) {
        cae.cardLogic.toExhaust = true;
        cae.cardPlayState[currentAction] = 1;
        yield return null;
    }
}
