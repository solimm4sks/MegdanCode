using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "CardAction/ShuffleNewCard")]
public class ShuffleNewCard : CardAction
{
    public Card card;
    public bool toTheBottom;

    public override IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction) {
        if (toTheBottom)
            pm.cardManager.AddCardToBottom(card, new CardInfo());
        else
            pm.cardManager.ShuffleNewCard(card, new CardInfo());

        cae.cardPlayState[currentAction] = 1;
        yield return null;
    }
}
