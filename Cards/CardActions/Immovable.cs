using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/Immovable")]
public class Immovable : CardAction
{
    public bool immovable = true;
    public bool enemy = false;
    public int turns;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction)
    {
        if (immovable)
            if (enemy)
                pm.otherManager.Immovable(turns);
            else
                pm.Immovable(turns);
        else
            if (enemy)
            pm.otherManager.Unimmovable();
        else
            pm.Unimmovable();

        cae.cardPlayState[currentAction] = 1;
        yield return null;
    }
}
