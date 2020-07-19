using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/Unshaken")]
public class Unshaken : CardAction
{
    public bool unshaken = true;
    public bool enemy = false;
    public int turns;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction)
    {
        if (unshaken)
            if (enemy)
                pm.otherManager.Unshaken(turns);
            else
                pm.Unshaken(turns);
        else
            if (enemy)
                pm.otherManager.Deunshaken();
            else
                pm.Deunshaken();

        cae.cardPlayState[currentAction] = 1;
        yield return null;
    }
}
