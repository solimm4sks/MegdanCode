using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/GainHealth")]
public class GainHealth : CardAction
{
    public int health;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) {
        pm.GainHealth(health);
        cae.cardPlayState[currentAction] = 1;
        yield return null;
    }
}
