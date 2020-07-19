using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/Doublecast")]
public class Doublecast : CardAction
{
    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) {
        cae.cardLogic.cardInfo.doubleCast = true;
        cae.cardPlayState[currentAction] = 1;
        yield return null;
    }
}
