using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckBuilder : MonoBehaviour
{
    private SceneLoader sc;
    private ClassCards classCards;

    [HideInInspector]public List<TextMeshProUGUI> p1CardsName;
    [HideInInspector] public List<TextMeshProUGUI> p1CardsNum;
    [HideInInspector] public List<TextMeshProUGUI> p2CardsName;
    [HideInInspector] public List<TextMeshProUGUI> p2CardsNum;
    private List<DeckBuild_PreviewOnHover> p1CardUIObjs;
    private List<DeckBuild_PreviewOnHover> p2CardUIObjs;
    private TextMeshProUGUI p1CardSum;
    private TextMeshProUGUI p2CardSum;

    public string p1Class = "";
    public string p2Class = "";
    public List<Card> p1Deck;
    public List<Card> p2Deck;

    private void Start() {
        DontDestroyOnLoad(gameObject);

        sc = GetComponent<SceneLoader>();
        classCards = GetComponent<ClassCards>();
        p1CardSum = GameObject.Find("Canvas/PlayerDeck1/CardSum").GetComponent<TextMeshProUGUI>();
        p2CardSum = GameObject.Find("Canvas/PlayerDeck2/CardSum").GetComponent<TextMeshProUGUI>();

        TextMeshProUGUI[] p1CardsT = GameObject.Find("Canvas/PlayerDeck1/PCards").GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI card in p1CardsT) {
            if (card.gameObject.name == "CardName")
                p1CardsName.Add(card);
            else if (card.gameObject.name == "InputText")
                p1CardsNum.Add(card);
        }
        TextMeshProUGUI[] p2CardsT = GameObject.Find("Canvas/PlayerDeck2/PCards").GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI card in p2CardsT) {
            if (card.gameObject.name == "CardName")
                p2CardsName.Add(card);
            else if (card.gameObject.name == "InputText")
                p2CardsNum.Add(card);
        }

        foreach (TextMeshProUGUI ctn in p1CardsNum)
            ctn.text = "0";
        foreach (TextMeshProUGUI ctn in p2CardsNum)
            ctn.text = "0";

        p1CardUIObjs = new List<DeckBuild_PreviewOnHover>(GameObject.Find("Canvas/PlayerDeck1/PCards").transform.GetComponentsInChildren<DeckBuild_PreviewOnHover>());
        p2CardUIObjs = new List<DeckBuild_PreviewOnHover>(GameObject.Find("Canvas/PlayerDeck2/PCards").transform.GetComponentsInChildren<DeckBuild_PreviewOnHover>());
    }

    private void Update() {
        SetCards();
        SumCardNums();    
    }

    private void SumCardNums() {
        int sum = 0;
        foreach (var num in p1CardsNum) {
            int.TryParse(num.text[0].ToString(), out int txt);
            sum += txt;
        }
        p1CardSum.text = sum.ToString() + "/" + GameInfo.deckSize.ToString();
        sum = 0;
        foreach (var num in p2CardsNum) {
            int.TryParse(num.text[0].ToString(), out int txt);
            sum += txt;
        }
        p2CardSum.text = sum.ToString() + "/" + GameInfo.deckSize.ToString();
    }

    private bool ValidateCardNums() {
        foreach (var num in p1CardsNum) {
            bool goodParse = int.TryParse(num.text[0].ToString(), out int txt);
            if (!goodParse || (txt != 0 && txt != 1 && txt != 2)) {
                return false;
            }
        }
        foreach (var num in p2CardsNum) {
            bool goodParse = int.TryParse(num.text[0].ToString(), out int txt);
            if (!goodParse || (txt != 0 && txt != 1 && txt != 2)) {
                return false;
            }
        }
        return true;
    }

    private void SetCards() {
        if(RealClass(p1Class))
            for(int i = 0; i < p1CardUIObjs.Count; ++i) {
                if(i < ClassCardList(p1Class).Length)
                    p1CardUIObjs[i].card = ClassCardList(p1Class)[i];
                else 
                    p1CardUIObjs[i].card = null;
            }
        if(RealClass(p2Class))
            for(int i = 0; i < p2CardUIObjs.Count; ++i) {
                if(i < ClassCardList(p2Class).Length)
                    p2CardUIObjs[i].card = ClassCardList(p2Class)[i];
                else
                    p2CardUIObjs[i].card = null;
            }
    }

    public void TryPlay() {
        string deckSizeStr = GameInfo.deckSize.ToString();
        if (p1Class == "" || p2Class == "" || !ValidateCardNums() || p1CardSum.text != (deckSizeStr + "/" + deckSizeStr) || p2CardSum.text != (deckSizeStr + "/" + deckSizeStr))
            return;

        for (int j = 0; j < ClassCardList(p1Class).Length; ++j) {
            int txt = int.Parse(p1CardsNum[j].text[0].ToString());
            for (int i = 0; i < txt; ++i) {
                p1Deck.Add(ClassCardList(p1Class)[j]);
            }
        }
        
        for (int j = 0; j < ClassCardList(p2Class).Length; ++j) {
            int txt = int.Parse(p2CardsNum[j].text[0].ToString());
            for (int i = 0; i < txt; ++i) {
                p2Deck.Add(ClassCardList(p2Class)[j]);
            }
        }

        sc.LoadScene("Scene1");
    }

    public void ChangeP1Class(string x) {
        x = x.ToLower();
        if (RealClass(x))
            p1Class = x;
        else
            p1Class = "";
    }

    public void ChangeP2Class(string x) {
        x = x.ToLower();
        if (RealClass(x))
            p2Class = x;
        else
            p2Class = "";
    }

    public bool RealClass(string x) {
        bool okClass = false;
        foreach (string _class in GameInfo.classes) {
            if (x == _class.ToLower()) {
                okClass = true;
                break;
            }
        }
        return okClass;
    }

    private Card[] ClassCardList(string clss) {
        switch (clss) {
            case "knight":
                return classCards.knightCards;
            case "ranger":
                return classCards.rangerCards;
            default:
                return null;
        }
    }
}
