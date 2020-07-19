using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/AlterBasicRange")]
public class AlterBasicRange : CardAction
{
    public int addedBasicRange = 0;
    public float basicRangeMultiplier = 1;
    public int setBasicRange = -1;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) {
        pm.AlterBasicRange(addedBasicRange, basicRangeMultiplier, setBasicRange);
        cae.cardPlayState[currentAction] = 1;
        yield return null;
    }
}
