using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

public class CardLogic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private PlayerManager player1;
    private PlayerManager player2;

    public Card card; //set in card manager
    public CardVisuals cardVisuals; //set in card manager
    public CardInfo cardInfo = new CardInfo(); //set in card manager
    public CardActionExecutor caePrefab;

    public bool toExile = false;
    public bool toExhaust = false;
    public int castCount; //how many times has this card been cast already (relevant for doublecast)

    public int realResolveCost;
    public CardCostHandler cardCostHandler;

    private void Start() {
        player1 = GameObject.Find("Player1").GetComponent<PlayerManager>();
        player2 = GameObject.Find("Player2").GetComponent<PlayerManager>();
        caePrefab = Resources.Load<CardActionExecutor>("Prefabs/CardActionExecutor");

        cardVisuals = GetComponent<CardVisuals>();
        realResolveCost = card.resolveCost;
        cardCostHandler = card.cardCostHandler;
        if(cardCostHandler != null)
            StartCoroutine(cardCostHandler.StartChecking(CurrentPlayer(), this));
    }

    private void Update() {

    }

    GameObject preview;

    public void OnPointerEnter(PointerEventData pointerData) {
        preview = CurrentPlayer().cardManager.ShowCardPreview(card, cardInfo);
    }

    public void OnPointerExit(PointerEventData pointerData) {
        Destroy(preview);
    }

    public void OnPointerClick(PointerEventData pointerData) {
        if (!CurrentPlayer().state.IsMyTurnGeneral())
            return;
        PlayThis();
    }


    public void PlayThis() {
        if (cardInfo.exhausted)
            return;

        if (!CanPlayMoreCards())
            return;

        if ((castCount == 1 && !cardInfo.doubleCast) || castCount == 2) //not even ever gonna get to this since the card will be destroyed but oh well
            return;

        bool allPlayable = true;
        foreach (CardCheck ca in card.cardChecks)
            allPlayable &= ca.Check(CurrentPlayer());

        if (allPlayable) {
            StartCoroutine(CurrentPlayer().state.PlayCard(this));            
        }
    }

    public void PlayCard() { //called by PlayingCard(State)
        CardActionExecutor cae = Instantiate(caePrefab); cae.player1 = player1; cae.player2 = player2;
        cae.Execute(this);
    }

    private bool CanPlayMoreCards() {
        //Debug.Log(realResolveCost);
        return CurrentPlayer().playerInfo.resolve >= realResolveCost;
    }

    private PlayerManager CurrentPlayer() {
        if (PhotonNetwork.IsMasterClient)
            return player1;
        else
            return player2;
    }
}
