using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/PreventDisableOnTurn")]
public class PreventDisable : CardAction
{
    public bool preventSlow;
    public bool preventShock;
    public bool preventStun;
    public double lastHowLong = 0;


    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) {
        double startingTurn = pm.gameManager.turnCount;
        bool prevented = false;

        while (startingTurn + lastHowLong > pm.gameManager.turnCount) {
            if(preventShock && pm.playerInfo.shocked > 0) {
                pm.Unshock();
                prevented = true;
            }
            if(preventSlow && pm.playerInfo.slowed > 0) {
                pm.Unslow();
                prevented = true;
            }
            if(preventStun && pm.playerInfo.stunned > 0) {
                pm.Unstun();
                prevented = true;
            }

            if(prevented && lastHowLong == 0)
                break;
            yield return null;
        }

        cae.cardPlayState[currentAction] = 1;
        yield return null;
    }
}
