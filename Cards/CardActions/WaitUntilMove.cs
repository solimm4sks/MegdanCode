using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/WaitUntilMove")]
public class WaitUntilMove : CardAction
{
    private CardActionExecutor cardActionExecutor;
    private int currAction;
    private PlayerManager playerManager;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction)
    {
        cardActionExecutor = cae;
        currAction = currentAction;
        playerManager = pm;

        pm.Moved += OnMoved;
        cae.startedPassiveAction = true;
        yield return null;
    }

    private void OnMoved()
    {
        cardActionExecutor.cardPlayState[currAction] = 1;
        playerManager.Moved -= OnMoved;
    }
}
