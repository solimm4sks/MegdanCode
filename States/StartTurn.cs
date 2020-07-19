using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTurn : State
{
    public StartTurn(PlayerManager playerManager) : base(playerManager) { }

    public override IEnumerator Start() {
        playerManager.playerInfo.moved = false;
        playerManager.playerInfo.attacked = false;
        playerManager.SetDistanceTraveled(0);

        if (playerManager.photonView.IsMine) {
            playerManager.SetStillStandingTurns(playerManager.playerInfo.stillStandingTurns + 1);
            if (playerManager.gameManager.turnCount < 2)
                playerManager.DrawCards(1);
            playerManager.DrawCards(1);
            playerManager.UpdateResolve(GameInfo.ResolveOnTurn((int)playerManager.gameManager.turnCount));
            //playerManager.RemoveBlock();
        }

        playerManager.SetState(new MyTurn(playerManager));
        yield break;
    }
}
