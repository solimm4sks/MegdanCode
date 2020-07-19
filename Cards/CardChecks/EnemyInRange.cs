using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardCheck/EnemyInRange")]
public class EnemyInRange : CardCheck
{
    public int range;
    public bool straight;
    public bool piercing;
    public bool basicRange = false;
    
    private List<Vector3Int> attackableTiles;

    public override bool Check(PlayerManager pm) {
        if (basicRange)
            return pm.AttackableInBasicRange();

        return pm.AttackableInTiles(pm.TilesInAttack(new RangeDefined(range, straight, piercing, basicRange)));
    }
}
