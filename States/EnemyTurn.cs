using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurn : State
{
    public EnemyTurn(PlayerManager playerManager) : base(playerManager) { }

    public override IEnumerator Start() {
        yield break;
    }

    public override bool IsMyTurnGeneral() {
        return false;
    }
}
