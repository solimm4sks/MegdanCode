using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/WaitTurns")]
public class WaitTurns : CardAction {
    public double turns;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) {
        cae.startedPassiveAction = true;

        double playedTurn = pm.gameManager.turnCount;
        double currentTurn = playedTurn;
        while (playedTurn + turns > currentTurn) {
            currentTurn = pm.gameManager.turnCount;
            yield return null;
        }

        cae.cardPlayState[currentAction] = 1;
        yield return null;
    }
}
