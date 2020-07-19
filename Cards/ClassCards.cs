using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ClassCards : MonoBehaviour
{
    public Card[] knightCards;
    public Card[] rangerCards;

    private void Awake() { //idk why its awake, i felt frisky i guess
        knightCards = Resources.LoadAll<Card>("Data/Cards/Knight");
        rangerCards = Resources.LoadAll<Card>("Data/Cards/Ranger");
    }
}
