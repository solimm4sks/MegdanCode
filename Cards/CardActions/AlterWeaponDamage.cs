using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/AlterWeaponDamage")]
public class AlterWeaponDamage : CardAction
{
    public int addedWeaponDamage = 0;
    public float weaponDamageMultiplier = 1;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction)
    {
        pm.AlterWeaponDamage(addedWeaponDamage, weaponDamageMultiplier);

        cae.cardPlayState[currentAction] = 1;
        yield break;
    }

}
