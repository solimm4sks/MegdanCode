using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected PlayerManager playerManager;

    public State(PlayerManager pm) {
        playerManager = pm; 
    }

    public virtual IEnumerator Start() {
        yield break;
    }

    public virtual IEnumerator ShowBasicMove() {
        yield break;
    }

    public virtual IEnumerator BasicMove(Vector3Int destination) {
        yield break;
    }

    public virtual IEnumerator ShowAttackRange() {
        yield break;
    }

    public virtual IEnumerator LandAttack(IAttackable target) {
        yield break;
    }

    public virtual IEnumerator HideAttack() {
        yield break;
    }
    /*
    public virtual IEnumerator HideAttackRange() {
        yield break;
    }
    */

    public virtual IEnumerator Attack() {
        yield break;
    }

    public virtual IEnumerator SwitchTurn() {
        yield break;
    }

    public virtual bool IsMyTurnGeneral() {
        return true;
    }

    public virtual bool IsMyTurn() {
        return false;
    }

    public virtual IEnumerator FinishedPlayingCard() {
        yield break;
    }

    public virtual bool CanMove() {
        return false;
    }

    public virtual IEnumerator PlayCard(CardLogic cl) {
        yield break;
    }

    public virtual IEnumerator ShowEnemyAtkOutline() {
        yield break;
    }
}
