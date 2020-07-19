using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/Exile")]
public class Exile : CardAction
{
    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) {
        cae.cardLogic.toExile = true;
        cae.cardPlayState[currentAction] = 1;
        yield return null;
    }
}
