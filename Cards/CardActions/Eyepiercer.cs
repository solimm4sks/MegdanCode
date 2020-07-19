using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/Eyepiercer")]
public class Eyepiercer : CardAction
{
    public int dist = 3;
    public bool straight = true;
    public bool piercing = true;
    public bool basicRange = false;
    public bool isWeaponAtk = true;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction)
    {
        pm.CardAttack((int)Mathf.Pow(2, 1 + Mathf.Max(pm.playerInfo.stillStandingTurns - 1, 0)), isWeaponAtk, new RangeDefined(dist, straight, piercing, basicRange), cae, currentAction);
        yield break;
    }
}
