using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardAction : ScriptableObject{
    public CardCheck[] actionChecks;

    public abstract IEnumerator Execute(PlayerManager pm, CardActionExecutor cae, int currentAction);
}
