using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTurn : State
{
    public MyTurn(PlayerManager playerManager) : base(playerManager) { }

    public override IEnumerator Start() {
        playerManager.SetAllAttackableOutlines();
        yield break;
    }

    public override IEnumerator ShowBasicMove() {
        if(!playerManager.playerInfo.moved && CanMove())
            playerManager.SetState(new Move(playerManager));
        yield break;
    }
    public override IEnumerator Attack() {
        List<RaycastHit2D> hits = GridHelper.RaycastTile(playerManager.MouseCoords());

        bool landedHit = false;
        foreach (var hit in hits) {
            IAttackable target = hit.transform.parent.GetComponent<IAttackable>();
            if (target == null) {
                continue;
            }
            landedHit = true;
            playerManager.AttackBasedOnPiercing(target);
            break;
        }

        if (landedHit) {
            playerManager.UpdateResolve(playerManager.playerInfo.resolve - playerManager.BasicCost());
            playerManager.playerInfo.attacked = true;
            playerManager.playerInfo.nextFreeBasic = false;
            playerManager.SetAllAttackableOutlines();
        }

        yield break;
    }

    public override IEnumerator ShowAttackRange() {
        playerManager.SetState(new ShowAttack(playerManager));
        yield break;
    }

    public override bool CanMove() {
        return playerManager.playerInfo.stunned == 0;
    }

    public override IEnumerator PlayCard(CardLogic cl) {
        playerManager.SetState(new PlayingCard(playerManager, cl));
        yield break;
    }

    public override bool IsMyTurn()
    {
        return true;
    }
}
