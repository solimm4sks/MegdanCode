using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardCheck/IsHoldingXCards")]
public class IsHoldingXCards : CardCheck
{
    public int numOfCards;

    public override bool Check(PlayerManager pm)
    {
        if (pm.cardManager.HandCards().Count == numOfCards)
            return true;
        return false;
    }
}
