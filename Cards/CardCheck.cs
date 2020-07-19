using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardCheck : ScriptableObject{
    public abstract bool Check(PlayerManager pm);
}
