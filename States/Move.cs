using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : State
{
    public Move(PlayerManager playerManager) : base(playerManager) { }

    public override IEnumerator Start() {
        playerManager._BasicMoveShow();
        yield break;
    }

    public override IEnumerator BasicMove(Vector3Int destination) {
        playerManager._BasicMove(destination);
        playerManager.SetState(new MyTurn(playerManager));
        yield break;
    }
}
