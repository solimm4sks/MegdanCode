using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/Slow")]
public class Slow : CardAction {
    public bool slow = true;
    public bool enemy = true;
    public int turns;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) {
        if (slow)
            if (enemy)
                pm.otherManager.Slow(turns);
            else
                pm.Slow(turns);
        else
            if (enemy)
            pm.otherManager.Unslow ();
        else
            pm.Unslow();

        cae.cardPlayState[currentAction] = 1;
        yield return null;
    }
}
