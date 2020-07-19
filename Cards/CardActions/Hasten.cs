using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/Hasten")]
public class Hasten : CardAction
{
    public int turns;
    public bool hasten;
    public bool enemy;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) 
    {
        if(enemy) {
            if(hasten) {
                pm.otherManager.Hasten(turns);
            }
            else {
                pm.otherManager.Unhasten();
            }
        }
        else {
            if(hasten) {
                pm.Hasten(turns);
            }
            else {
                pm.Unhasten();
            }
        }

        cae.cardPlayState[currentAction] = 1;
        yield return null;        
    }
}
