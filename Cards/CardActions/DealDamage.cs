using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/DealDamage")]
public class DealDamage : CardAction
{
    public int range;
    public bool straight;
    public bool piercing;
    public bool basicRange;
    public int damage;
    public bool weapon = false;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) {
        pm.CardAttack(damage, weapon, new RangeDefined(range, straight, piercing, basicRange), cae, currentAction); //action state gets set in here
        yield return null;
    }
}
