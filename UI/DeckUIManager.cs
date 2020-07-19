using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using JetBrains.Annotations;
using Photon.Pun;

public class DeckUIManager : MonoBehaviourPun
{
#pragma warning disable 649

    [SerializeField] private TextMeshProUGUI errorMessage;
    [SerializeField] private TextMeshProUGUI cardSum;
    [SerializeField] private SceneLoader sc;
    [SerializeField] private ClassCards classCards;

    [HideInInspector] public List<TextMeshProUGUI> cardsName;
    [HideInInspector] public List<TMP_InputField> cardsNum;
    private List<DeckBuild_PreviewOnHover> cardUIObjs;

    public string pClass = "";

    private void Start()
    {
        pClass = "Knight";

        TextMeshProUGUI[] pCardsT = GameObject.Find("Canvas/PlayerDeck/PCards").GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI card in pCardsT) {
            if (card.gameObject.name == "CardName")
                cardsName.Add(card);
        }

        TMP_InputField[] pCardsI = GameObject.Find("Canvas/PlayerDeck/PCards").GetComponentsInChildren<TMP_InputField>();
        foreach (TMP_InputField card in pCardsI) {
            cardsNum.Add(card);
        }

        foreach (TMP_InputField ctn in cardsNum)
            ctn.text = "0";

        cardUIObjs = new List<DeckBuild_PreviewOnHover>(GameObject.Find("Canvas/PlayerDeck/PCards").transform.GetComponentsInChildren<DeckBuild_PreviewOnHover>());

        LoadDeck();
    }

    private void Update()
    {
        SetCards();
        SumCardNums();
    }

    private void SumCardNums()
    {
        int sum = 0;
        foreach (var num in cardsNum) {
            if (num.text.Length == 0)
                continue;
            int.TryParse(num.text[0].ToString(), out int txt);
            sum += txt;
        }
        cardSum.text = sum.ToString() + "/" + GameInfo.deckSize.ToString();
    }

    private bool ValidateCardNums()
    {
        for (int i = 0; i < cardsNum.Count; ++i) {
            var num = cardsNum[i];
            var name = cardsName[i];

            if (num.text.Length == 0)
                return false;
            bool goodParse = int.TryParse(num.text[0].ToString(), out int amount);
            if (!goodParse || (amount != 0 && amount != 1 && amount != 2)) {
                return false;
            }
            if (name.text == "     /") {
                if (amount != 0)
                    return false;
            }
        }
        return true;
    }

    private void SetCards()
    {
        for (int i = 0; i < cardUIObjs.Count; ++i) {
            if (i < ClassCardList(pClass).Length)
                cardUIObjs[i].card = ClassCardList(pClass)[i];
            else
                cardUIObjs[i].card = null;
        }
    }

    public void ResetCardsNum() {
        foreach (var num in cardsNum) {
            num.text = "0";
        }
    }

    public void TrySave()
    {
        if (pClass == "") {
            errorMessage.text = "Save Failed: Invalid Class";
            errorMessage.gameObject.SetActive(true);
            return;
        }

        if (!ValidateCardNums()) {
            errorMessage.text = "Save Failed: Amount of certain cards is invalid. It can be 0, 1, or 2; and only for listed cards.";
            errorMessage.gameObject.SetActive(true);
            return;
        }

        string deckSizeStr = GameInfo.deckSize.ToString();
        if (cardSum.text != (deckSizeStr + "/" + deckSizeStr)) {
            errorMessage.text = "Save Failed: Total amount of cards is invalid. It must be " + deckSizeStr;
            errorMessage.gameObject.SetActive(true);
            return;
        }

        errorMessage.text = "Success! Saved " + pClass + " deck.";
        errorMessage.gameObject.SetActive(true);

        DeckInfo deckInfo = new DeckInfo(pClass, GetDeckFromUI());
        string json = JsonUtility.ToJson(deckInfo);
        Directory.CreateDirectory(Application.dataPath + "/SaveData");
        File.WriteAllText(ClassFileLocation(), json);
    }

    private List<Card> GetDeckFromUI() {
        List<Card> deck = new List<Card>();
        for (int j = 0; j < ClassCardList(pClass).Length; ++j) {
            int amount = int.Parse(cardsNum[j].text[0].ToString());
            for (int i = 0; i < amount; ++i) {
                deck.Add(ClassCardList(pClass)[j]);
            }
        }
        return deck;
    }

    private void SetUIFromDeck(List<Card> deck) {
        for (int i = 0; i < ClassCardList(pClass).Length; ++i) {
            int amount = deck.FindAll(u => u == ClassCardList(pClass)[i]).Count;
            cardsNum[i].text = "" + amount;
        }
    }

    public void ChangepClass(int x)
    {
        pClass = GameInfo.classes[x];
        LoadDeck();
    }

    private Card[] ClassCardList(string clss)
    {
        switch (clss) {
            case "Knight":
                return classCards.knightCards;
            case "Ranger":
                return classCards.rangerCards;
            default:
                return null;
        }
    }

    private void LoadDeck() {
        if (!File.Exists(ClassFileLocation())) {
            ResetCardsNum();
            return;
        }

        string json = File.ReadAllText(ClassFileLocation());
        DeckInfo deckInfo = JsonUtility.FromJson<DeckInfo>(json);
        List<Card> deck = deckInfo.deck;
        if(DeckIsValid(deck))
            SetUIFromDeck(deck);
        return;
    }

    private bool DeckIsValid(List<Card> deck) {
        if (deck.Count != GameInfo.deckSize)
            return false;
        //probably enough for now..
        return true;
    }

    private string ClassFileLocation() {
        return Application.dataPath + "/SaveData/" + pClass + "DeckSave.txt";
    }
}
