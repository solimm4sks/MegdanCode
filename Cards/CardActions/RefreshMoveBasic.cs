using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/RefreshMoveBasic")]
public class RefreshMoveBasic : CardAction
{
    public bool refreshBasic;
    public bool rbEnemy;
    public bool refreshMove;
    public bool rmEnemy;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction)
    {
        if(refreshBasic)
            if(rbEnemy)
                pm.otherManager.RefreshBasic();
            else
                pm.RefreshBasic();
        if (refreshMove)
            if (rmEnemy)
                pm.otherManager.RefreshMove();
            else
                pm.RefreshMove();

        cae.cardPlayState[currentAction] = 1;
        yield return null;
    }
}
