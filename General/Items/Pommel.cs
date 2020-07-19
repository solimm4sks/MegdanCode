using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pommel : Spawnable
{
    bool entered = false;

    protected override void Start()
    {
        base.Start();
    }

    private void FixedUpdate() {
        if (!entered && Coords() == spawnerPlayer.Coords()) {
            Pickup();
        }
    }

    private void Pickup() {
        if (!photonView.IsMine)
            return;

        entered = true;

        //unexh from hand
        if (spawnerPlayer.state.IsMyTurnGeneral()) {
            GameObject playerHand = GameObject.Find("Canvas/My Hand");
            foreach (Transform child in playerHand.transform) {
                CardLogic cl = child.GetComponent<CardLogic>();
                if (cl.card.title == "Pommel Toss" && cl.cardInfo.exhausted) {
                    cl.cardInfo.exhausted = false;
                    cl.cardVisuals.LoadCard(cl.card); //update viusals in hand
                    PhotonNetwork.Destroy(gameObject);
                    return;
                }
            }
        }

        //unexh from deck
        foreach (Pair<Card, CardInfo> card in spawnerPlayer.cardManager.playerDeckCards) {
            if (card.First.title == "Pommel Toss" && card.Second.exhausted) {
                card.Second.exhausted = false;
                break;
            }
        }

        PhotonNetwork.Destroy(gameObject);
    }
}
