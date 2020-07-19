using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/DrawCards")]
public class DrawCards : CardAction
{
    public int num;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) {
        pm.cardManager.DrawHandCards(num);
        cae.cardPlayState[currentAction] = 1;
        yield return null;
    }
}
