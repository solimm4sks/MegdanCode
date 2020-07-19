using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/AlterBasicDamage")]
public class AlterBasicDamage : CardAction
{
    public int addedBasicAttack = 0;
    public float basicAttackMultiplier = 1;
    public int setBasicAttack = -1;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction)
    {
        pm.AlterBasicDamage(addedBasicAttack, basicAttackMultiplier, setBasicAttack);

        cae.cardPlayState[currentAction] = 1;
        yield break;
    }

}
