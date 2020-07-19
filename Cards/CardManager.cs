using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class CardManager : MonoBehaviourPun
{
    public List<Pair<Card, CardInfo>> playerDeckCards = new List<Pair<Card, CardInfo>>();
    [HideInInspector] public PlayerManager playerManager;
    
    private GameObject myHand;
    private GameObject playedCardPreview;
    private GameObject previewObj;

    [SerializeField] private List<Card> inspectorViewDeck = new List<Card>();

    private void Awake() {
        myHand = GameObject.Find("Canvas/My Hand");
        playedCardPreview = GameObject.Find("Canvas/PlayedCardPreview");
        previewObj = GameObject.Find("Canvas/CardPreview");
    }

    private void Update() {
        inspectorViewDeck.Clear();
        foreach (Pair<Card, CardInfo> card in playerDeckCards) {
            inspectorViewDeck.Add(card.First);
        }
    }

    public void GetDeck() {
        string json = File.ReadAllText(Application.dataPath + "/SaveData/CurrentDeck.txt");
        DeckInfo deckInfo = JsonUtility.FromJson<DeckInfo>(json);

        foreach (var card in deckInfo.deck) {
            playerDeckCards.Add(new Pair<Card, CardInfo>(card, new CardInfo()));
        }

        Shuffle(playerDeckCards);
    }

    public void ReshuffleEndOfTurn() {
        List<CardLogic> handCards = HandCards();
        if (handCards.Count < 3)
            return;
    
        if(handCards.Count > 0)
            ReshuffleCard(handCards[0]);
    }

    public List<CardLogic> HandCards() {
        List<CardLogic> handCards = new List<CardLogic>();
        foreach (Transform cardTrans in myHand.transform) {
            CardLogic hcl = cardTrans.GetComponent<CardLogic>();
            handCards.Add(hcl);
        }
        return handCards;
    }


    public void PlayedCard(CardLogic card) {
        if(!card.cardInfo.exiled)
            playerDeckCards.Add(new Pair<Card, CardInfo>(card.card, card.cardInfo));
        playerManager.playerInfo.cardsPlayed++;
        playerManager.UpdateResolve(playerManager.playerInfo.resolve - card.realResolveCost);
        AddToPlayedCardPreview(card);
        Destroy(card.gameObject); 
        HideCardPreview();
    }

    public void PlayedDoublecastCard(CardLogic card) {
        playerManager.playerInfo.cardsPlayed++;
        playerManager.UpdateResolve(playerManager.playerInfo.resolve - card.realResolveCost);
        AddToPlayedCardPreview(card);
    }

    public void ReshuffleRandom() {
        int rand = Random.Range(0, myHand.transform.childCount);
        ReshuffleCard(myHand.transform.GetComponentsInChildren<CardLogic>()[rand]);
    }

    public void AddCardToBottom(Card card) { //dodaj kartu na dno decka
        AddCardToBottom(card, new CardInfo());
    }

    public void AddCardToBottom(Card card, CardInfo cardInfo) { //dodaj kartu na dno decka
        playerDeckCards.Add(new Pair<Card, CardInfo>(card, cardInfo));
    }

    public void ExileCard(CardLogic card, bool played) {
        playerManager.playerInfo.cardsPlayed++;
        Destroy(card.gameObject);
        HideCardPreview();
    }

    public void ReshuffleHandCards() {
        if (!photonView.IsMine)
            return;

        CardLogic[] handCards = myHand.GetComponentsInChildren<CardLogic>();
        foreach (CardLogic card in handCards) {
            ReshuffleCard(card); 
        }
    }

    public void DrawHandCards(int num = 2) {
        if (!photonView.IsMine)
            return;

        for (int i = 0; i < num; ++i) {
            GameObject curr = Instantiate(Resources.Load<GameObject>("Prefabs/Card"));
            curr.transform.SetParent(myHand.transform, false); 

            CardLogic cl = curr.GetComponent<CardLogic>(); 
            cl.card = playerDeckCards[0].First;
            cl.cardInfo = playerDeckCards[0].Second;

            CardVisuals cv = curr.GetComponent<CardVisuals>();
            cv.LoadCard(cl.card); 
            //if (cl.cardInfo.doubleCast) 
            //    cv.descObj.text = cl.card.desc + " (DOUBLECAST)";
            
            if (cl.cardInfo.exhausted)  
                cv.titleObj.text = cl.card.title + " (EXHAUSTED)";
            
            playerDeckCards.Remove(playerDeckCards[0]); 
        }
    }

    public void ReshuffleCard(CardLogic card) {
        playerDeckCards.Insert(Random.Range(1, playerDeckCards.Count), new Pair<Card, CardInfo>(card.card, card.cardInfo));
        Destroy(card.gameObject); 
        HideCardPreview();
    }

    public void ShuffleNewCard(Card card, CardInfo cardInfo) {
        playerDeckCards.Insert(Random.Range(1, playerDeckCards.Count), new Pair<Card, CardInfo>(card, cardInfo));
    }


    [PunRPC]
    public void ATPCRPC(string title, string desc, string flavor, int resolveCost) {
        GameObject curr = Instantiate(Resources.Load<GameObject>("Prefabs/Card"));
        curr.transform.SetParent(playedCardPreview.transform, false);
        curr.transform.localScale = new Vector3(1.6f, 1.6f, 1);

        CardLogic cl = curr.GetComponent<CardLogic>();
        cl.enabled = false;

        CardVisuals cv = curr.GetComponent<CardVisuals>();
        
        cv.LoadCard(title, desc, flavor, resolveCost);
        StartCoroutine(PlayPCPAnim(curr));
    }

    private IEnumerator PlayPCPAnim(GameObject card) {
        Animator canimator = card.GetComponent<Animator>();
        canimator.SetTrigger("slide");

        yield return new WaitForEndOfFrame();

        while (canimator.GetCurrentAnimatorStateInfo(0).IsName("Card_PlayedPreview")) {
            yield return null;
        }

        Destroy(card);
    }

    public void AddToPlayedCardPreview(CardLogic card)
    {
        photonView.RPC("ATPCRPC", RpcTarget.All, card.card.name, card.card.desc, card.card.flavor, card.realResolveCost);
    }

    public GameObject ShowCardPreview(Card card, CardInfo cardInfo, string previewObjName = "Canvas/CardPreview") {
        if(previewObjName == "Canvas/CardPreview")
            previewObj = GameObject.Find(previewObjName);
        
        GameObject curr = Instantiate(Resources.Load<GameObject>("Prefabs/Card"));
        curr.transform.SetParent(previewObj.transform, false);
        curr.transform.localScale = new Vector3(1.4f, 1.4f, 1);

        CardLogic cl = curr.GetComponent<CardLogic>();
        cl.enabled = false;

        CardVisuals cv = curr.GetComponent<CardVisuals>();
        cv.LoadCard(card);

        //if (cardInfo.doubleCast) 
        //    cv.descObj.text = card.desc + " (DOUBLECAST)";
        
        if (cardInfo.exhausted) 
            cv.titleObj.text = card.title + " (EXHAUSTED)";

        return curr;    
    }

    public void HideCardPreview(string previewObjName = "Canvas/CardPreview") {
        GameObject previewObj = GameObject.Find(previewObjName);
        foreach (Transform child in previewObj.transform) {
            Destroy(child.gameObject);
        }   
    }

    private void Shuffle<T>(IList<T> ts) {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}
