using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : PlayerManager
{
    protected override void SetClassProperties() {
        playerInfo.basicRange = 3;
        playerInfo.basicIsPiercing = true;
        playerInfo.attack = 1;
    }
}
