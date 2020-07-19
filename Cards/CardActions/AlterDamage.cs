using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/AlterDamage")]
public class AlterDamage : CardAction
{
    public int addedDamage = 0;
    public float damageMultiplier = 1;
    public int setDamage = -1;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction)
    {
        pm.AlterDamage(addedDamage, damageMultiplier, setDamage);

        cae.cardPlayState[currentAction] = 1;
        yield break;
    }

}
