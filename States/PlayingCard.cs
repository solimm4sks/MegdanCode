using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingCard : State
{
    private CardLogic cardLogic;

    public PlayingCard(PlayerManager playerManager, CardLogic cl) : base(playerManager) { cardLogic = cl; }

    public override IEnumerator Start() {
        playerManager.SetAllAttackableOutlines();
        cardLogic.PlayCard();
        yield break;
    }

    public override IEnumerator FinishedPlayingCard() {
        playerManager.SetState(new MyTurn(playerManager));
        yield break;
    }
}
