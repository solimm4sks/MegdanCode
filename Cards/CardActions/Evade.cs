using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/Evade")]
public class Evade : CardAction
{
    public bool evade = true;
    public bool enemy = false;
    public int turns;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction)
    {
        if (evade)
            if (enemy)
                pm.otherManager.AddEvade(turns);
            else
                pm.AddEvade(turns);
        else
            if (enemy)
            pm.otherManager.Unevade();
        else
            pm.Unevade();

        cae.cardPlayState[currentAction] = 1;
        yield break;
    }
}
