using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/SetHealth")]
public class SetHealth : CardAction
{
    public int health;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) {
        pm.SetHealth(health);
        cae.cardPlayState[currentAction] = 1;
        yield return null;
    }
}
