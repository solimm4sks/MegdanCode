using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "CardAction/DamageEveryoneInRange")]
public class DamageEveryoneInRange : CardAction
{
    public int range;
    public bool straight;
    public bool piercing;
    public bool basicRange;
    public int damage;
    public bool weapon = false;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction)
    {
        List<IAttackable> targets = GridHelper.IAttackablesInTiles(GridHelper.TilesInRange(pm.Coords(), new RangeDefined(range, straight, piercing, basicRange)));
        foreach (var target in targets) {
            pm.DamageTarget(damage, target, weapon);
        }

        cae.cardPlayState[currentAction] = 1;
        yield break;
    }
}
