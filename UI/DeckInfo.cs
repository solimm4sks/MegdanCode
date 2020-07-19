using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckInfo
{
    public string playerClass;
    public List<Card> deck;

    public DeckInfo(string x, List<Card> d) {
        playerClass = x;
        deck = d;
    }
}
