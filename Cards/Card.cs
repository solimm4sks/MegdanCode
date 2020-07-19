using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Card")]
public class Card : ScriptableObject
{
    public string title = "";
    public Sprite art;
    public string desc = "";
    public string flavor = "";
    public int resolveCost = 1;

    public CardCostHandler cardCostHandler;
    public List<CardCheck> cardChecks;
    public List<CardAction> cardActions;

    public void Init(Card card) {
        title = card.title;
        art = card.art;
        desc = card.desc;
        flavor = card.flavor;
        resolveCost = card.resolveCost;
        cardChecks = card.cardChecks;
        cardActions = card.cardActions;
    }

    public void Init(string t, string d, string f, int rc) {
        title = t;
        desc = d;
        flavor = f;
        resolveCost = rc;
    }
    

}
