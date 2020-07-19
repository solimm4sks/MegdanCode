using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActionExecutor : MonoBehaviour
{
    //public CardAction[] knightActions;
    //public CardAction[] rangerActions;

    [HideInInspector]public PlayerManager player1;
    [HideInInspector]public PlayerManager player2; //gets set in CardLogic

    [HideInInspector]public List<int> cardPlayState; //0 = didnt finish yet, 1 = passed, 2 = failed, 
    [HideInInspector]public CardLogic cardLogic;
    [HideInInspector]public bool startedPassiveAction = false;

    public void Execute(CardLogic cl) {
        cardLogic = cl;
        StartCoroutine(PlayCoroutine());
    }

    private IEnumerator PlayCoroutine() {
        List<CardAction> actions = cardLogic.card.cardActions;
        List<CardCheck> checks = cardLogic.card.cardChecks;
        CardInfo cardInfo = cardLogic.cardInfo;

        bool ok = true, playedThisAction = false;
        int counter = 0;

        cardPlayState = new List<int>();
        for (int i = 0; i < actions.Count; ++i) 
            cardPlayState.Add(0);
        

        while (counter < cardPlayState.Count) {
            if (cardPlayState[counter] == 1) { //played current action, move to next
                counter++;
                playedThisAction = false;
            }
            else if (cardPlayState[counter] == 2) { //stopped playing current action, cancel it!
                ok = false;
                break;
            }

            if (counter == cardPlayState.Count) //exit the loop
                break;

            if (startedPassiveAction) { ///if i started passive action, destroy the card instance
                break;
            }

            if (!playedThisAction) { //if i am not waiting for this action to be finished but need to actually play it
                bool passedActionCheck = true;
                if (actions[counter].actionChecks != null)
                    for (int i = 0; i < actions[counter].actionChecks.Length; ++i)
                        passedActionCheck &= actions[counter].actionChecks[i].Check(CurrentPlayer());

                if (passedActionCheck) {
                    StartCoroutine(actions[counter].Execute(CurrentPlayer(), this, counter));
                }
                playedThisAction = true; //if i cannot play this action -here-, its not vital to the card so dont cancel and just continue on to the next action
            }

            yield return null;
        }

        FinishedPlayingCard(ok); ///destroy the card instance

        while (counter < cardPlayState.Count) { //continue the passive actions, after card instance has been destroyed
            if (cardPlayState[counter] == 1) { //played current action, move to next
                counter++;
                playedThisAction = false;
            }
            else if (cardPlayState[counter] == 2) { //stopped playing current action, cancel it!
                ok = false;
                break;
            }

            if (counter == cardPlayState.Count) //exit the loop
                break;

            if (!playedThisAction) { //if i am not waiting for this action to be finished but need to actually play it
                bool passedActionCheck = true;
                if(actions[counter].actionChecks != null)
                    for (int i = 0; i < actions[counter].actionChecks.Length; ++i)
                        passedActionCheck &= actions[counter].actionChecks[i].Check(CurrentPlayer());

                if (passedActionCheck) {
                    StartCoroutine(actions[counter].Execute(CurrentPlayer(), this, counter));
                }
                playedThisAction = true; //if i cannot play this action -here-, its not vital to the card so dont cancel and just continue on to the next action
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    private void FinishedPlayingCard(bool ok) { ///handles exile, doublecast, exhaust, destroys card instance
        if (ok) {
            if(!(cardLogic.castCount == 1 && cardLogic.cardInfo.doubleCast) && cardLogic.cardCostHandler != null)
                cardLogic.cardCostHandler.StopChecking();

            cardLogic.castCount++;
            if (cardLogic.toExhaust) {
                cardLogic.cardInfo.exhausted = true;
            }
            if (cardLogic.toExile) {
                cardLogic.cardInfo.exiled = true;
            }

            if (cardLogic.castCount == 1 && cardLogic.cardInfo.doubleCast)
                CurrentPlayer().cardManager.PlayedDoublecastCard(cardLogic);
            else
                CurrentPlayer().cardManager.PlayedCard(cardLogic);
        }

        StartCoroutine(CurrentPlayer().state.FinishedPlayingCard());

        if (cardLogic.cardInfo.exiled) {
            CurrentPlayer().cardManager.ExileCard(cardLogic, true);
        }
    }

    private PlayerManager CurrentPlayer()
    {
        if (PhotonNetwork.IsMasterClient)
            return player1;
        else
            return player2;
    }
}
