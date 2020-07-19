using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurn : State
{
    public EndTurn(PlayerManager playerManager) : base(playerManager) { }

    public override IEnumerator Start() {
        playerManager.UpdateAHBText();
        if(playerManager.photonView.IsMine)
            playerManager.cardManager.ReshuffleEndOfTurn();
        playerManager.playerInfo.slowed -= playerManager.playerInfo.slowed > 0 ? 1 : 0;
        playerManager.playerInfo.shocked -= playerManager.playerInfo.shocked > 0 ? 1 : 0;
        playerManager.playerInfo.hasted -= playerManager.playerInfo.hasted > 0 ? 1 : 0;
        playerManager.playerInfo.stunned -= playerManager.playerInfo.stunned > 0 ? 1 : 0;
        playerManager.playerInfo.unshaken -= playerManager.playerInfo.unshaken > 0 ? 1 : 0;
        playerManager.playerInfo.immovable -= playerManager.playerInfo.immovable > 0 ? 1 : 0;

        playerManager.SetAllAttackableOutlines();
        playerManager.SetState(new EnemyTurn(playerManager));
        yield break;
    }
}
