using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/Shock")]
public class Shock : CardAction{
    public bool shock = true;
    public bool enemy = true;
    public int turns;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) {
        if (shock)
            if (enemy)
                pm.otherManager.Shock(turns);
            else
                pm.Shock(turns);
        else
            if (enemy)
                pm.otherManager.Unshock();
            else
                pm.Unshock();

        cae.cardPlayState[currentAction] = 1;
        yield return null;
    }
}
