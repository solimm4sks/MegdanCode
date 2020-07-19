using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardCostHandler")]
public class CardCostHandler : ScriptableObject
{
    public List<CardCheck> checks;
    public int newCost;

    private int normalCost;
    //private int needsToCheck = 0;

    public virtual IEnumerator StartChecking(PlayerManager pm, CardLogic cl)
    {
        PlayerManager playerManager;
        CardLogic cardLogic;


        playerManager = pm;
        cardLogic = cl;
        normalCost = cl.card.resolveCost;
        //needsToCheck++;

        while (cardLogic != null) {

            bool ok = true;
            foreach (var check in checks)
                ok &= check.Check(playerManager);

            if (ok) {
                cardLogic.realResolveCost = newCost;
                cardLogic.cardVisuals.resolveObj.text = "" + newCost;
            }
            else {
                cardLogic.realResolveCost = normalCost;
                cardLogic.cardVisuals.resolveObj.text = "" + normalCost;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    public void StopChecking() { 
        //? wierd interaction since this is scriptable obj
    }
}
